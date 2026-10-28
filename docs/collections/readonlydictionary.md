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

- **Key Lookups**: Fast flat hash mapping performance delegated directly to the wrapped table.
- **Collection Wrap Initializations**: Constant-time constant-time reference proxy linkage (O(1)).

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

Environment: BenchmarkDotNet 0.15.8; Windows 11 (10.0.26300.9711); virtual CPU `virt-10.0` at 1.00 GHz (1 CPU; 4 logical / 4 physical cores); .NET SDK 10.0.403; .NET 10.0.14, Arm64 RyuJIT (`armv8.0-a`); 3 warmup and 8 measurement iterations. [Full report](../../BenchmarkDotNet.Artifacts/results/PracticalDotNetCollections.Benchmarks.Lookup.ReadOnlyDictionaryBenchmark-report-github.md).

| Method                           | N     | Mean          | Error         | StdDev      | Gen0   | Allocated |
|--------------------------------- |------ |--------------:|--------------:|------------:|-------:|----------:|
| **DictionaryLookup**                 | **1000**  |      **4.570 ns** |     **0.3263 ns** |   **0.1706 ns** |      **-** |         **-** |
| ReadOnlyDictionaryLookup         | 1000  |      7.420 ns |     3.0771 ns |   1.6094 ns |      - |         - |
| IterateDictionaryForeach         | 1000  |  1,187.709 ns |    13.5001 ns |   5.9941 ns |      - |         - |
| IterateReadOnlyDictionaryForeach | 1000  |  3,138.183 ns |   225.7794 ns | 118.0870 ns | 0.0229 |      48 B |
| **DictionaryLookup**                 | **10000** |      **4.486 ns** |     **0.3159 ns** |   **0.1652 ns** |      **-** |         **-** |
| ReadOnlyDictionaryLookup         | 10000 |      6.295 ns |     1.8386 ns |   0.8163 ns |      - |         - |
| IterateDictionaryForeach         | 10000 | 12,048.081 ns |   343.3509 ns | 152.4499 ns |      - |         - |
| IterateReadOnlyDictionaryForeach | 10000 | 31,103.656 ns | 1,005.5211 ns | 525.9070 ns |      - |      48 B |

### Interpretation

- **Lookup cost**: at N = 10,000, `ReadOnlyDictionaryLookup` averaged **6.30 ns** versus **4.49 ns** for `DictionaryLookup`. Their Error ranges overlap, so this run does not establish a reliable lookup penalty.
- **The foreach interface cost**: `IterateReadOnlyDictionaryForeach` averaged **31.1 us** at N = 10,000 versus **12.0 us** for `IterateDictionaryForeach`, and allocated 48 B. The direct dictionary enumeration allocated nothing in this run.

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
- When you require a zero-allocation read-only view proxy that automatically and immediately reflects live structural updates handled by a background system task.

## When I would avoid it
- Need true immutability → use ImmutableDictionary
- On extreme high-frequency execution tracks where raw loop iteration speed across entries is your highest application metric. Use direct dictionary references or refactor to flat arrays.

## Rule of thumb

Use ReadOnlyDictionary<TKey, TValue> as a wrapper, not as an immutable collection.
