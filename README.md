# Practical .NET Collections

A practical, performance-focused guide to choosing the right .NET collection based on real behavior, trade-offs, and measured performance.

---

## How to use this guide

- **Step 1:** Use the **Decision Flow Chart** below to narrow down your architectural constraints.
- **Step 2:** Use the **Quick Reference Matrix** to confirm the exact data structure choice.
- **Step 3:** Click the collection names in the matrix or the catalog section to navigate directly to detailed behavior, trade-offs, and benchmarks.

---

## Decision Flow

Decisions are evaluated sequentially in this exact order:
1. **Thread Safety** (Is data shared across multiple threads concurrently?)
2. **Data Lifetime** (Is data dynamic/mutable, completely static, immutable, or a safe view?)
3. **Access Pattern** (Are you looking up items by matching keys/values, or navigating sequentially?)
4. **Ordering Requirements** (Do items need sorting, prioritization, FIFO, or LIFO?)

```mermaid
flowchart TD
    A[Start] --> B{Thread Safety\nRequired?}

    %% === CONCURRENT CONTEXT ===
    B -->|Yes| C{Processing Pattern?}
    C -->|Async Producer / Consumer| C1[Channel]
    C -->|Blocking Producer / Consumer| E1[BlockingCollection]
    C -->|Shared Key-Value Storage| E2[ConcurrentDictionary]

    %% === MAIN LIFE CYCLE LIFETIME ===
    B -->|No| H{How will this data change\nover time?}

    %% Frozen Stream
    H -->|Built once at startup\nNever changes, read millions of times| L{How will you look up data?}
    L -->|By key mapping| M[FrozenDictionary]
    L -->|By unique values only| N[FrozenSet]

    %% ReadOnly View Stream
    H -->|It changes internally, but I want to\nblock consumers from modifying it| R2{How will consumers access it?}
    R2 -->|By key mapping| R3[ReadOnlyDictionary]
    R2 -->|By index or list iteration| R4[ReadOnlyCollection]

    %% Immutable Structural Stream
    H -->|It never changes, but I need to make\nnew updated copies safely| I{What is the primary action?}
    I -->|Mostly reading, rarely making a copy| J1[ImmutableArray]
    I -->|Frequently adding/removing items to make new copies| J2[ImmutableList]
    I -->|Looking up values by a key| J3[ImmutableDictionary]
    I -->|Tracking unique items| J4[ImmutableHashSet]

    %% Standard Mutable Stream
    H -->|Standard collection\nFrequent Add, Remove, and Clear| O{How do you want to find\nitems in the collection?}
    
    %% Lookups Sub-branch
    O -->|Look up by matching a Key or value| P{Do you need the items\nto be sorted?}
    P -->|No, just fast lookup| P1{Do you have a Key?}
    P1 -->|Yes, map Key to Value| T2[Dictionary]
    P1 -->|No, just check if Value exists| Q[HashSet]
    
    P -->|Yes, sort by Key| P2{Is the collection large\nwith frequent additions?}
    P2 -->|Yes, heavy additions| S[SortedDictionary]
    P2 -->|No, mostly setup once / low memory| S2[SortedList]

    %% Sequential Sub-branch
    O -->|Look up by order or position| U{What is the order pattern?}
    U -->|First-In, First-Out queue| V[Queue]
    U -->|Last-In, First-Out stack| W[Stack]
    U -->|By custom priority ranking| X[PriorityQueue]
    U -->|Keep values sorted automatically| Y[SortedSet]
    U -->|Direct index lookup like items[i]| Z[List]
```

---

## Quick Decision Matrix

| Context / Category | Requirement & Behavior | Recommended Collection |
| :--- | :--- | :--- |
| **Standard Mutable** | Dynamic list of items with direct index access (`[i]`) | [`List<T>`](./docs/collections/list.md) |
| **Standard Mutable** | High-speed key-based data mapping | [`Dictionary<TKey, TValue>`](./docs/collections/dictionary.md) |
| **Standard Mutable** | High-speed uniqueness and existence checks | [`HashSet<T>`](./docs/collections/hashset.md) |
| **Standard Mutable** | Process sequential items in arrival order (FIFO) | [`Queue<T>`](./docs/collections/queue.md) |
| **Standard Mutable** | Process sequential items in most-recent order (LIFO) | [`Stack<T>`](./docs/collections/stack.md) |
| **Standard Mutable** | Process sequential items ordered by a priority ranking | [`PriorityQueue<TElement, TPriority>`](./docs/collections/priorityqueue.md) |
| **Standard Mutable** | Kept sorted by key; optimized for frequent additions | [`SortedDictionary<TKey, TValue>`](./docs/collections/sorteddictionary.md) |
| **Standard Mutable** | Kept sorted by key; optimized for frequent read lookups | [`SortedList<TKey, TValue>`](./docs/collections/sortedlist.md) |
| **Standard Mutable** | Kept sorted by unique value elements | [`SortedSet<T>`](./docs/collections/sortedset.md) |
| **Concurrent** | Multi-threaded thread-safe key-value data storage | [`ConcurrentDictionary<TKey, TValue>`](./docs/collections/concurrentdictionary.md) |
| **Concurrent** | High-performance async producer–consumer messaging | [`Channel<T>`](./docs/collections/channel.md) |
| **Concurrent** | Multi-threaded thread-safe blocking pipeline messaging | [`BlockingCollection<T>`](./docs/collections/blockingcollection.md) |
| **Immutable** | Data never changes; optimized for maximum read performance | [`ImmutableArray<T>`](./docs/collections/immutablearray.md) |
| **Immutable** | Data never changes; optimized for frequent structural updates | [`ImmutableList<T>`](./docs/collections/immutablelist.md) |
| **Immutable** | Thread-safe, unchangeable key-value data storage | [`ImmutableDictionary<TKey, TValue>`](./docs/collections/immutabledictionary.md) |
| **Immutable** | Thread-safe, unchangeable set of unique elements | [`ImmutableHashSet<T>`](./docs/collections/immutablehashset.md) |
| **Static / Frozen** | Created once at app startup; optimized key lookups | [`FrozenDictionary<TKey, TValue>`](./docs/collections/frozendictionary.md) |
| **Static / Frozen** | Created once at app startup; optimized unique sets | [`FrozenSet<T>`](./docs/collections/frozenset.md) |
| **Read-Only View** | Safe, unmodifiable view tracking a dynamic underlying list | [`ReadOnlyCollection<T>`](./docs/collections/readonlycollection.md) |
| **Read-Only View** | Safe, unmodifiable view tracking dynamic key-value data | [`ReadOnlyDictionary<TKey, TValue>`](./docs/collections/readonlydictionary.md) |

---

## Common Mistakes

- **Using `List<T>.Contains` inside loops:** Forces sequential scans from index 0 to N on every single check. Convert the collection to a `HashSet<T>` to gain instant lookups.
- **Modifying `ImmutableArray<T>` in sequential loops:** Calling `.Add()` allocates a brand-new underlying array copy on every single iteration. Use `ImmutableArray.CreateBuilder<T>()` to populate data in memory before freezing it.
- **Leaving collection capacities unassigned:** Instantiating collections without a size hint triggers multiple array reallocations and memory copies as elements grow. Always pass an estimated size parameter to constructors like `new List<T>(capacity)`.
- **Using structural keys in dictionaries without equality contract overrides:** Passing custom `struct` configurations as dictionary keys without explicitly implementing `IEquatable<T>` and overriding `GetHashCode()` triggers heavy CPU boxing overhead and runtime reflection lookups on every access.
- **Double-looking up dictionary keys during updates:** Checking `ContainsKey()` immediately followed by an indexer update executes two separate hash table lookups. Use `TryGetValue()` or `CollectionsMarshal.GetValueRefOrAddDefault` to perform the entire operation in a single step.
- **Iterating `Dictionary.Keys` or `Dictionary.Values` to look up the matching pair:** Scanning one of the side-collections sequentially destroys the purpose of a map. Use `KeyValuePair<TKey, TValue>` iteration directly via `foreach (var (key, value) in dict)` to access keys and values together without extra lookups or array allocations.
- **Passing an active mutable collection to a `ReadOnlyCollection<T>` wrapper while continuing to modify the source:** Modifying the original `List<T>` after creating the wrapper will instantly change the wrapper's content. If you require a true snapshot that cannot change from underneath you, use `.ToFrozenSet()`, `.ToFrozenDictionary()`, or an immutable equivalent.
- **Using `ConcurrentBag<T>` as a generic substitute for `ConcurrentQueue<T>` in dedicated producer-consumer tracks:** When distinct threads only add items and completely different threads only pull them out, `ConcurrentBag<T>` forces constant cross-thread work-stealing synchronization locks. Use `ConcurrentQueue<T>` for separated workloads.
- **Overusing `SortedDictionary<TKey, TValue>` when updates are rare:** `SortedDictionary` uses a binary search tree, which incurs heavy object allocation overhead for its nodes. If your collection is populated once or twice and then primarily read, `SortedList<TKey, TValue>` is vastly superior because it stores elements in tightly packed arrays, saving substantial memory and improving CPU cache locality.
- **Enqueuing large objects into `PriorityQueue` with a complex, unoptimized priority type:** When sorting elements dynamically, `PriorityQueue` executes structural comparison swaps. If your priority parameter is a large mutable custom struct rather than a simple primitive (like an `int` or a `long`), .NET is forced to copy that value type across memory boundaries on every swap, driving up CPU cycles during updates.
- **Using `ConcurrentDictionary.GetOrAdd` with a lambda that allocates closure memory:** If your factory lambda references variables defined outside of its scope, the compiler secretly instantiates a hidden closure object on the heap on every single execution. Use the performance-oriented overload `GetOrAdd(key, (k, state) => factory(k, state), factoryState)` to pass external variables safely into the factory state context without allocations.
- **Treating `ConcurrentDictionary.Count` as a cheap property check:** Unlike a standard collection where checking the count reads a pre-calculated field, calling `.Count` on a `ConcurrentDictionary` forces the runtime to acquire locks sequentially across every single internal bucket partition to sum up the items safely. For presence checks in multi-threaded tracks, try iterating with `.IsEmpty` instead.
- **Failing to use `PriorityQueue.DequeueEnqueue` for rapid replacement loops:** When you need to pop the top priority item out and immediately push a replacement element in, calling `.Dequeue()` followed by `.Enqueue()` forces the internal binary heap structure to re-balance itself twice. Using the single combined `.DequeueEnqueue()` method executes both actions in a single optimized pass, cutting the structural sorting work in half.
- **Instantiating a `Queue<T>` or `Stack<T>` when you frequently need to check middle elements:** Queues and stacks are array-backed structures strictly optimized for entry-point and exit-point manipulations. If you regularly look up items in the middle of the chain using loops or linear scans, you lose the layout benefit. Use a standard `List<T>` which naturally supports fast direct indexing.
- **Using `List<bool>` or `bool[]` arrays for massive flags or binary toggle structures:** In .NET, a `bool` type actually consumes a full byte (8 bits) of memory storage. If you are holding millions of binary status toggles, a standard list wastes massive amounts of RAM. Switching to a `BitArray` compacts your data tightly by storing up to 32 independent boolean flags inside a single 32-bit integer block.
- **Using `ImmutableList<T>` when you only need a snapshot of an array:** If you just want to freeze an existing array or list so it cannot be mutated, calling `.ToImmutableList()` creates an expensive, multi-node binary tree structure in memory. If you only plan to read from the snapshot, use `.ToImmutableArray()` instead, which simply wraps a raw array with zero tree overhead.
- **Holding references to small sub-arrays using `Array.Copy` or standard slicing instead of `ReadOnlySpan<T>`:** When you slice or extract a subset of elements from a large array into a new collection, .NET allocates a brand-new array on the heap and copies the values over. For read-only processing on a window of data, use `ReadOnlySpan<T>` or `Memory<T>` to pass a zero-allocation virtual view pointing directly to the original memory layout.
- **Assuming `ReadOnlyCollection<T>` protects the elements inside it from being changed:** A read-only wrapper only stops a consumer from adding, removing, or replacing items in the list structure itself. If the collection holds mutable class objects, external code can still freely modify the properties of the individual objects inside the list. To achieve absolute safety for reference types, the objects themselves must be designed as immutable.
- **Casting a collection to `IEnumerable<T>` to pass it to a method, assuming it prevents modification:** Casting a `List<T>` or a `Dictionary<TKey, TValue>` down to a base interface like `IEnumerable<T>` hides the mutation methods from the compiler, but it does not change the runtime object. A malicious or careless consumer can easily cast the interface back to `IList<T>` or `List<T>` at runtime and completely modify your private collection state.

---

## Detailed Collections Catalog

### Core Mutable
- [List](./docs/collections/list.md) — Array-backed indexable dynamic list.
- [LinkedList](./docs/collections/linkedlist.md) — Node-backed dynamic sequence for fast center updates.
- [Queue](./docs/collections/queue.md) — First-In, First-Out sequence behavior.
- [Stack](./docs/collections/stack.md) — Last-In, First-Out sequence behavior.
- [Dictionary](./docs/collections/dictionary.md) — High-speed unique key-to-value map storage.
- [HashSet](./docs/collections/hashset.md) — High-speed membership validation collection.

### Automatically Sorted
- [SortedList](./docs/collections/sortedlist.md) — Fast-reading, memory-efficient sorted key-value array.
- [SortedDictionary](./docs/collections/sorteddictionary.md) — Fast-updating binary tree sorted key-value map.
- [SortedSet](./docs/collections/sortedset.md) — Automatically sorted unique values element tree.

### Concurrent / Thread-Safe
- [ConcurrentDictionary](./docs/collections/concurrentdictionary.md) — Thread-safe concurrent map storage.
- [ConcurrentQueue](./docs/collections/concurrentqueue.md) — Thread-safe dynamic FIFO queue.
- [ConcurrentStack](./docs/collections/concurrentstack.md) — Thread-safe dynamic LIFO stack.
- [ConcurrentBag](./docs/collections/concurrentbag.md) — Thread-safe unordered object storage wrapper.
- [BlockingCollection](./docs/collections/blockingcollection.md) — Thread-safe blocking pipeline coordinator.
- [Channel](./docs/collections/channel.md) — Async producer-consumer high-performance messaging.

### Immutable (Structural Sharing)
- [ImmutableArray](./docs/collections/immutablearray.md) — Value-type array wrapper optimized for high-speed reads.
- [ImmutableList](./docs/collections/immutablelist.md) — Tree-backed structure for balanced copy modifications.
- [ImmutableHashSet](./docs/collections/immutablehashset.md) — Safe immutable snapshot validation set.
- [ImmutableDictionary](./docs/collections/immutabledictionary.md) — Safe immutable snapshot tree key-to-value map.

### Highly Specialized
- [PriorityQueue](./docs/collections/priorityqueue.md) — Sorts elements dynamically by custom rank metadata.
- [FrozenDictionary](./docs/collections/frozendictionary.md) — Fixed map built once at startup for max read speed.
- [FrozenSet](./docs/collections/frozenset.md) — Fixed set built once at startup for max read speed.
- [BitArray](./docs/collections/bitarray.md) — Highly compact packed collection managing boolean flag bits.

### Protected Read-Only Wrappers
- [ReadOnlyCollection](./docs/collections/readonlycollection.md) — Prevents mutations while tracking live list adjustments.
- [ReadOnlyDictionary](./docs/collections/readonlydictionary.md) — Prevents mutations while tracking live map adjustments.

---

## Learn More

- [Comparisons](./docs/comparisons/)
- [Scenarios](./docs/scenarios/)
- [Benchmarks](./benchmarks/)
- [Advanced topics](./docs/advanced/)

---

## Rules of Thumb

### General Defaults
- **Start with `List<T>` or `Dictionary<TKey, TValue>`:** These are your default, versatile workhorses for everyday business logic.
- **Preallocate immediately:** If you know (or can roughly estimate) the final count, always pass the capacity parameter to the constructor `new List<T>(N)` to stop CPU-heavy memory resizing.
- **Switch to `HashSet<T>` for scanning workloads:** If you ever find yourself calling `.Contains()` inside a loop against a standard list, stop and convert that collection into a `HashSet<T>`.

### Concurrency & Pipelines
- **Use `Channel<T>` for cross-thread data streams:** If you are passing data between threads asynchronously, default to channels. They are significantly faster and produce fewer allocations than older thread-safe alternatives.
- **Save `ConcurrentDictionary` for state caching:** Only use it when multiple threads actively need to read, write, and update a single shared state map simultaneously.

### Static Data & Memory
- **Use `Frozen` collections for app configuration:** If your data is built once at application startup (like JSON configs, dependency setups, or static codes) and read millions of times, use a `FrozenDictionary` or `FrozenSet`. 
- **Use `Immutable` structures for functional code snapshots:** If your data never changes but you frequently need to thread-safely emit slightly altered *copies* of it, choose `Immutable` collections. Use `ImmutableArray` if you mostly read, and `ImmutableList` if you constantly modify.
- **Use `ReadOnly` wrappers only to hide class internals:** Use these strictly to shield public APIs. They do not prevent the underlying data from changing; they just prevent external code from doing the changing.

