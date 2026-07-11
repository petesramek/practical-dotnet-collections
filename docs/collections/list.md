# List<T>

## What it is

`List<T>` is a dynamic array that provides fast indexed access and efficient sequential processing.

## Typical use cases
- Sequential processing
- Ordered data storage
- Accumulating results

## Sample usage

See:
[samples/list-processing.cs](../../samples/list-processing.cs)

### How to run the sample

```bash
dotnet run samples/list-processing.cs
```

Notes:
- Requires .NET 10 or later
- Runs as a standalone script

## Internal implementation

`List<T>` is backed by a contiguous array.

### Lookup flow
- Direct index access (`list[i]`) is O(1)
- Lookup via `Contains` requires sequential scan

## Memory characteristics
- Uses a contiguous array
- Resizes by allocating a larger array and copying elements
- Capacity typically grows by ~2x
- Good cache locality

## Complexity overview

Lookup: O(n)  
Add: O(1) amortized  
Insert: O(n)  
Remove: O(n)

## Benchmark results

### Scenario

Compare add vs insert performance:
- `List<T>.Add`
- `List<T>.Insert(0, item)`

### How to run this benchmark only

```bash
cd benchmarks
dotnet run -c Release -- --filter *ListAddInsertBenchmark*
```

### Benchmark code

[benchmarks/Add/ListAddInsertBenchmark.cs](../../benchmarks/Add/ListAddInsertBenchmark.cs)

### Results

(To be filled after running benchmark)

### Interpretation

(To be filled after running benchmark)

## Practical optimizations
- Pre-size list when size is known: `new List<T>(capacity)`
- Prefer `Add` over `Insert` for large collections

## Common mistakes
- Using `Insert(0, ...)` in loops
- Using `Contains` for frequent lookups

## When I would choose it
- Ordered data is required
- Sequential processing dominates
- As default collection

## When I would avoid it
- Frequent lookups → use `HashSet<T>` or `Dictionary<TKey, TValue>`
- Frequent inserts in the middle → consider `LinkedList<T>`

## Rule of thumb

Start with `List<T>` unless you have a reason not to.
