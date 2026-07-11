# Practical .NET Collections

A practical, performance-focused guide to choosing the right .NET collection.

---

## Quick Decision Matrix

| Requirement | Use |
|------------|-----|
| Dynamic list | List<T> |
| Key lookup | Dictionary<TKey, TValue> |
| Existence checks | HashSet<T> |
| FIFO | Queue<T> |
| LIFO | Stack<T> |
| Sorted key-value | SortedDictionary<TKey, TValue> |
| Sorted values | SortedSet<T> |
| Thread-safe key-value | ConcurrentDictionary<TKey, TValue> |
| Async pipeline | Channel<T> |
| Blocking pipeline | BlockingCollection<T> |
| Immutable read-heavy | ImmutableArray<T> |
| Immutable structural updates | ImmutableList<T> |
| Static lookup | FrozenDictionary<TKey, TValue> |
| Static set | FrozenSet<T> |
| Priority | PriorityQueue<TElement, TPriority> |

---

## Decision Flow

`mermaid
flowchart TD
    A[Start] --> B{Thread-safe?}

    B -->|Yes| C{Async?}
    C -->|Yes| D[Channel]
    C -->|No| E{Blocking?}
    E -->|Yes| F[BlockingCollection]
    E -->|No| G[ConcurrentDictionary]

    B -->|No| H{Lifetime}

    H -->|Immutable| I{Pattern}
    I -->|Read| J[ImmutableArray]
    I -->|Updates| K[ImmutableList]

    H -->|Static| L{Type}
    L -->|Key-Value| M[FrozenDictionary]
    L -->|Values| N[FrozenSet]

    H -->|Dynamic| O{Access}

    O -->|Lookup| P{Existence only}
    P -->|Yes| Q[HashSet]
    P -->|No| R{Sorted}
    R -->|Yes| S[SortedDictionary / SortedList]
    R -->|No| T[Dictionary]

    O -->|Sequence| U{Order}
    U -->|FIFO| V[Queue]
    U -->|LIFO| W[Stack]
    U -->|Priority| X[PriorityQueue]
    U -->|Sorted| Y[SortedSet]
    U -->|Default| Z[List]
`

---

## Documentation

### Collections
- [List](docs/collections/list.md)
- [Dictionary](docs/collections/dictionary.md)
- [HashSet](docs/collections/hashset.md)

### Comparisons
- [List vs LinkedList](docs/comparisons/list-vs-linkedlist.md)
- [Dictionary vs SortedDictionary](docs/comparisons/dictionary-vs-sorteddictionary.md)
- [HashSet vs List](docs/comparisons/hashset-vs-list-lookup.md)
- [ImmutableArray vs ImmutableList](docs/comparisons/immutablearray-vs-immutablelist.md)
- [List Capacity](docs/comparisons/list-default-vs-capacity.md)
- [Channel vs BlockingCollection](docs/comparisons/channel-vs-blockingcollection.md)
- [SortedDictionary vs SortedList](docs/comparisons/sorteddictionary-vs-sortedlist.md)
- [Dictionary vs FrozenDictionary](docs/comparisons/dictionary-vs-frozendictionary.md)
- [HashSet vs FrozenSet](docs/comparisons/hashset-vs-frozenset.md)

### Scenarios
- [Configuration caching](docs/scenarios/configuration-caching.md)
- [Async pipeline](docs/scenarios/async-pipeline.md)
- [Concurrent deduplication](docs/scenarios/concurrent-deduplication.md)
- [Undo/redo](docs/scenarios/undo-redo.md)
- [Sliding window](docs/scenarios/sliding-window-buffer.md)
- [Priority processing](docs/scenarios/priority-processing.md)

### Advanced
- [Large Object Heap](docs/advanced/large-object-heap.md)
- [Struct dictionary keys](docs/advanced/struct-dictionary-keys.md)

---

## Common Mistakes

### Lookup on list

`csharp
list.Contains(x);
`

Use:

`csharp
var set = new HashSet<T>(list);
set.Contains(x);
`

---

### ImmutableArray in loops

`csharp
array = array.Add(x);
`

Use:

`csharp
var builder = ImmutableArray.CreateBuilder<T>();
`

---

### Missing capacity

`csharp
new List<T>()
`

Use:

`csharp
new List<T>(N)
`

---

### Struct keys

Implement IEquatable<T> to avoid boxing.

---

## Rule of Thumb

- Start with List<T> or Dictionary<TKey, TValue>
- Use HashSet<T> for lookups
- Preallocate when possible

