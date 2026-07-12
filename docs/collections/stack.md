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

| Method | N | Mean | Gen 0 | Allocated |
| :--- | :--- | :--- | :--- | :--- |
| **StackThroughput** | 1000 | 5.989 us | 4.0207 | 8.23 KB |
| **PreSizedStackThroughput** | 1000 | 4.816 us | 1.9379 | 3.96 KB |
| **ListAddRemoveFromEnd** | 1000 | 4.780 us | 4.0207 | 8.23 KB |
| | | | | |
| **StackThroughput** | 10000 | 51.387 us | 62.4390 | 128.32 KB |
| **PreSizedStackThroughput** | 10000 | **49.420 us** | 18.8599 | **39.12 KB** |
| **ListAddRemoveFromEnd** | 10000 | **47.190 us** | 62.4390 | 128.32 KB |

### Interpretation

- **The Tail Mutation Parity**: Unlike operations at the front of array-backed structures, mutating elements at the very end of a stack or list involves zero memory shifting. Both `StackThroughput` and `ListAddRemoveFromEnd` run at almost identical speeds under scale (~51 us vs ~47 us). They simply assign values to index positions and adjust an internal pointer boundary.
- **The Pre-Sized Payload Optimization**: The `PreSizedStackThroughput` track highlights the advantages of eliminating dynamic resizing loops. Specifying the target size in the constructor (`new Stack<int>(N)`) drops memory allocation overhead down to **39.12 KB** (a **69.5% reduction** at N = 10,000). Pre-sizing eliminates multiple intermediate array duplications and protects the runtime from managed heap garbage collection cycles.

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
