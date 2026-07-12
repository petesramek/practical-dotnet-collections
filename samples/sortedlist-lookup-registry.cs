using System;
using System.Collections.Generic;

// Initialize a compact, array-backed sorted map.
// SortedList maintains internal elements inside two separate flat arrays (Keys and Values),
// keeping them continuously sorted using binary search insertion placement.
var localizationLookupRegistry = new SortedList<int, string>();

// USE CASE: Fixed metadata tracking pool initialized at application startup.
// Elements are registered and immediately sorted into place within internal contiguous arrays.
localizationLookupRegistry.Add(303, "ERR_RESOURCE_NOT_FOUND");
localizationLookupRegistry.Add(101, "STATUS_SUCCESS_OK");
localizationLookupRegistry.Add(202, "ERR_UNAUTHORIZED_ACCESS");

// Enumerate the sorted mapping directly via contiguous memory strides.
// Because it iterates raw arrays rather than bouncing across separate heap object pointers, 
// SortedList provides maximum CPU L1/L2 cache locality for sequential scans.
foreach (var (statusCode, statusDescription) in localizationLookupRegistry) {
    Console.WriteLine($"[Contiguous Registry Code {statusCode}]: {statusDescription}");
}
