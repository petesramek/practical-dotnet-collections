# Stack<T>

## What it is

Stack<T> is a LIFO (last-in, first-out) collection optimized for reverse-order processing.

## Typical use cases
- Undo/redo operations
- Expression evaluation
- Backtracking algorithms

## Sample usage

See:
[samples/stack-undo-actions.cs](../../samples/stack-undo-actions.cs)

### How to run the sample

```bash
dotnet run samples/stack-undo-actions.cs
```

## Internal implementation

Stack<T> uses an array-backed structure similar to List<T>.

### Lookup flow
- No lookup support
- Only Push / Pop / Peek

## Memory characteristics
- Backed by array
- Resizes when needed
- Good cache locality

## Complexity overview

Push: O(1) amortized  
Pop: O(1)  
Lookup: not supported

## Benchmark results

### Scenario

Compare stack behavior vs list equivalent:
- Stack<T>.Push/Pop
- List<T>.Add/RemoveAt(end)

### How to run this benchmark only

```bash
cd benchmarks
dotnet run -c Release -- --filter *Iteration.StackBenchmark*
```

### Benchmark code

[benchmarks/Iteration/StackBenchmark.cs](../../benchmarks/Iteration/StackBenchmark.cs)

### Results

Environment: BenchmarkDotNet 0.15.8; Windows 11 (10.0.26300.9711); virtual CPU `virt-10.0` at 1.00 GHz (1 CPU; 4 logical / 4 physical cores); .NET SDK 10.0.403; .NET 10.0.14, Arm64 RyuJIT (`armv8.0-a`); 3 warmup and 8 measurement iterations. [Full report](../../BenchmarkDotNet.Artifacts/results/PracticalDotNetCollections.Benchmarks.Iteration.StackBenchmark-report-github.md).

| Method                  | N     | Mean      | Error     | StdDev    | Gen0    | Allocated |
|------------------------ |------ |----------:|----------:|----------:|--------:|----------:|
| **StackThroughput**         | **1000**  |  **6.616 μs** | **0.3903 μs** | **0.2041 μs** |  **4.0207** |   **8.23 KB** |
| PreSizedStackThroughput | 1000  |  6.253 μs | 0.4390 μs | 0.2296 μs |  1.9379 |   3.96 KB |
| ListAddRemoveFromEnd    | 1000  |  6.342 μs | 0.8112 μs | 0.4243 μs |  4.0207 |   8.23 KB |
| **StackThroughput**         | **10000** | **67.579 μs** | **4.1846 μs** | **1.8580 μs** | **62.3779** | **128.32 KB** |
| PreSizedStackThroughput | 10000 | 60.444 μs | 2.8204 μs | 1.2523 μs | 18.7988 |  39.12 KB |
| ListAddRemoveFromEnd    | 10000 | 59.482 μs | 1.5605 μs | 0.8162 μs | 62.3779 | 128.32 KB |

### Interpretation

- **Tail mutation**: at N = 10,000, `StackThroughput` averaged **67.6 us** and `ListAddRemoveFromEnd` averaged **59.5 us**. Both perform operations at the end of array-backed collections; the current means are not identical.
- **Pre-sized capacity**: at N = 10,000, `PreSizedStackThroughput` allocated **39.1 KB** versus **128 KB** for the default stack (about 70% less). Its mean was 60.4 us versus 67.6 us, with Error margins that narrowly do not overlap in this run.

## Practical optimizations
- Use for LIFO scenarios only
- **Always provide a preallocated starting capacity if known**: If your tracking pool loads an explicit chunk of data rows all at once (such as an undo log sequence), pass that expected count straight to the constructor (`new Stack<T>(capacity)`). This locks down a single array block upfront, completely bypassing dynamic expansion overhead.
- **Consolidate validations into atomic extractions**: Do not run separate presence checks like testing `if (stack.Count > 0)` before calling `.Pop()`. Use `.TryPop()` to check presence and extract items in a single, safe step.

## Common mistakes
- Using when order matters differently (FIFO)
- **Using dynamic arrays individually without preallocation**: Relying on un-sized stacks inside high-frequency processing loops forces continuous internal array doubling cycles.

## When I would choose it
- Reverse processing needed
- Last-in-first-out semantics required

## When I would avoid it
- Need FIFO → use Queue<T>
- Need lookup → use other collections

## Rule of thumb

Use Stack<T> when last-in-first-out behavior is required.
