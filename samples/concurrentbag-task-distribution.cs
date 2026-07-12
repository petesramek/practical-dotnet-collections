using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

// Initialize an unordered thread-safe container.
// ConcurrentBag creates an independent tracking queue for each individual thread ID.
var localizedWorkBag = new ConcurrentBag<int>();

// Define a highly localized thread workload where each thread acts as its own producer and consumer.
Parallel.For(0, 4, threadId => {
    // Part 1: Each worker thread populates its own local memory queue
    for (int i = 1; i <= 3; i++) {
        int uniqueTaskId = (threadId * 10) + i;
        localizedWorkBag.Add(uniqueTaskId);
        Console.WriteLine($"[Thread {Environment.CurrentManagedThreadId}] Generated Task {uniqueTaskId}");
    }

    // Part 2: Each worker thread immediately drains its own local queue.
    // This is the optimal path: it runs entirely lock-free without stealing from other threads.
    while (localizedWorkBag.TryTake(out var localTaskId)) {
        Console.WriteLine($"[Thread {Environment.CurrentManagedThreadId}] Completed Local Task {localTaskId}");
    }
});
