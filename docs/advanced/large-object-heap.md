# Large Object Heap (LOH)

## What it is

Objects larger than ~85,000 bytes are allocated on the Large Object Heap.

---

## Why it matters

- LOH is NOT compacted by default
- Large allocations stay in memory
- Can lead to fragmentation

---

## Example

`csharp
var list = new List<int>(100000);
`

This may push the internal array to LOH.

---

## Impact

- memory fragmentation
- higher GC pauses
- unpredictable memory usage

---

## Rule of thumb

Avoid repeated large allocations. Reuse buffers or control capacity.
