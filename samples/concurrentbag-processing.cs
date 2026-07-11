using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

// Unordered concurrent processing using ConcurrentBag<T>
var bag = new ConcurrentBag<int>();

Parallel.For(0, 5, i =>
{
    bag.Add(i);
});

while (bag.TryTake(out var item))
{
    Console.WriteLine($"Processing {item}");
}
