using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

// Producer-consumer with blocking semantics
var collection = new BlockingCollection<int>(boundedCapacity: 5);

var producer = Task.Run(() =>
{
    for (int i = 0; i < 10; i++)
    {
        collection.Add(i);
        Console.WriteLine($"Produced {i}");
    }
    collection.CompleteAdding();
});

var consumer = Task.Run(() =>
{
    foreach (var item in collection.GetConsumingEnumerable())
    {
        Console.WriteLine($"Consumed {item}");
    }
});

Task.WaitAll(producer, consumer);
