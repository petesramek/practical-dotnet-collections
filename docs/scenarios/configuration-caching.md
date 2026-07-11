# Configuration Caching (FrozenDictionary)

## Scenario

Application loads configuration data once at startup and performs lookups on every request.

---

## Problem

Using Dictionary<TKey, TValue>:
- optimized for mixed workloads
- still carries mutation and resize logic
- not ideal for pure lookup scenarios

---

## Solution

Convert to FrozenDictionary after load:

`csharp
var config = LoadConfiguration();
var frozen = config.ToFrozenDictionary();

var value = frozen[key];
`

---

## Why it works

- FrozenDictionary removes mutation logic
- Precomputes optimal internal layout
- Optimized for repeated lookups

---

## When to use

- Configuration data
- Lookup tables
- Static datasets

---

## When NOT to use

- Data changes at runtime
- Frequent updates required

---

## Rule of thumb

If data is loaded once and read many times, use FrozenDictionary.
