# SortedDictionary<TKey, TValue> vs SortedList<TKey, TValue>

## What this comparison answers

When should you use a tree-based sorted map vs a compact array-based sorted map?

---

## Key differences

| Aspect | SortedDictionary<TKey, TValue> | SortedList<TKey, TValue> |
|-------|--------------------------------|--------------------------|
| Storage | Node-based balanced tree | Two contiguous arrays (keys/values) |
| Memory | Higher (object per node) | Lower (packed arrays) |
| Insert pattern | Stable for random inserts | Expensive for mid inserts (shifts) |
| Lookup | Consistent tree traversal | Binary search over array |
| Iteration | In-order traversal | Sequential array scan (cache-friendly) |

---

## Internal behavior

### SortedDictionary<TKey, TValue>
- Red-black tree
- Each element is a node with references
- Insert finds position and may rebalance tree

### SortedList<TKey, TValue>
- Backed by two arrays: keys[] and values[]
- Keeps keys sorted at all times
- Insert requires shifting elements to open a slot

---

## Benchmark scenarios

### 1. Random inserts
- SortedDictionary: inserts without shifting large blocks
- SortedList: frequent array shifts (Array.Copy)

### 2. Bulk load (known order)
- SortedList: excellent if data is appended in sorted order
- SortedDictionary: consistent but more allocations

### 3. Iteration
- SortedList: very fast due to contiguous memory
- SortedDictionary: pointer traversal

---

## Practical guidance

### Use SortedDictionary<TKey, TValue> when
- You insert/remove frequently
- Keys arrive in random order
- You need stable update performance

### Use SortedList<TKey, TValue> when
- Data is loaded once (or rarely changes)
- You care about memory footprint
- You iterate frequently over sorted data

---

## The trap

Using SortedList for dynamic workloads:

`csharp
var list = new SortedList<int, string>();

foreach (var item in incoming)
{
    list.Add(item.Key, item.Value); // shifts arrays frequently
}
`

This triggers repeated large memory moves.

Better:

`csharp
var dict = new SortedDictionary<int, string>();

foreach (var item in incoming)
{
    dict.Add(item.Key, item.Value);
}
`

Or, if data is known upfront and sortable:

`csharp
var items = incoming.OrderBy(x => x.Key).ToList();
var list = new SortedList<int, string>(items.Count);
foreach (var i in items)
    list.Add(i.Key, i.Value);
`

---

## Rule of thumb

- Dynamic inserts → SortedDictionary<TKey, TValue>
- Static, read-heavy data → SortedList<TKey, TValue>
