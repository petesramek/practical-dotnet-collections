using System;
using System.Collections.Generic;

// Setup a raw dataset containing redundant information (e.g., duplicate log entries or user IDs).
int[] rawImportedUserIds = new int[] { 101, 102, 101, 103, 102, 104, 103, 105 };

// Initialize a unique hash pool.
var processedRecordsPool = new HashSet<int>();

foreach (int userId in rawImportedUserIds) {
    // USE CASE: Atomic check-and-insert uniqueness filtering.
    // Instead of executing an expensive double lookup (calling '.Contains()' followed by '.Add()'),
    // '.Add()' returns a boolean status directly: True if the element is unique and added successfully,
    // or False if the element already exists within the internal hash buckets.
    if (!processedRecordsPool.Add(userId)) {
        // Skip duplicate records instantly without triggering execution overhead
        Console.WriteLine($"[Duplicate Ignored] User ID {userId} has already been processed.");
        continue;
    }

    Console.WriteLine($"[Unique Processed] Handling data payload for User ID: {userId}");
}
