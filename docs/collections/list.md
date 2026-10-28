# List<T>

## What it is

`List<T>` is a dynamic array that provides fast indexed access and efficient sequential processing.

## Typical use cases
- Sequential processing
- Ordered data storage
- Accumulating results

## Sample usage

See:
[samples/list-batch-buffering.cs](../../samples/list-batch-buffering.cs)

### How to run the sample

```bash
dotnet run samples/list-batch-buffering.cs
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

- **Tail Appends (`Add`)**: Constant-time constant-time operations on average, punctuated by linear reallocations when boundaries split.
- **Index Access Lookup**: Constant-time direct hardware address offset mapping.
- **Middle/Front Mutations (`Insert`/`RemoveAt`)**: Linear vector shifting penalties proportional to row counts.

## Benchmark results

### Scenario

Compare add vs insert performance:
- `List<T>.Add`
- `List<T>.Insert(0, item)`

### How to run this benchmark only

```bash
cd benchmarks
dotnet run -c Release -- --filter *Add.ListBenchmark*
```

### Benchmark code

[benchmarks/Add/ListBenchmark.cs](../../benchmarks/Add/ListBenchmark.cs)

### Results

Environment: BenchmarkDotNet 0.15.8; Windows 11 (10.0.26300.9711); virtual CPU `virt-10.0` at 1.00 GHz (1 CPU; 4 logical / 4 physical cores); .NET SDK 10.0.403; .NET 10.0.14, Arm64 RyuJIT (`armv8.0-a`); 3 warmup and 8 measurement iterations. [Full report](../../BenchmarkDotNet.Artifacts/results/PracticalDotNetCollections.Benchmarks.Add.ListBenchmark-report-github.md).

| Method              | N     | Mean         | Error       | StdDev      | Gen0    | Allocated |
|-------------------- |------ |-------------:|------------:|------------:|--------:|----------:|
| **Add**                 | **1000**  |     **4.372 μs** |   **0.4840 μs** |   **0.2149 μs** |  **4.0207** |   **8.23 KB** |
| PreSizedCapacityAdd | 1000  |     4.146 μs |   0.8742 μs |   0.4572 μs |  1.9379 |   3.96 KB |
| InsertAtBeginning   | 1000  |    77.772 μs |   9.3019 μs |   4.1301 μs |  3.9063 |    8.2 KB |
| **Add**                 | **10000** |    **45.933 μs** |   **3.1941 μs** |   **1.4182 μs** | **62.4390** | **128.32 KB** |
| PreSizedCapacityAdd | 10000 |    36.292 μs |   3.3583 μs |   1.4911 μs | 18.8599 |  39.12 KB |
| InsertAtBeginning   | 10000 | 7,526.086 μs | 507.7377 μs | 225.4387 μs | 46.8750 | 128.29 KB |

### Interpretation

- **The array shifting cost**: inserting 10,000 items sequentially at index 0 with `InsertAtBeginning` averaged **7.53 ms**, compared with **45.9 us** for `Add`. The array-backed list shifts existing elements on each front insertion.
- **The dynamic resizing overhead**: at N = 10,000, default-capacity `Add` averaged **45.9 us** and allocated **128 KB**.
- **Pre-sized capacity**: `PreSizedCapacityAdd` averaged **36.3 us** and allocated **39.1 KB** at N = 10,000 (about 70% less allocation). At N = 100,000, allocation was 1.05 MB by default versus 400 KB pre-sized, while the Error margins for the timing means were large and overlap; this run does not establish a reliable speedup at that size.

## Practical optimizations
- Pre-size list when size is known: `new List<T>(capacity)`
- Prefer `Add` over `Insert` for large collections
- **Bypass sequential searches using separate index lookups**: If your pipeline executes `.Contains()`, `.Find()`, or `.Remove()` calls continuously against giant lists, you are triggering linear O(N) element scans. Sort the collection first to use binary searching, or complement the structure using a tracking `HashSet<T>` if unique key checks dominate.

## Common mistakes
- Using `Insert(0, ...)` in loops
- Using `Contains` for frequent lookups
- **Relying on .Count checking loops before appending records**: Lists naturally scale their boundaries internally. Do not write complex allocation checks or capacity modifications yourself; let the native internal buffer array manage layout expansions automatically if capacity scales are variable.

## When I would choose it
- Ordered data is required
- Sequential processing dominates
- As default collection
- When multi-threaded access is absent and rapid direct index lookup performance is your primary architectural constraint.

## When I would avoid it
- Frequent lookups → use `HashSet<T>` or `Dictionary<TKey, TValue>`
- Frequent inserts in the middle → consider `LinkedList<T>`
- When multiple background execution paths require concurrent modifications to the same collection structure without global locks. Use `ConcurrentQueue<T>` or `ConcurrentBag<T>` instead.

## Rule of thumb

Start with `List<T>` unless you have a reason not to.
