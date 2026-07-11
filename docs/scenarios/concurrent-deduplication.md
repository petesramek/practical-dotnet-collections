# Concurrent Deduplication (ConcurrentDictionary)

## Scenario

Multiple threads/processes ingest data concurrently and must ensure each item is processed only once.

Example:
- processing messages
- importing records
- handling events from multiple sources

---

## Problem

Using HashSet<T> in concurrent scenarios:
- not thread-safe
- requires locking
- can cause contention and bugs

---

## Solution

Use ConcurrentDictionary<TKey, byte> as a thread-safe set:

`csharp
using System.Collections.Concurrent;

var processed = new ConcurrentDictionary<int, byte>();

bool TryProcess(int id)
{
    return processed.TryAdd(id, 0);
}

// usage
if (TryProcess(item.Id))
{
    // process only once
}
`

---

## Why it works

- ConcurrentDictionary is thread-safe
- TryAdd is atomic
- No external locking required

---

## Why not ConcurrentBag

`csharp
var bag = new ConcurrentBag<int>();
`

- allows duplicates
- no lookup guarantees
- cannot enforce uniqueness

---

## Memory note

- value type is minimized (byte)
- key holds the actual identity

---

## When to use

- parallel ingestion pipelines
- deduplication across threads
- idempotent processing

---

## When NOT to use

- single-threaded scenarios → use HashSet<T>
- no uniqueness requirement

---

## The trap

Assuming a concurrent set exists in .NET:

- there is no ConcurrentHashSet
- using non-thread-safe collections causes race conditions

---

## Rule of thumb

For concurrent deduplication, use ConcurrentDictionary<TKey, TValue> as a set.
