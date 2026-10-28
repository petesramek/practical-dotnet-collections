# Channel<T> vs BlockingCollection<T>

## What this comparison answers

When should you use modern async pipelines vs traditional blocking producer-consumer collections?

---

## Key differences

| Aspect | BlockingCollection<T> | Channel<T> |
|-------|----------------------|------------|
| Model | Blocking (thread-based) | Async (task-based) |
| Waiting | Blocks OS thread | Yields thread (async) |
| Throughput | Comparable in the [benchmark](../related-types/channel.md) | Comparable in the [benchmark](../related-types/channel.md) |
| Backpressure | Supported | Supported (bounded channels) |
| Usage style | Synchronous | Async/await |

---

## Internal behavior

### BlockingCollection<T>
- Wraps a concurrent collection (usually ConcurrentQueue)
- Uses locks and OS synchronization primitives
- When empty/full, thread is blocked

### Channel<T>
- Built for async pipelines
- Uses async coordination (ValueTask, Task)
- Threads are released back to the pool while waiting

---

## Benchmark scenarios

### 1. Producer-consumer pipeline
- BlockingCollection blocks threads when waiting
- Channel allows threads to be reused

### 2. High concurrency
- BlockingCollection holds one thread per blocked consumer, which can exhaust the thread pool
- Channel releases the thread while waiting (not measured by the benchmark, which uses dedicated threads for `BlockingCollection`)

---

## Benchmark results

See [Channel<T>](../related-types/channel.md). Raw throughput was comparable; the clear difference was allocation under a slow consumer. `Channel<T>` is a related type rather than a collection.

---

## Practical guidance

### Use BlockingCollection<T> when
- You are in synchronous code
- Simpler threading model is acceptable

### Use Channel<T> when
- You build async systems
- You run in ASP.NET / services
- You want low allocation under a slow consumer (about 6 KB vs 8.6 MB at N = 50,000 in the benchmark)

---

## The trap

Using BlockingCollection in async environments:

- Threads get blocked waiting for data
- Thread pool gets exhausted under load
- Requests start timing out (ASP.NET scenario)

Better:

```csharp
var channel = Channel.CreateUnbounded<int>();

await channel.Writer.WriteAsync(1);

await foreach (var item in channel.Reader.ReadAllAsync())
{
    Console.WriteLine(item);
}
```

---

## Rule of thumb

Use Channel<T> for modern async pipelines. Use BlockingCollection<T> only in legacy or purely synchronous scenarios.
