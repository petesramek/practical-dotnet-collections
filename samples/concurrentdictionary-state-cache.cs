using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

// Initialize a thread-safe map to act as a shared application state cache.
var inventoryCache = new ConcurrentDictionary<int, string>();

// Define external metadata values that will be used inside our multi-threaded initialization.
string regionCode = "EU";

// Simulate multiple incoming worker requests attempting to access or initialize cache keys simultaneously.
Parallel.For(1, 6, itemId => {
    // USE CASE: Stateful key insertion.
    // By passing 'regionCode' explicitly as the third parameter (the factory state), 
    // .NET can reuse the exact same compiler-generated static method handle. 
    // This removes the massive allocation bug where variables from an outer scope 
    // are captured, forcing the creation of a hidden heap closure block on every loop pass.
    string finalCachedValue = inventoryCache.GetOrAdd(
        itemId,
        (key, state) => $"item-{key}-region-{state}",
        regionCode
    );

    Console.WriteLine($"[Thread {Environment.CurrentManagedThreadId}] Retrieved: {finalCachedValue}");
});

// Enumerate the unified thread-safe state cache entries.
foreach (var (key, value) in inventoryCache) {
    Console.WriteLine($"Cache Entry: {key} -> {value}");
}
