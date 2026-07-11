# Channel<T>

## What it is

Channel<T> is an asynchronous, thread-safe producer-consumer primitive designed for high-throughput pipelines.

## Typical use cases
- Async pipelines
- Background processing
- High-throughput messaging systems

## Sample usage

See:
[samples/channel-processing.cs](../../samples/channel-processing.cs)

### How to run the sample

`ash
dotnet run samples/channel-processing.cs
`

## Internal implementation

Uses lock-free techniques and async coordination between producers and consumers.

### Lookup flow
- No lookup
- Uses WriteAsync / ReadAllAsync

## Memory characteristics
- Backed by buffers (bounded/unbounded)
- Can apply backpressure

## Complexity overview

Write: O(1)  
Read: O(1)  
Async coordination overhead

## Benchmark results

### Scenario

Measure async producer-consumer throughput using Channel<T>.

### How to run this benchmark only

`ash
cd benchmarks
dotnet run -c Release -- --filter *ChannelBenchmark*
`

### Benchmark code

[benchmarks/Concurrency/ChannelBenchmark.cs](../../benchmarks/Concurrency/ChannelBenchmark.cs)

### Results

(To be filled after running benchmark)

### Interpretation

(To be filled after running benchmark)

## Practical optimizations
- Use bounded channels to control memory
- Prefer Channel over BlockingCollection for async workloads

## Common mistakes
- Using blocking collections in async code

## When I would choose it
- Async pipelines
- High-throughput messaging

## When I would avoid it
- Simple synchronous scenarios

## Rule of thumb

Use Channel<T> for async producer-consumer pipelines.
