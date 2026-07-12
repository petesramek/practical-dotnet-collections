using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

// Initialize an unconstrained, thread-safe first-in, first-out (FIFO) collection.
// ConcurrentQueue splits internal storage into linked segmented arrays to minimize thread conflicts.
var logStreamingQueue = new ConcurrentQueue<int>();

// Simulate multiple concurrent web request threads generating traffic logs simultaneously.
Parallel.For(1, 6, logId => {
    logStreamingQueue.Enqueue(logId);
    Console.WriteLine($"[Thread {Environment.CurrentManagedThreadId}] Enqueued log trace: {logId}");
});

// Drain the queue concurrently or sequentially.
// TryDequeue replaces the separate 'Count > 0' check and 'Dequeue()' extraction 
// with a single atomic operation, preventing race conditions where elements disappear.
while (logStreamingQueue.TryDequeue(out var extractedLogId)) {
    Console.WriteLine($"Processing log payload: {extractedLogId}");
}
