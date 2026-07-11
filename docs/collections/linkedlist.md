# LinkedList<T>

## What it is

LinkedList<T> is a node-based collection optimized for fast insertions and removals.

## Typical use cases
- Frequent inserts/removes at beginning or middle
- When shifting elements is too expensive

## Sample usage

See:
[samples/linkedlist-prepend.cs](../../samples/linkedlist-prepend.cs)

### How to run the sample

`ash
dotnet run samples/linkedlist-prepend.cs
`

## Internal implementation

Doubly-linked list of nodes.

### Lookup flow
- Sequential traversal required

## Memory characteristics
- Each element is a separate node
- Higher memory overhead
- Poor cache locality

## Complexity overview

Lookup: O(n)  
AddFirst: O(1)  
Insert: O(1) if node known  
Remove: O(1) if node known

## Benchmark results

### Scenario

Compare insert at beginning:
- List<T>.Insert(0, item)
- LinkedList<T>.AddFirst

### How to run this benchmark only

`ash
cd benchmarks
dotnet run -c Release -- --filter *LinkedListAddInsertBenchmark*
`

### Benchmark code

[benchmarks/Add/LinkedListAddInsertBenchmark.cs](../../benchmarks/Add/LinkedListAddInsertBenchmark.cs)

### Results

(To be filled after running benchmark)

### Interpretation

(To be filled after running benchmark)

## Practical optimizations
- Use when insert/remove dominates

## Common mistakes
- Using for lookup-heavy workloads

## When I would choose it
- Frequent insert/remove operations

## When I would avoid it
- Frequent lookups
- Memory-sensitive scenarios

## Rule of thumb

Use LinkedList<T> when insertion cost matters more than lookup performance.
