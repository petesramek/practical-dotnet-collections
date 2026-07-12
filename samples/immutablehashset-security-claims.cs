using System;
using System.Collections.Generic;
using System.Collections.Immutable;

// Setup a baseline list of authenticated security role claims.
var baseClaims = new List<string>
{
    "ROLE_USER",
    "CLAIM_READ_PROFILE"
};

// Initialize an absolute read-only uniqueness pool snapshot.
// ImmutableHashSet<T> is structured internally as an array-backed AVL binary tree.
var defaultUserClaims = baseClaims.ToImmutableHashSet();

// USE CASE: Safe structural expansion using an allocation-optimized Builder path.
// Instead of updating an immutable set entry-by-entry inside a loop (which copies and 
// re-balances the underlying internal tree structures on every single loop iteration), 
// the companion Builder is used to perform high-speed linear mutations completely allocation-free.
var stagingClaimsBuilder = defaultUserClaims.ToBuilder();

var localizedElevatedClaims = new List<string>
{
    "ROLE_MANAGER",
    "CLAIM_WRITE_INVENTORY",
    "CLAIM_DELETE_RECORD"
};

foreach (string elevatedClaim in localizedElevatedClaims) {
    // Mutates the internal intermediate tree state cleanly without generating memory trash
    stagingClaimsBuilder.Add(elevatedClaim);
}

// Emits a frozen, thread-safe, and lock-free immutable hash set block
var managerClaimsToken = stagingClaimsBuilder.ToImmutable();

Console.WriteLine($"Default Token Security Claims: {defaultUserClaims.Count}");
Console.WriteLine($"Elevated Manager Security Claims: {managerClaimsToken.Count}");

// Any request execution track can validate tokens simultaneously with zero lock contention
bool hasDeleteAccess = managerClaimsToken.Contains("CLAIM_DELETE_RECORD");
Console.WriteLine($"Security Check - Has Delete Access: {hasDeleteAccess}");
