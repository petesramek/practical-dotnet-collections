using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

// Setup a baseline dictionary of live system configuration parameters.
var liveConfigurationStore = new Dictionary<int, string> {
    [101] = "ENDPOINT_PRODUCTION_PRIMARY"
};

// USE CASE: Map Configuration Shielding Pattern.
// Initializing a ReadOnlyDictionary wraps the original reference collection.
// This enforces an absolute mutation-proof shield for public lanes, throwing 
// compiler errors if external components try to overwrite indexers or add keys.
var shieldedSettingsView = new ReadOnlyDictionary<int, string>(liveConfigurationStore);

string activeGateway = shieldedSettingsView[101];
Console.WriteLine($"Shielded Active Gateway: {activeGateway}");

// CRITICAL REALITY CHECK: Defensive Copy vs Reference Proxy.
// A ReadOnlyDictionary is NOT a static immutable snapshot. It is a live structural proxy view.
// If the internal engine mutates a single key inside the underlying dictionary, 
// that specific value change reflects through the proxy instantly.
liveConfigurationStore[101] = "ENDPOINT_PRODUCTION_FAILOVER";

// Querying the exact same key securely shows the value mutation is live
activeGateway = shieldedSettingsView[101];
Console.WriteLine($"Shielded Active Gateway (Post-Mutation): {activeGateway}");
