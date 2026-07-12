using System;
using System.Collections.Generic;

// Initialize a self-balancing binary search map.
// SortedDictionary keeps its entries continuously sorted by key using an internal Red-Black tree.
var priorityLogRegistry = new SortedDictionary<int, string>();

// USE CASE: Live unordered event stream aggregation.
// Records arrive randomly out of sequence but must automatically layout in proper numerical alignment.
priorityLogRegistry[303] = "Transaction Batch Settle";
priorityLogRegistry[101] = "Critical Systems Auth Failure";
priorityLogRegistry[202] = "Database Connection Retry Alert";

// Enumerate the sorted mapping directly.
// The iteration path navigates the internal binary tree nodes from left to right, 
// guaranteeing that keys emerge in ascending, predictable order.
foreach (var (logCode, descriptiveText) in priorityLogRegistry) {
    Console.WriteLine($"[System Status Code {logCode}]: {descriptiveText}");
}
