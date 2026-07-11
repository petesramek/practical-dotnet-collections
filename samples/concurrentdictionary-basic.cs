using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

var dict = new ConcurrentDictionary<int, string>();

Parallel.For(0, 5, i =>
{
    dict[i] = $"value-{i}";
});

foreach (var item in dict)
{
    Console.WriteLine($"{item.Key} -> {item.Value}");
}
