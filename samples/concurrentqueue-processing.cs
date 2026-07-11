using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

// Producer-consumer using ConcurrentQueue<T>
var queue = new ConcurrentQueue<int>();

Parallel.For(0, 5, i =>
{
    queue.Enqueue(i);
});

while (queue.TryDequeue(out var item))
{
    Console.WriteLine($"Processing {item}");
}
