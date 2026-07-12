using System;
using System.Collections.Generic;

// Setup a baseline stream of raw incoming transaction codes.
string[] incomingMessagePayloads = new string[] { "MSG_PAYMENT_INIT", "MSG_FRAUD_CHECK", "MSG_LEDGER_POST" };

// USE CASE: Optimized FIFO Message Streaming.
// By passing the expected array size into the constructor, we preallocate the internal 
// circular buffer capacity immediately. This prevents the queue from executing hidden array 
// clones and memory-doubling routines as new data flows into the streaming pipeline.
var messageProcessingQueue = new Queue<string>(incomingMessagePayloads.Length);

foreach (string messageToken in incomingMessagePayloads) {
    // Appends cleanly to the tail of the circular array block
    messageProcessingQueue.Enqueue(messageToken);
}

// Drain the queue sequentially.
// TryPeek and TryDequeue provide atomic presence checking and element extraction, 
// completely removing the need for split 'Count > 0' conditional branching logic.
while (messageProcessingQueue.TryDequeue(out string? activeMessage)) {
    Console.WriteLine($"[FIFO Consumer] Successfully processed message signature: {activeMessage}");
}
