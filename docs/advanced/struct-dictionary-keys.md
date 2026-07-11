# Struct Keys in Dictionary (Boxing)

## What it is

Using struct keys without proper equality implementation can cause boxing.

---

## Problem

`csharp
struct MyKey { public int Id; }
var dict = new Dictionary<MyKey, string>();
`

Without IEquatable<MyKey>, lookups may box the struct.

---

## Impact

- hidden allocations
- GC pressure
- degraded performance

---

## Solution

Implement IEquatable<T>:

`csharp
struct MyKey : IEquatable<MyKey>
{
    public int Id;

    public bool Equals(MyKey other) => Id == other.Id;
    public override int GetHashCode() => Id;
}
`

---

## Rule of thumb

Always implement IEquatable<T> for struct keys in dictionaries.
