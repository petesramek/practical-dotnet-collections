# SortedList<TKey, TValue>

## What it is

SortedList<TKey, TValue> stores keys and values in two tightly packed arrays that are always kept sorted.

## Typical use cases
- Small, read-heavy sorted datasets
- Configuration or lookup tables loaded once

## Internal implementation

Uses two arrays (keys + values). Inserts require shifting elements to keep order.

## Memory characteristics
- Very compact (no per-node allocations)
- Better cache locality than tree-based collections

## Complexity overview

Lookup: O(log n)  
Add: O(n) (due to shifting)  
Remove: O(n)

## Benchmark results

### Scenario

Compare insertion:
- SortedList<TKey, TValue>
- SortedDictionary<TKey, TValue>

### How to run this benchmark only

`ash
cd benchmarks
dotnet run -c Release -- --filter *SortedListAddBenchmark*
`

### Benchmark code

[benchmarks/Add/SortedListAddBenchmark.cs](../../benchmarks/Add/SortedListAddBenchmark.cs)

### Results

(To be filled after running benchmark)

### Interpretation

(To be filled after running benchmark)

## Practical optimizations
- Use when data is loaded once and rarely modified

## Common mistakes
- Using for frequently changing data

## When I would choose it
- Memory efficiency matters
- Mostly read operations

## When I would avoid it
- Frequent inserts/removes → use SortedDictionary

## Rule of thumb

Use SortedList<TKey, TValue> for compact, mostly-read sorted data.
