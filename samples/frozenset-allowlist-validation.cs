using System;
using System.Collections.Frozen;
using System.Collections.Generic;

// Setup a baseline list of verified service identifier tokens.
var verifiedTokens = new List<string>
{
    "SERVICE_AUTH_GATEWAY",
    "SERVICE_INVENTORY_API",
    "SERVICE_BILLING_WORKER",
    "SERVICE_TELEMETRY_LOG"
};

// USE CASE: Fixed immutable allowlist verification checking.
// ToFrozenSet analyzes the string inputs at startup time to build an ultra-dense,
// specialized internal layout that strips away standard hash bucket bucket-chaining overhead,
// providing the fastest possible containment validation path for hot application loops.
var trustedServiceAllowlist = verifiedTokens.ToFrozenSet();

// Simulate checking an inbound request signature in a high-frequency filter track.
string inboundServiceId = "SERVICE_INVENTORY_API";
bool isRequestAuthorized = trustedServiceAllowlist.Contains(inboundServiceId);

Console.WriteLine($"Incoming Service ID: {inboundServiceId}");
Console.WriteLine($"Access Authorized:   {isRequestAuthorized}");
