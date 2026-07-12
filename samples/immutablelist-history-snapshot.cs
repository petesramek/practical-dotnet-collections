using System;
using System.Collections.Generic;
using System.Collections.Immutable;

// Setup a baseline list of critical financial transaction ledger logs.
var baseTransactions = new List<string>
{
    "TXN_INIT_1001",
    "TXN_AUTH_1001"
};

// Initialize an absolute read-only history block snapshot.
// ImmutableList<T> is structured internally as an AVL binary tree.
var initialLedgerHistory = baseTransactions.ToImmutableList();

// USE CASE: Safe structural expansion using an allocation-optimized Builder path.
// Instead of updating an immutable list entry-by-entry inside a loop (which copies and 
// re-balances the underlying internal tree structures on every single loop iteration), 
// the companion Builder is used to perform high-speed linear mutations completely allocation-free.
var stagingLedgerBuilder = initialLedgerHistory.ToBuilder();

var localizedStreamingLogs = new List<string>
{
    "TXN_SETTLE_1001",
    "TXN_CLOSE_1001",
    "TXN_ARCHIVE_1001"
};

foreach (string operationalLog in localizedStreamingLogs) {
    // Mutates the internal intermediate tree state cleanly without generating memory trash
    stagingLedgerBuilder.Add(operationalLog);
}

// Emits a frozen, thread-safe, and lock-free immutable sequence branch block
var finalizedLedgerHistory = stagingLedgerBuilder.ToImmutable();

Console.WriteLine($"Initial Ledger History Entries: {initialLedgerHistory.Count}");
Console.WriteLine($"Finalized Ledger History Entries: {finalizedLedgerHistory.Count}");

// Any auditing worker thread can index elements safely with structural node isolation
string specificLog = finalizedLedgerHistory[3]; // Traverses internal tree nodes to resolve the position
Console.WriteLine($"Audit Query - Target Row 4 Status: {specificLog}");
