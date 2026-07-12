using System;
using System.Collections.Generic;

// Initialize a min-heap based scheduler tracking items paired with explicit numerical weights.
// PriorityQueue guarantees that the element containing the lowest numerical priority index value 
// will always bubble to the absolute top of the processing column.
var backgroundJobTriagePool = new PriorityQueue<string, int>();

// USE CASE: Live incoming event stream processing.
// Tasks arrive in arbitrary order, but must be triaged strictly by critical system weight.
backgroundJobTriagePool.Enqueue("Minor Internal Telemetry Sync", 3);
backgroundJobTriagePool.Enqueue("Critical Database Outage Failsafe", 1);
backgroundJobTriagePool.Enqueue("Standard Customer Email Dispatch", 2);

// Extract items sequentially.
// Under the hood, PriorityQueue maps these rows without sorting the full array on every addition, 
// using binary heap node traversals to pop the highest priority task instantly.
while (backgroundJobTriagePool.TryDequeue(out string? activeJobDescription, out int priorityWeight)) {
    Console.WriteLine($"[Executing Triage Code Weight: {priorityWeight}] Handling task: {activeJobDescription}");
}
