using System;
using System.Collections.Generic;

// Setup a baseline data cache using a standard hash map layout.
var userProfileCache = new Dictionary<int, string>();

// Define external variables to represent a backing system dependency.
var mockDatabase = new Dictionary<int, string> {
    [101] = "Alice",
    [102] = "Bob",
    [103] = "Charlie"
};
int databaseFetchCount = 0;

// Repeated mock requests executing against our localized gateway system.
int[] incomingRequestUserIds = new int[] { 101, 102, 101, 103, 102, 101 };

foreach (int userId in incomingRequestUserIds) {
    // USE CASE: High-speed single-pass lookup cache entry validation.
    // Instead of calling '.ContainsKey()' followed by the indexer accessor '_cache[id]',
    // '.TryGetValue()' validates presence and extracts the value in a single hash evaluation step.
    if (!userProfileCache.TryGetValue(userId, out string? cachedName)) {
        // Cache Miss: Simulate executing a heavy external data store query
        databaseFetchCount++;
        cachedName = mockDatabase[userId];

        // Store the extracted record back into our localized dictionary cache
        userProfileCache[userId] = cachedName;
    }

    Console.WriteLine($"Retrieved User: {cachedName}");
}

Console.WriteLine($"\nTotal Backing Store Interactions Saved: {incomingRequestUserIds.Length - databaseFetchCount}");
