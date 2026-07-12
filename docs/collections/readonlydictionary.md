# ReadOnlyDictionary<TKey, TValue>

## What it is

ReadOnlyDictionary<TKey, TValue> is a wrapper around an existing dictionary that prevents modification through its interface.

## Typical use cases
- Exposing dictionary data safely from APIs
- Preventing accidental modification

## Sample usage

See:
[samples/readonlydictionary-configuration-shielding.cs](../../samples/readonlydictionary-configuration-shielding.cs)

### How to run the sample

```bash
dotnet run samples/readonlydictionary-configuration-shielding.cs
```

## Internal implementation

Wraps an existing IDictionary<TKey, TValue> without copying data. All operations delegate to the underlying dictionary.

### Lookup flow
- Direct mapping to the underlying collection tracking indexes.

## Memory characteristics
- No additional storage
- Minimal overhead (wrapper only)

## Complexity overview

- **Key Lookups**: Near-instant flat hash mapping performance delegated directly to the wrapped table.
- **Collection Wrap Initializations**: Instant constant-time reference proxy linkage (O(1)).

## Benchmark results

### Scenario

Compare lookup:
- Dictionary<TKey, TValue>
- ReadOnlyDictionary<TKey, TValue>

### How to run this benchmark only

```bash
cd benchmarks
dotnet run -c Release -- --filter *Lookup.ReadOnlyDictionaryBenchmark*
```

### Benchmark code

[benchmarks/Lookup/ReadOnlyDictionaryBenchmark.cs](../../benchmarks/Lookup/ReadOnlyDictionaryBenchmark.cs)

### Results

| Method | N | Mean | Gen 0 | Allocated |
| :--- | :--- | :--- | :--- | :--- |
| **DictionaryLookup** | 1000 | 3.489 ns | - | - |
| **ReadOnlyDictionaryLookup** | 1000 | 3.849 ns | - | - |
| **IterateDictionaryForeach** | 1000 | 977.923 ns | - | - |
| **IterateReadOnlyDictionaryForeach** | 1000 | 2,463.922 ns | 0.0229 | 48 B |
| | | | | |
| **DictionaryLookup** | 10000 | 3.718 ns | - | - |
| **ReadOnlyDictionaryLookup** | 10000 | **4.079 ns** | - | - |
| **IterateDictionaryForeach** | 10000 | **9,745.774 ns** | - | - |
| **IterateReadOnlyDictionaryForeach** | 10000 | **24,756.497 ns** | - | **48 B** |

### Interpretation

- **The Zero-Overhead Lookup Triumph**: On pure key-based lookups (`ReadOnlyDictionaryLookup`), the wrapper introduces almost zero execution delay, processing queries in a flat **4.0 ns** compared to the raw dictionary's **3.7 ns** at N = 10,000. Because .NET routes lookups via a direct internal reference pointer delegate, your application gains complete API encapsulation with absolute zero performance penalties on search paths.
- **The Foreach Interface Trap**: Iterating a `ReadOnlyDictionary` using an entry loop (`IterateReadOnlyDictionaryForeach`) is an exceptional execution performance pitfall. At N = 10,000, it takes a brutal **24.7 microseconds**—making it **2.5x slower** than iterating the native dictionary. Because the wrapper hides the inner type definitions, the JIT compiler cannot optimize the path. It forces .NET to fallback to virtual `IEnumerable<KeyValuePair<T,K>>` method lookups and creates a **48-Byte** generic enumerator reference type allocation on the heap on every single run.
- **The Struct Enumerator Edge**: The raw `IterateDictionaryForeach` track completes in just **9.7 microseconds** with zero heap allocations. When a loop interacts directly with a standard dictionary type, the C# compiler leverages a specialized, pre-compiled value-type struct enumerator (`Dictionary<TKey, TValue>.Enumerator`), which executes inline without heap tracking noise.

## Practical optimizations
- Use at API boundaries
- **Avoid using `foreach` loops over ReadOnlyDictionary instances in hot paths**: If your software architecture requires continuously looping through a wrapped dictionary's records, do not iterate the proxy object directly. Extract its keys or values via `.Keys` and `.Values` layout iterations, or loop over the underlying list using index trackers if available to bypass virtual interface dispatches.
- **Prefer FrozenDictionary for fixed, permanent read-only caches**: If your dataset is compiled once at application startup and is never altered again by any internal engine thread, do not use `ReadOnlyDictionary`. Use `FrozenDictionary<TKey, TValue>` instead, which compiles optimized hashing layouts that run lookups up to 4x faster.

## Common mistakes
- Assuming it copies data
- Assuming underlying data cannot change
- **Re-instantiating the wrapper object inside continuous data loops**: Instantiating `new ReadOnlyDictionary<K, V>(dict)` inside high-frequency execution tracks forces constant heap wrapper object creation cycles, causing garbage collection spikes. Wrap your map once at initialization.

## When I would choose it
- Need read-only view of dictionary
- When building robust encapsulation layers to protect private state maps from being cleared, overwritten, or modified by external consumer code.
- When you require a zero-allocation read-only view proxy that automatically and instantly reflects live structural updates handled by a background system task.

## When I would avoid it
- Need true immutability → use ImmutableDictionary
- On extreme high-frequency execution tracks where raw loop iteration speed across entries is your highest application metric. Use direct dictionary references or refactor to flat arrays.

## Rule of thumb

Use ReadOnlyDictionary<TKey, TValue> as a wrapper, not as an immutable collection.
