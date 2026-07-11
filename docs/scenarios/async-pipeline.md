# Async Pipeline (Channel<T>)

## Scenario

Process incoming work (e.g., API requests, messages) and hand it off to background processing without blocking threads.

---

## Problem

Using blocking primitives in modern services (ASP.NET, workers):
- Threads wait for data (blocked)
- Thread pool can be exhausted under load
- Throughput drops and latency increases

---

## Solution

Use Channel<T> to build an async producer-consumer pipeline:

`csharp
using System.Threading.Channels;

var channel = Channel.CreateBounded<int>(new BoundedChannelOptions(100)
{
    FullMode = BoundedChannelFullMode.Wait
});

// Producer
_ = Task.Run(async () =>
{
    for (int i = 0; i < 1000; i++)
    {
        await channel.Writer.WriteAsync(i);
    }
    channel.Writer.Complete();
});

// Consumer
await foreach (var item in channel.Reader.ReadAllAsync())
{
    // process item
}
`

---

## Why it works

- WriteAsync / ReadAllAsync use async coordination
- Waiting does NOT block threads
- Threads return to pool while awaiting

---

## Backpressure

Use bounded channels to avoid memory growth:

`csharp
var channel = Channel.CreateBounded<int>(capacity: 1000);
`

- Producers wait when full
- System stabilizes under load

---

## When to use

- ASP.NET request pipelines
- Background workers / queues
- High-throughput ingestion

---

## When NOT to use

- Purely synchronous apps
- Very simple, low-throughput tasks

---

## The trap

Using BlockingCollection in async code:
- Blocks threads while waiting
- Can cause thread pool starvation

Channel<T> avoids this by yielding threads.

---

## Rule of thumb

For async systems, use Channel<T>. Prefer it over BlockingCollection<T> in modern .NET services.
