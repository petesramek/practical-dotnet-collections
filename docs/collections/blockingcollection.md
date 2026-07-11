# BlockingCollection<T>

## What it is

BlockingCollection<T> is a thread-safe collection that adds blocking and bounding capabilities on top of concurrent collections.

## Typical use cases
- Producer-consumer pipelines with backpressure
- Bounded queues
- Controlled throughput systems

## Sample usage

See:
[samples/blockingcollection-processing.cs](../../samples/blockingcollection-processing.cs)

### How to run the sample

`ash
dotnet run samples/blockingcollection-processing.cs
`

## Internal implementation

Wraps an IProducerConsumerCollection<T> (default: ConcurrentQueue<T>) and adds blocking semantics.

### Lookup flow
- Uses Add / Take / GetConsumingEnumerable

## Memory characteristics
- Backed by underlying concurrent collection
- Optional bounded capacity limits memory usage

## Complexity overview

Add: O(1)  
Take: O(1)  
Blocking behavior depends on capacity

## Benchmark results

### Scenario

Compare blocking vs non-blocking queue:
- BlockingCollection<T>
- ConcurrentQueue<T>

### How to run this benchmark only

`ash
cd benchmarks
dotnet run -c Release -- --filter *BlockingCollectionBenchmark*
`

### Benchmark code

[benchmarks/Concurrency/BlockingCollectionBenchmark.cs](../../benchmarks/Concurrency/BlockingCollectionBenchmark.cs)

### Results

(To be filled after running benchmark)

### Interpretation

(To be filled after running benchmark)

## Practical optimizations
- Use bounded capacity to control memory

## Common mistakes
- Using when simple concurrent collections are enough

## When I would choose it
- Need blocking producer-consumer pipeline

## When I would avoid it
- Non-blocking scenarios → use ConcurrentQueue

## Rule of thumb

Use BlockingCollection<T> when you need backpressure and blocking semantics.
