using System;
using System.Collections.Generic;

// Simulated data source (e.g. database)
var database = new Dictionary<int, string> {
    [1] = "Alice",
    [2] = "Bob",
    [3] = "Charlie"
};

// Track how many times we hit the data source
var fetchCount = 0;

string FetchUserName(int userId) {
    fetchCount++;
    return database[userId];
}

// Cache
var cache = new Dictionary<int, string>();

string GetUserName(int userId) {
    if (cache.TryGetValue(userId, out var value))
        return value;

    value = FetchUserName(userId);
    cache[userId] = value;

    return value;
}

// Repeated lookup pattern
var ids = new[] { 1, 2, 1, 3, 2, 1 };

foreach (var id in ids) {
    Console.WriteLine(GetUserName(id));
}

Console.WriteLine();
Console.WriteLine($"FetchUserName called: {fetchCount} times");
