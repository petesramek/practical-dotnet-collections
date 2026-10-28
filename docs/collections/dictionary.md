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

Environment: BenchmarkDotNet 0.15.8; Windows 11 (10.0.26300.9711); virtual CPU `virt-10.0` at 1.00 GHz (1 CPU; 4 logical / 4 physical cores); .NET SDK 10.0.403; .NET 10.0.14, Arm64 RyuJIT (`armv8.0-a`); 3 warmup and 8 measurement iterations. [Full report](../../BenchmarkDotNet.Artifacts/results/PracticalDotNetCollections.Benchmarks.Lookup.DictionaryBenchmark-report-github.md).

| Method                         | N      | Mean         | Error      | StdDev     | Gen0   | Allocated |
|------------------------------- |------- |-------------:|-----------:|-----------:|-------:|----------:|
| **DictionaryLookup**               | **10000**  |     **4.773 ns** |  **0.4031 ns** |  **0.2108 ns** |      **-** |         **-** |
| ListLookup                     | 10000  |   574.105 ns | 15.5259 ns |  8.1203 ns |      - |         - |
| FourLookupUpdate               | 10000  |    13.303 ns |  0.7678 ns |  0.4016 ns |      - |         - |
| TwoLookupTryGetValueUpdate     | 10000  |     8.729 ns |  0.8710 ns |  0.3867 ns |      - |         - |
| SingleLookupGetValueRefUpdate  | 10000  |     5.790 ns |  0.7577 ns |  0.3364 ns |      - |         - |
| LookupWithUnoptimizedStructKey | 10000  |    43.593 ns |  4.3794 ns |  1.9445 ns | 0.0344 |      72 B |
| LookupWithOptimizedStructKey   | 10000  |     5.001 ns |  0.1366 ns |  0.0607 ns |      - |         - |
| **DictionaryLookup**               | **100000** |     **5.580 ns** |  **0.4076 ns** |  **0.1454 ns** |      **-** |         **-** |
| ListLookup                     | 100000 | 5,591.395 ns | 37.0815 ns | 16.4644 ns |      - |         - |
| FourLookupUpdate               | 100000 |    14.965 ns |  0.6439 ns |  0.3368 ns |      - |         - |
| TwoLookupTryGetValueUpdate     | 100000 |    11.327 ns |  2.2877 ns |  1.1965 ns |      - |         - |
| SingleLookupGetValueRefUpdate  | 100000 |     6.432 ns |  0.2850 ns |  0.1265 ns |      - |         - |
| LookupWithUnoptimizedStructKey | 100000 |    45.715 ns |  4.5121 ns |  2.3599 ns | 0.0344 |      72 B |
| LookupWithOptimizedStructKey   | 100000 |     6.471 ns |  0.7380 ns |  0.3277 ns |      - |         - |

### Interpretation

- **Scaling**: `ListLookup` scans the collection, so its work grows with N; `DictionaryLookup` hashes the key and does not scan all entries. Compare matching N rows and use Error and StdDev to judge the size of small differences.
- **Number of lookups per update**: `FourLookupUpdate`, `TwoLookupTryGetValueUpdate`, and `SingleLookupGetValueRefUpdate` perform four, two, and one dictionary lookups respectively. At N = 100,000, their means were 14.97 ns, 11.33 ns, and 6.43 ns; the latter two comparisons should still be read alongside their Error margins.
- **Struct keys**: `LookupWithUnoptimizedStructKey` allocated 72 B per operation at N = 100,000, while `LookupWithOptimizedStructKey` allocated none. Implementing `IEquatable<T>` and overriding `GetHashCode()` avoids the boxing path; see [struct dictionary keys](../advanced/struct-dictionary-keys.md).

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
