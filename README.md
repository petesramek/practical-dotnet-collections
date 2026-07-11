# Practical .NET Collections

A practical, performance-focused guide to choosing the right .NET collection based on real behavior, trade-offs, and measured performance.

---

## Quick Decision Matrix

| If you need... | Use |
|---------------|-----|
| Dynamic list of items | List<T> |
| Fast lookup by key | Dictionary<TKey, TValue> |
| Fast existence checks | HashSet<T> |
| FIFO processing | Queue<T> |
| LIFO processing | Stack<T> |
| Sorted data (dynamic) | SortedDictionary<TKey, TValue> |
| Sorted data (read-heavy) | SortedList<TKey, TValue> |
| Thread-safe dictionary | ConcurrentDictionary<TKey, TValue> |
| Async producer-consumer | Channel<T> |
| Immutable read-heavy data | ImmutableArray<T> |
| Immutable frequently updated data | ImmutableList<T> |
| Read-only, lookup-heavy data | FrozenDictionary<TKey, TValue> |
| Deduplicated static set | FrozenSet<T> |
| Priority-based processing | PriorityQueue<TElement, TPriority> |

---

## Decision Flowchart

`
Start
 │
 ├── Do you need ordering?
 │     ├── No → Dictionary / HashSet
 │     └── Yes
 │          ├── Always sorted → SortedDictionary / SortedList
 │          └── Only sorted at the end → Dictionary + OrderBy
 │
 ├── Do you need fast lookup?
 │     ├── Yes → Dictionary / HashSet
 │     └── No → List
 │
 ├── Do you need concurrency?
 │     ├── Yes
 │     │     ├── Async → Channel<T>
 │     │     └── Sync → ConcurrentDictionary
 │     └── No
 │
 ├── Is data immutable?
 │     ├── Yes
 │     │     ├── Read-heavy → ImmutableArray
 │     │     └── Write-heavy → ImmutableList
 │     └── No
 │
 ├── Is data static after creation?
 │     ├── Yes → FrozenDictionary / FrozenSet
 │     └── No
 │
 ├── Processing pattern?
 │     ├── FIFO → Queue
 │     ├── LIFO → Stack
 │     └── Priority → PriorityQueue
 │
 └── Default → List<T>
`

---

## Documentation

### Collections
- [Collections overview](docs/collections)

### Comparisons
- [All comparisons](docs/comparisons)

### Scenarios
- [Real-world scenarios](docs/scenarios)

### Benchmarks
- [Benchmark implementations](benchmarks)

### Advanced topics
- [Advanced topics](docs/advanced)

---

## Key Trade-offs

- List vs LinkedList → prefer List for cache locality
- Dictionary vs SortedDictionary → sorting vs lookup speed
- HashSet vs List → lookup vs iteration
- ImmutableArray vs ImmutableList → read vs write patterns
- Channel vs BlockingCollection → async vs blocking

---

## Common Mistakes

### Using List for lookups
`
list.Contains(x)
`
Use HashSet<T> instead.

### ImmutableArray in loops
`
array = array.Add(x);
`
Use a builder.

### Not setting List capacity
`
new List<T>()
`
Use 
ew List<T>(N) when size is known.

### Blocking in async code
Use Channel<T> instead of blocking primitives.

---

## Rule of Thumb

Start with List<T> or Dictionary<TKey, TValue>. Switch only when workload requires it.

---

## Goal

Provide a practical reference for choosing collections based on real behavior and measured performance.

---

## Summary

- Collections
- Comparisons
- Benchmarks
- Scenarios
- Advanced topics

