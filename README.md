# Practical .NET Collections

A practical, performance-focused guide to choosing the right .NET collection based on real behavior, trade-offs, and measured performance.

---

## Quick Decision Matrix

| Requirement | Use |
|------------|-----|
| Dynamic list | List<T> |
| Key lookup | Dictionary<TKey, TValue> |
| Existence checks | HashSet<T> |
| FIFO processing | Queue<T> |
| LIFO processing | Stack<T> |
| Sorted key-value (dynamic) | SortedDictionary<TKey, TValue> |
| Sorted key-value (read-heavy) | SortedList<TKey, TValue> |
| Sorted values | SortedSet<T> |
| Thread-safe key-value | ConcurrentDictionary<TKey, TValue> |
| Async pipeline | Channel<T> |
| Blocking pipeline | BlockingCollection<T> |
| Immutable read-heavy | ImmutableArray<T> |
| Immutable structural updates | ImmutableList<T> |
| Static lookup | FrozenDictionary<TKey, TValue> |
| Static set | FrozenSet<T> |
| Priority processing | PriorityQueue<TElement, TPriority> |

---

## Decision Flow

`mermaid
flowchart TD
    A[Start] --> B{Thread-safe / Shared?}

    B -->|Yes| C{Async pipeline?}
    C -->|Yes| D[Channel]
    C -->|No| E{Blocking pipeline?}
    E -->|Yes| F[BlockingCollection]
    E -->|No| G[ConcurrentDictionary]

    B -->|No| H{Data lifetime}

    H -->|Immutable| I{Access pattern}
    I -->|Read-heavy| J[ImmutableArray]
    I -->|Structural updates| K[ImmutableList]

    H -->|Static / Read-only| L{Data type}
    L -->|Key-value| M[FrozenDictionary]
    L -->|Values only| N[FrozenSet]

    H -->|Dynamic| O{Data access}

    O -->|Key lookup| P{Existence only?}
    P -->|Yes| Q[HashSet]
    P -->|No| R{Always sorted?}
    R -->|Yes| S[SortedDictionary / SortedList]
    R -->|No| T[Dictionary]

    O -->|Sequential| U{Ordering}
    U -->|FIFO| V[Queue]
    U -->|LIFO| W[Stack]
    U -->|Priority| X[PriorityQueue]
    U -->|Sorted values| Y[SortedSet]
    U -->|Indexed/default| Z[List]
`

---

## Documentation

### Collections
- [List](./docs/collections/list.md)
- [Dictionary](./docs/collections/dictionary.md)
- [HashSet](./docs/collections/hashset.md)

### Comparisons
- [List vs LinkedList](./docs/comparisons/list-vs-linkedlist.md)
- [Dictionary vs SortedDictionary](./docs/comparisons/dictionary-vs-sorteddictionary.md)
- [HashSet vs List](./docs/comparisons/hashset-vs-list-lookup.md)
- [ImmutableArray vs ImmutableList](./docs/comparisons/immutablearray-vs-immutablelist.md)
- [List capacity](./docs/comparisons/list-default-vs-capacity.md)
- [Channel vs BlockingCollection](./docs/comparisons/channel-vs-blockingcollection.md)
- [SortedDictionary vs SortedList](./docs/comparisons/sorteddictionary-vs-sortedlist.md)
- [Dictionary vs FrozenDictionary](./docs/comparisons/dictionary-vs-frozendictionary.md)
- [HashSet vs FrozenSet](./docs/comparisons/hashset-vs-frozenset.md)

### Scenarios
- [Configuration caching](./docs/scenarios/configuration-caching.md)
- [Async pipeline](./docs/scenarios/async-pipeline.md)
- [Concurrent deduplication](./docs/scenarios/concurrent-deduplication.md)
- [Undo / Redo](./docs/scenarios/undo-redo.md)
- [Sliding window buffer](./docs/scenarios/sliding-window-buffer.md)
- [Priority processing](./docs/scenarios/priority-processing.md)

### Benchmarks
- [BitArray benchmark](./benchmarks/Memory/BitArrayBenchmark.cs)
- [List capacity benchmark](./benchmarks/Memory/ListCapacityBenchmark.cs)
- [Immutable builder benchmark](./benchmarks/Memory/ImmutableBuilderBenchmark.cs)
- [Failed lookup benchmark](./benchmarks/Lookup/FailedLookupBenchmark.cs)

### Advanced
- [Large Object Heap](./docs/advanced/large-object-heap.md)
- [Struct dictionary keys](./docs/advanced/struct-dictionary-keys.md)

---

## Common Mistakes

### Using List<T> for lookup

`csharp
list.Contains(x);
`

Use:

`csharp
var set = new HashSet<T>(list);
set.Contains(x);
`

---

### ImmutableArray<T> in loops

`csharp
array = array.Add(x);
`

Use:

`csharp
var builder = ImmutableArray.CreateBuilder<T>();
builder.Add(x);
`

---

### Missing capacity

`csharp
var list = new List<T>();
`

Use:

`csharp
var list = new List<T>(N);
`

---

### Struct keys without IEquatable<T>

Causes boxing and hidden allocations.

---

## Rule of Thumb

- Start with List<T> or Dictionary<TKey, TValue>
- Preallocate when size is known
- Use HashSet<T> for lookup-heavy workloads

---

## Summary

- Collections
- Comparisons
- Benchmarks
- Scenarios
- Advanced topics

