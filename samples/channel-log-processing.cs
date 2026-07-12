using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

// Initialize an asynchronous pipeline with a hard boundary limit of exactly 5 items.
// We pass configuration options to explicitly optimize for single or multi-worker threads.
var channelOptions = new BoundedChannelOptions(5) {
    FullMode = BoundedChannelFullMode.Wait,
    SingleWriter = true,
    SingleReader = true
};
var loggingChannel = Channel.CreateBounded<int>(channelOptions);

var writer = loggingChannel.Writer;
var reader = loggingChannel.Reader;

// Fast Producer Track: Simulates an asynchronous background process generating traffic logs.
var loggingProducer = Task.Run(async () => {
    for (int i = 1; i <= 15; i++) {
        // WriteAsync yields control back to the orchestrator if the 5-item bound is reached,
        // pausing the producer task asynchronously without tying up an operating system thread.
        await writer.WriteAsync(i);
        Console.WriteLine($"[Producer] Enqueued item {i}");
    }

    // Signals to consumers that the production cycle has completed so reading streams can exit.
    writer.Complete();
});

// Bottlenecked Consumer Track: Simulates a slow background write to an external database.
var loggingConsumer = Task.Run(async () => {
    // ReadAllAsync yields cleanly while waiting for new data, maximizing thread reuse.
    await foreach (var logId in reader.ReadAllAsync()) {
        Console.WriteLine($"[Consumer] Starting heavy database write for item {logId}...");

        // Simulates an asynchronous I/O delay without blocking the executing worker thread.
        await Task.Delay(200);

        Console.WriteLine($"[Consumer] Completed item {logId}");
    }
});

await Task.WhenAll(loggingProducer, loggingConsumer);
