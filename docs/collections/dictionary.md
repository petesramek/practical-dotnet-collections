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

- **Key Lookups**: Near-instant flat scaling performance via hash-bucket mapping logic.
- **Insertions/Deletions**: Near-instant bucket tracking, except when insertions trigger an internal resize operation.

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
| **DictionaryLookup** | 10000 | 3.854 ns | - | - |
| **ListLookup** | 10000 | 460.350 ns | - | - |
| **DoubleLookupUpdate** | 10000 | 10.188 ns | - | - |
| **SingleLookupTryGetValueUpdate** | 10000 | 6.408 ns | - | - |
| **LookupWithUnoptimizedStructKey** | 10000 | 31.524 ns | 0.0344 | 72 B |
| **LookupWithOptimizedStructKey** | 10000 | 3.481 ns | - | - |
| | | | | |
| **DictionaryLookup** | 100000 | 4.131 ns | - | - |
| **ListLookup** | 100000 | 4,578.521 ns | - | - |
| **DoubleLookupUpdate** | 100000 | 11.481 ns | - | - |
| **SingleLookupTryGetValueUpdate** | 100000 | 7.325 ns | - | - |
| **LookupWithUnoptimizedStructKey** | 100000 | 34.018 ns | 0.0344 | 72 B |
| **LookupWithOptimizedStructKey** | 100000 | 4.423 ns | - | - |

### Interpretation

- **The Sublinear Scaling Triumph**: As data grows 10x (from N = 10,000 to 100,000), `ListLookup` search degradation scales linearly, jumping from 460 ns to a brutal **4,578.5 ns**. In sharp contrast, `DictionaryLookup` performance remains completely flat, scaling only from 3.8 ns to **4.1 ns**—making it **over 1,100x faster** than a list. Dictionaries bypass item-by-item array scanning entirely by using instant math calculations on the key's hash value.
- **The Double-Lookup Penalty**: Checking `.ContainsKey()` before retrieving or updating a value (`DoubleLookupUpdate`) takes 11.48 ns. Using `.TryGetValue()` instead reduces the execution time to **7.32 ns**, making it **36% faster**. The single-pass approach prevents the collection from executing two completely independent hash code calculations and duplicate bucket pointer evaluations.
- **The Structural Boxing Catastrophe**: Utilizing a custom value type (`struct`) as a dictionary key without explicit overrides (`LookupWithUnoptimizedStructKey`) results in poor performance. It slows lookup speeds down to 34.01 ns and leaves **72 Bytes** of heap garbage behind *on every single lookup pass*. This happens because .NET is forced to fallback to reflection and box the struct into a generic object to use the default `Object.GetHashCode` implementation. Implementing `IEquatable<T>` and overriding `GetHashCode()` (`LookupWithOptimizedStructKey`) completely eliminates this heap pollution and speeds up lookup execution by **nearly 10x**.

## Practical optimizations
- **Always provide a preallocated initial capacity if known**: If you know how many keys will reside inside your dictionary cache, specify it upfront in the constructor (`new Dictionary<TKey, TValue>(capacity)`). This completely prevents expensive internal resizing operations and array re-allocations.
- **Consolidate multi-step validations into a single operation**: Never write validation checks that combine separate dictionary checks back-to-back (such as checking `.ContainsKey()` followed immediately by an indexer extraction). Use `.TryGetValue()` or the modern `.CollectionsMarshal` extensions to achieve single-pass lookup paths.

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
