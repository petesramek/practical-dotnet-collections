using System;
using System.Collections.Generic;

// Setup a mock stream of processing transaction parameters.
int[] incomingTransactionStream = new int[] { 10, 25, 45, 90, 120 };

// USE CASE: Pre-sizing buffer capacity to eliminate dynamic array reallocations.
// Instead of initializing a blank list, passing the expected size upfront allocates 
// a single contiguous block of memory. This prevents the runtime from executing hidden 
// array copies and memory-doubling routines on the managed heap as the collection scales.
var optimizedBatchBuffer = new List<int>(incomingTransactionStream.Length);

foreach (int transactionAmount in incomingTransactionStream) {
    // Appends cleanly to the pre-allocated array tail with absolute maximum speed
    optimizedBatchBuffer.Add(transactionAmount);
}

// Enumerate the buffered values sequentially via index memory strides
foreach (int transactionAmount in optimizedBatchBuffer) {
    Console.WriteLine($"Buffered Batch Transaction: ${transactionAmount}");
}
