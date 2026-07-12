using System;
using System.Collections.Generic;

// Setup an inbound buffer array of historical data entries.
int[] historicalLogBatch = new int[] { 1, 2, 3, 4, 5 };

// Initialize a doubly-linked node list sequence.
var realTimeActivityStream = new LinkedList<int>();

foreach (int logEntryId in historicalLogBatch) {
    // USE CASE: High-speed constant-time head insertion.
    // Instead of forcing an array to copy and shift all of its entries down one slot
    // in memory, '.AddFirst()' creates a standalone node wrapper, snaps its forwards and backwards
    // pointers onto the current head, and updates the list boundary instantly.
    realTimeActivityStream.AddFirst(logEntryId);
}

// Enumerate the structural sequence from head to tail.
// Because we used 'AddFirst', the sequence reads in reverse chronological order.
foreach (int logEntryId in realTimeActivityStream) {
    Console.WriteLine($"Active Log Position: {logEntryId}");
}
