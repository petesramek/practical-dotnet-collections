using System;
using System.Collections.Generic;
using System.Collections.Immutable;

// Setup a baseline block of critical microservice endpoint routes.
var baseEndpoints = new Dictionary<string, string> {
    ["orders"] = "https://api.internal",
    ["shipping"] = "https://api.internal"
};

// Initialize an absolute read-only key-value segment snapshot.
// ImmutableDictionary<TKey, TValue> is an array-backed AVL binary tree structure.
var fallbackRoutingTable = baseEndpoints.ToImmutableDictionary();

// USE CASE: Safe structural expansion using an allocation-optimized Builder path.
// Instead of updating an immutable map entry-by-entry inside a loop (which copies and 
// re-balances the underlying internal tree structures on every single loop iteration), 
// the companion Builder is used to perform high-speed linear mutations completely allocation-free.
var stagingRoutingBuilder = fallbackRoutingTable.ToBuilder();

var dynamicRuntimeRegions = new Dictionary<string, string> {
    ["inventory-eu"] = "https://eu.internal",
    ["inventory-us"] = "https://us.internal"
};

foreach (var (routeKey, endpointUrl) in dynamicRuntimeRegions) {
    // Mutates the internal intermediate tree state cleanly without generating memory trash
    stagingRoutingBuilder.Add(routeKey, endpointUrl);
}

// Emits a frozen, thread-safe, and lock-free immutable dictionary branch block
var productionRoutingTable = stagingRoutingBuilder.ToImmutable();

Console.WriteLine($"Fallback Routing Rules Tracked: {fallbackRoutingTable.Count}");
Console.WriteLine($"Production Target Route Triggers: {productionRoutingTable.Count}");

// Any request thread can read these paths simultaneously with zero lock contention
string targetEndpoint = productionRoutingTable["inventory-eu"];
Console.WriteLine($"Direct Hash Routing Active Target: {targetEndpoint}");
