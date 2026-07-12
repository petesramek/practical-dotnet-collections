using System;
using System.Collections.Frozen;
using System.Collections.Generic;

// Setup a baseline dictionary with configuration settings.
var templateData = new Dictionary<string, string> {
    ["Environment"] = "Production",
    ["MaxConnections"] = "5000",
    ["EnableTelemetry"] = "True",
    ["TimeoutSeconds"] = "30"
};

// USE CASE: Fixed immutable configuration lookups.
// ToFrozenDictionary analyzes the specific string keys at initialization time. 
// It builds a highly optimized, specialized internal lookup tree that completely
// eliminates standard hash table collision checking loops for maximum read-only speed.
var applicationSettings = templateData.ToFrozenDictionary();

// Querying fields in high-frequency runtime loops.
string currentEnv = applicationSettings["Environment"];
bool supportsTelemetry = applicationSettings.ContainsKey("EnableTelemetry");

Console.WriteLine($"Environment Config: {currentEnv}");
Console.WriteLine($"Telemetry Status:   {supportsTelemetry}");
