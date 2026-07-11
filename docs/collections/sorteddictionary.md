# SortedDictionary<TKey, TValue>

## What it is

SortedDictionary<TKey, TValue> is a tree-based collection that maintains key/value pairs in sorted order by key.

## Typical use cases
- Ordered key/value data
- Range queries
- Iterating keys in sorted order

## Sample usage

See:
[samples/sorteddictionary-ordering.cs](../../samples/sorteddictionary-ordering.cs)

### How to run the sample

`ash
dotnet run samples/sorteddictionary-ordering.cs
`

## Internal implementation

Implemented as a balanced binary search tree (red-black tree).

### Lookup flow
- Tree traversal (O(log n))

## Memory characteristics
- Node-based structure
- Higher overhead than Dictionary
- Poorer cache locality than array-based collections

## Complexity overview

Lookup: O(log n)  
Add: O(log n)  
Remove: O(log n)

## Benchmark results

### Scenario

Compare lookup performance:
- SortedDictionary<TKey, TValue>.ContainsKey
- Dictionary<TKey, TValue>.ContainsKey

### How to run this benchmark only

`ash
cd benchmarks
dotnet run -c Release -- --filter *SortedDictionaryLookupBenchmark*
`

### Benchmark code

[benchmarks/Lookup/SortedDictionaryLookupBenchmark.cs](../../benchmarks/Lookup/SortedDictionaryLookupBenchmark.cs)

### Results

(To be filled after running benchmark)

### Interpretation

(To be filled after running benchmark)

## Practical optimizations
- Use when ordering is required
- Avoid if only lookup speed matters

## Common mistakes
- Using SortedDictionary instead of Dictionary for lookup-heavy workloads

## When I would choose it
- Need sorted key/value pairs
- Need ordered iteration

## When I would avoid it
- Need fastest lookup → use Dictionary<TKey, TValue>

## Rule of thumb

Use SortedDictionary<TKey, TValue> when ordering matters, otherwise prefer Dictionary<TKey, TValue>.
