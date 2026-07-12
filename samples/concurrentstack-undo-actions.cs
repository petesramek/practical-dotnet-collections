using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

// Initialize an unconstrained, thread-safe last-in, first-out (LIFO) stack collection.
// ConcurrentStack uses a lock-free singly-linked list structure to manage elements.
var undoActionStack = new ConcurrentStack<int>();

// Define a batch of structural history logs to apply simultaneously.
int[] historicalActionBatch = new int[] { 101, 102, 103, 104, 105 };

// USE CASE: Bulk pointer range manipulation.
// Instead of calling '.Push()' in a loop and forcing multiple atomic thread updates,
// 'PushRange' links the entire array segment onto the stack head pointer in a single step.
undoActionStack.PushRange(historicalActionBatch);
Console.WriteLine($"Pushed initial historical block batch. Current count: {undoActionStack.Count}");

// Simulate multiple concurrent operations adding runtime modifications simultaneously.
Parallel.For(1, 4, actionId => {
    undoActionStack.Push(actionId);
    Console.WriteLine($"[Thread {Environment.CurrentManagedThreadId}] Pushed transactional undo state: {actionId}");
});

// Pop data atomically in bulk using a shared buffer track.
// TryPopRange reduces head pointer thread contention to a singular atomic clock operation.
var popBuffer = new int[5];
int poppedCount = undoActionStack.TryPopRange(popBuffer);

Console.WriteLine($"Atomically extracted {poppedCount} items from the head of the stack:");
for (int i = 0; i < poppedCount; i++) {
    Console.WriteLine($"-> Rolled back Action ID: {popBuffer[i]}");
}
