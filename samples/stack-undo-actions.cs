using System;
using System.Collections.Generic;

// Setup a baseline stream of raw sequential operational events.
string[] userRuntimeActions = new string[] { "ACTION_DRAW_TEXT", "ACTION_APPLY_FILTER", "ACTION_RESIZE_IMAGE" };

// USE CASE: Optimized LIFO Transaction Rollbacks.
// By passing the expected array size into the constructor, we preallocate the internal 
// vector capacity immediately. This prevents the stack from executing hidden array clones 
// and memory-doubling routines on the managed heap as history events flow in.
var operationsHistoryStack = new Stack<string>(userRuntimeActions.Length);

foreach (string actionToken in userRuntimeActions) {
    // Appends cleanly to the active top slot of the underlying array buffer
    operationsHistoryStack.Push(actionToken);
}

// Drain the stack sequentially.
// TryPeek and TryPop provide atomic presence checking and element extraction, 
// completely removing the need for split 'Count > 0' conditional branching logic.
while (operationsHistoryStack.TryPop(out string? revertedAction)) {
    Console.WriteLine($"[LIFO Engine] Successfully rolled back structural state: {revertedAction}");
}
