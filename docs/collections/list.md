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

- **Tail Appends (`Add`)**: Instant constant-time operations on average, punctuated by linear reallocations when boundaries split.
- **Index Access Lookup**: Instant direct hardware address offset mapping.
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

| Method | N | Mean | Gen 0 | Allocated |
| :--- | :--- | :--- | :--- | :--- |
| **Add** | 1000 | 2.782 us | 4.0207 | 8.23 KB |
| **PreSizedCapacityAdd** | 1000 | 2.347 us | 1.9379 | 3.96 KB |
| **InsertAtBeginning** | 1000 | 50.821 us | 3.9673 | 8.20 KB |
| | | | | |
| **Add** | 10000 | 27.361 us | 62.4695 | 128.32 KB |
| **PreSizedCapacityAdd** | 10000 | **22.997 us** | 18.8599 | **39.12 KB** |
| **InsertAtBeginning** | 10000 | **4,793.659 us** | 54.6875 | 128.29 KB |

### Interpretation

- **The Array Shifting Disaster**: The `InsertAtBeginning` method demonstrates a brutal layout penalty under scaling. Inserting 10,000 items sequentially via `.Insert(0, i)` takes a massive **4.79 milliseconds**—making it **over 200x slower** than standard tail additions. Because arrays are contiguous, adding an item at index 0 forces the processor to physically copy and shift all existing values forward one slot in memory on every single loop pass.
- **The Dynamic Resizing Overhead**: The standard `Add` method takes 27.3 us and allocates 128.3 KB of memory at N = 10,000. It starts with an empty layout, forcing .NET to execute multiple intermediate array doublings and internal block migrations as the collection grows.
- **The Pre-Sized Capacity Triumph**: The `PreSizedCapacityAdd` method resolves this initialization overhead. Specifying the target size in the constructor (`new List<int>(N)`) reduces total allocations down to **39.1 KB** (a **69.5% memory reduction**) and cuts execution time to 22.9 us. Pre-sizing creates exactly one single, perfectly matched memory array block upfront, completely eliminating dynamic resize operations and heap garbage collection traffic.

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
