using System;
using System.Threading.Channels;
using System.Threading.Tasks;

// Async producer-consumer using Channel<T>
var channel = Channel.CreateUnbounded<int>();

var writer = channel.Writer;
var reader = channel.Reader;

var producer = Task.Run(async () =>
{
    for (int i = 0; i < 5; i++)
    {
        await writer.WriteAsync(i);
        Console.WriteLine($"Produced {i}");
    }
    writer.Complete();
});

var consumer = Task.Run(async () =>
{
    await foreach (var item in reader.ReadAllAsync())
    {
        Console.WriteLine($"Consumed {item}");
    }
});

await Task.WhenAll(producer, consumer);
