# Stack<T>

## What it is

Stack<T> is a LIFO (last-in, first-out) collection optimized for reverse-order processing.

## Typical use cases
- Undo/redo operations
- Expression evaluation
- Backtracking algorithms

## Sample usage

See:
[samples/stack-processing.cs](../../samples/stack-processing.cs)

### How to run the sample

`ash
dotnet run samples/stack-processing.cs
`

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

`ash
cd benchmarks
dotnet run -c Release -- --filter *StackPushPopBenchmark*
`

### Benchmark code

[benchmarks/Iteration/StackPushPopBenchmark.cs](../../benchmarks/Iteration/StackPushPopBenchmark.cs)

### Results

(To be filled after running benchmark)

### Interpretation

(To be filled after running benchmark)

## Practical optimizations
- Use for LIFO scenarios only

## Common mistakes
- Using when order matters differently (FIFO)

## When I would choose it
- Reverse processing needed
- Last-in-first-out semantics required

## When I would avoid it
- Need FIFO → use Queue<T>
- Need lookup → use other collections

## Rule of thumb

Use Stack<T> when last-in-first-out behavior is required.
