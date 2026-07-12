using System;
using System.Collections.Generic;

// Setup a raw incoming telemetry stream containing out-of-order data and duplicates.
int[] incomingSensorReadings = new int[] { 505, 101, 303, 202, 505, 404, 101 };

// Initialize a self-balancing binary search set.
// SortedSet automatically strips duplicates and keeps its elements sorted by value using a Red-Black tree.
var analyticalMetricsRegistry = new SortedSet<int>(incomingSensorReadings);

// USE CASE: Live deduplication and sorted range streaming.
// Enumerate the sorted mapping directly.
// The iteration path navigates the internal binary tree nodes from left to right, 
// guaranteeing that unique keys emerge in ascending, predictable order.
foreach (int uniqueSensorCode in analyticalMetricsRegistry) {
    Console.WriteLine($"[Unique Aggregated Metric ID]: {uniqueSensorCode}");
}
