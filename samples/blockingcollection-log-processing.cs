using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

// Initialize a thread-safe pipeline with a hard boundary limit of exactly 5 items.
// This cap protects server memory during unexpected downstream bottlenecks.
var loggingPipeline = new BlockingCollection<int>(boundedCapacity: 5);

// Fast Producer Track: Simulates a massive burst of incoming traffic logs.
var loggingProducer = Task.Run(() => {
    for (int i = 1; i <= 15; i++) {
        // This method blocks automatically when 5 unconsumed items accumulate in the pipe.
        loggingPipeline.Add(i);
        Console.WriteLine($"[Producer] Enqueued item {i} (Current Count: {loggingPipeline.Count})");
    }

    // Signals to consumers that no more data will be sent, allowing loops to exit safely.
    loggingPipeline.CompleteAdding();
});

// Bottlenecked Consumer Track: Simulates a slow background write to an external database.
var loggingConsumer = Task.Run(() => {
    // GetConsumingEnumerable blocks automatically when empty until new data arrives.
    foreach (var logId in loggingPipeline.GetConsumingEnumerable()) {
        Console.WriteLine($"[Consumer] Starting heavy database write for item {logId}...");

        // Minor inline block to guarantee the producer outpaces the worker thread.
        Thread.Sleep(200);

        Console.WriteLine($"[Consumer] Completed item {logId}");
    }
});

Task.WaitAll(loggingProducer, loggingConsumer);
