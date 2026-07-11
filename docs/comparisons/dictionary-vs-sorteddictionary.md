# Dictionary<TKey, TValue> vs SortedDictionary<TKey, TValue>

## What this comparison answers

When should you use a hash-based dictionary vs a tree-based dictionary that keeps keys ordered?

---

## Key differences

| Aspect | Dictionary<TKey, TValue> | SortedDictionary<TKey, TValue> |
|-------|--------------------------|--------------------------------|
| Storage | Hash table (buckets) | Balanced tree (nodes) |
| Ordering | None | Always sorted by key |
| Memory | Compact | Higher (node per item) |
| Lookup | Very fast | Slightly slower |
| Insert | Very fast | Consistent but slower |
| Iteration | Unordered | Sorted order |

---

## Internal behavior

### Dictionary<TKey, TValue>
- Uses hashing to place items into buckets
- May resize and rehash as it grows
- No guarantees on iteration order

### SortedDictionary<TKey, TValue>
- Uses a balanced tree (red-black tree)
- Every insert finds the correct position in order
- Maintains sorted order at all times

---

## Benchmark scenarios

### 1. Lookup
- Dictionary: direct bucket lookup
- SortedDictionary: tree traversal

### 2. Insert
- Dictionary: append into bucket
- SortedDictionary: walk tree and rebalance

### 3. Iteration
- Dictionary: fast but unordered
- SortedDictionary: ordered but requires traversal

---

## Practical guidance

### Use Dictionary<TKey, TValue> when
- You need fastest possible lookups
- Order does not matter
- You build data dynamically

### Use SortedDictionary<TKey, TValue> when
- You need keys always sorted
- You iterate in order frequently
- You perform range-based operations

---

## The trap

Using SortedDictionary just to get sorted output.

- Every .Add() pays the cost of keeping the structure ordered
- If you only need sorted output at the end:

`csharp
var dict = new Dictionary<int, string>();
// populate

foreach (var key in dict.Keys.OrderBy(x => x))
{
    Console.WriteLine(dict[key]);
}
`

This is often faster than maintaining order during every insert.

---

## Rule of thumb

Start with Dictionary<TKey, TValue>. Only use SortedDictionary<TKey, TValue> if you need continuous sorted access, not just final output ordering.
