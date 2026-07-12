using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

// Setup a baseline list of critical domain tracking nodes.
var transactionalLogStore = new List<int> { 101, 102, 103 };

// USE CASE: Data-Shielding Wrapper Architecture.
// .AsReadOnly() creates a lightweight, reference-bound ReadOnlyCollection wrapper.
// This enforces an absolute mutation-proof shield for consumer lanes, throwing 
// compiler blocks if external callers attempt to invoke operations like '.Add()' or '.Remove()'.
ReadOnlyCollection<int> exposedShieldedView = transactionalLogStore.AsReadOnly();

// CRITICAL REALITY CHECK: Defensive Copy vs Reference Proxy.
// A ReadOnlyCollection is NOT an isolated snapshot. It is a live structural proxy view.
// If the internal engine modifies the underlying mutable list, changes propagate instantly.
transactionalLogStore.Add(104);

Console.WriteLine($"Underlying List mutated directly. Current Proxy Count: {exposedShieldedView.Count}");

// Enumerate the view using index-bound looping structures to bypass interface overhead.
for (int i = 0; i < exposedShieldedView.Count; i++) {
    Console.WriteLine($"Shielded Ledger Node Row [{i}]: {exposedShieldedView[i]}");
}
