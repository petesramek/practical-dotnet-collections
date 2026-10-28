# Dictionary<TKey, TValue>

## What it is

`Dictionary<TKey, TValue>` is a hash-based key-value collection optimized for fast lookups by key.
It provides near constant-time access to values based on a unique key.

## Typical use cases
- Caching (avoid repeated work for the same key)
- ID → entity lookup (e.g. `userId` → user)
- Deduplication with associated data
- Indexing data for fast access

## Sample usage

See:
[samples/dictionary-caching.cs](../../samples/dictionary-caching.cs)

### How to run the sample

```bash
dotnet run samples/dictionary-caching.cs
```

## Internal implementation

`Dictionary<TKey, TValue>` is implemented using:
- buckets array
- entries array

### Lookup flow
- Compute hash code
- Map to bucket
- Traverse collision chain
- Compare keys

## Memory characteristics
- Extra arrays (buckets + entries)
- Higher overhead than `List<T>`
- Resizes cause rehashing

## Complexity overview

- **Key Lookups**: Fast flat scaling performance via hash-bucket mapping logic.
- **Insertions/Deletions**: Fast bucket tracking, except when insertions trigger an internal resize operation.

## Benchmark results

### Scenario

Compare lookup performance:
- `Dictionary<int, int>`
- `List<T>.Contains`

### How to run this benchmark only

```bash
cd benchmarks
dotnet run -c Release -- --filter *Lookup.DictionaryBenchmark*
```

### Benchmark code

[benchmarks/Lookup/DictionaryBenchmark.cs](../../benchmarks/Lookup/DictionaryBenchmark.cs)

### Results

| Method | N | Mean | Gen 0 | Allocated |
| :--- | :--- | :--- | :--- | :--- |
| **DictionaryLookup** | 10000 | 6.138 ns | - | - |
| **ListLookup** | 10000 | 721.414 ns | - | - |
| **FourLookupUpdate** | 10000 | 17.310 ns | - | - |
| **TwoLookupTryGetValueUpdate** | 10000 | 12.423 ns | - | - |
| **SingleLookupGetValueRefUpdate** | 10000 | 7.263 ns | - | - |
| **LookupWithUnoptimizedStructKey** | 10000 | 58.852 ns | 0.0343 | 72 B |
| **LookupWithOptimizedStructKey** | 10000 | 6.591 ns | - | - |
| | | | | |
| **DictionaryLookup** | 100000 | 9.178 ns | - | - |
| **ListLookup** | 100000 | 7,293.714 ns | - | - |
| **FourLookupUpdate** | 100000 | 22.861 ns | - | - |
| **TwoLookupTryGetValueUpdate** | 100000 | 15.859 ns | - | - |
| **SingleLookupGetValueRefUpdate** | 100000 | 9.779 ns | - | - |
| **LookupWithUnoptimizedStructKey** | 100000 | 80.209 ns | 0.0343 | 72 B |
| **LookupWithOptimizedStructKey** | 100000 | 8.522 ns | - | - |

Numbers come from a short BenchmarkDotNet job (3 warmup, 8 iterations) on an Arm64 machine with .NET 10; absolute values vary by hardware, so compare ratios rather than nanoseconds.

### Interpretation

- **Scaling**: When N grows 10x (10,000 to 100,000), `ListLookup` grows roughly 10x (721 ns to 7,294 ns), as expected for a linear scan. `DictionaryLookup` stays in the same order of magnitude (6.1 ns to 9.2 ns), because the cost depends on the key's hash rather than on the number of items.
- **Number of lookups per update**: the three update variants differ in how many hash lookups they do. `FourLookupUpdate` does `ContainsKey`, a get, a set and a final get (4 lookups). `TwoLookupTryGetValueUpdate` does `TryGetValue` plus an indexer set (2 lookups). `SingleLookupGetValueRefUpdate` uses `CollectionsMarshal.GetValueRefOrNullRef` and writes through the returned `ref` (1 lookup). At N = 100,000 they take 22.9 ns, 15.9 ns and 9.8 ns respectively, so each removed lookup saves a measurable amount of time.
- **Struct keys**: a custom `struct` key without `IEquatable<T>` and `GetHashCode()` overrides (`LookupWithUnoptimizedStructKey`) took 58.9 ns to 80.2 ns and allocated 72 B per lookup, versus 6.6 ns to 8.5 ns and no allocation for the optimized key (`LookupWithOptimizedStructKey`). The default equality path goes through `object`-based comparison, which boxes the struct on every call. Implementing `IEquatable<T>` and overriding `GetHashCode()` removes the boxing; see [struct dictionary keys](../advanced/struct-dictionary-keys.md).

## Practical optimizations
- **Always provide a preallocated initial capacity if known**: If you know how many keys will reside inside your dictionary cache, specify it upfront in the constructor (`new Dictionary<TKey, TValue>(capacity)`). This completely prevents expensive internal resizing operations and array re-allocations.
- **Consolidate multi-step validations into a single operation**: Never write validation checks that combine separate dictionary checks back-to-back (such as checking `.ContainsKey()` followed immediately by an indexer extraction). Use `.TryGetValue()` or the modern `CollectionsMarshal.GetValueRefOrNullRef` to avoid repeated lookups.

## Common mistakes
- **Using a raw struct as a key without overriding equality**: As proven by the metrics, forgetting to implement `IEquatable<T>` on a custom key value type triggers hidden heap allocations and slows down operations on hot execution paths.
- **Modifying or mutating a key object after insertion**: A dictionary organizes its inner structure based on the hash code of the key at the exact moment it was added. If you change a property on a key object after inserting it, its hash code changes, making the entry permanently lost and unretrievable.

## When I would choose it
- When mapping unique data keys to object entities where read or update counts scale heavily into thousands of iterations.
- When direct item retrieval performance is your highest technical priority, and base memory size is a secondary constraint.

## When I would avoid it
- When managing tiny datasets (under a dozen items), where a simple linear `List<T>` search is slightly faster and takes significantly less memory.
- When you require keys to remain in a strictly sorted numerical or alphabetical sequence. Use `SortedList<TKey, TValue>` or `SortedDictionary<TKey, TValue>` instead.

## Rule of thumb

Use `Dictionary<TKey, TValue>` when you need fast lookup by key.
