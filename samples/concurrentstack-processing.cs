using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

// Concurrent LIFO processing using ConcurrentStack<T>
var stack = new ConcurrentStack<int>();

Parallel.For(0, 5, i =>
{
    stack.Push(i);
});

while (stack.TryPop(out var item))
{
    Console.WriteLine($"Processing {item}");
}
