# Progress

## Core Collections
- [x] List<T>
- [x] Dictionary<TKey, TValue>
- [x] HashSet<T>
- [x] LinkedList<T>
- [x] Queue<T>
- [x] Stack<T>

## Sorted Collections
- [x] SortedSet<T>
- [x] SortedDictionary<TKey, TValue>
- [x] SortedList<TKey, TValue>

## Concurrent Collections
- [x] ConcurrentDictionary<TKey, TValue>
- [x] ConcurrentQueue<T>
- [x] ConcurrentStack<T>
- [x] ConcurrentBag<T>
- [x] BlockingCollection<T>
- [x] Channel<T>

## Immutable Collections
- [x] ImmutableArray<T>
- [x] ImmutableList<T>
- [x] ImmutableHashSet<T>
- [x] ImmutableDictionary<TKey, TValue>

## Frozen Collections
- [x] FrozenSet<T>
- [x] FrozenDictionary<TKey, TValue>

## Specialized Collections
- [x] PriorityQueue<TElement, TPriority>

## Wrapper & Specialized Types
- [ ] ReadOnlyCollection<T>
- [ ] ReadOnlyDictionary<TKey, TValue>
- [ ] BitArray

## Performance Primitives (Advanced)
- [ ] ArrayPool<T>
- [ ] Span<T>
- [ ] ReadOnlySpan<T>

## Comparisons
- [ ] list-vs-linkedlist
- [ ] dictionary-vs-sorteddictionary
- [ ] hashset-vs-list-lookup
- [ ] immutablearray-vs-immutablelist
- [ ] list-default-vs-capacity
- [ ] channel-vs-blockingcollection
- [ ] sorteddictionary-vs-sortedlist
- [ ] dictionary-vs-frozendictionary
- [ ] hashset-vs-frozenset

## Benchmarks
- [x] dictionary-vs-list-lookup
- [x] hashset-vs-list-lookup
- [x] list-add-vs-insert
- [x] linkedlist-add-vs-list-insert
- [x] queue-vs-list-removeat
- [x] stack-vs-list-end-ops
- [ ] list-capacity-benchmark
- [ ] immutable-builder-benchmark
- [ ] failed-lookup-benchmark

## Scenarios
- [x] caching
- [x] deduplication
- [x] producer-consumer
- [ ] configuration-caching (FrozenDictionary)
- [ ] async-pipeline (Channel)
- [ ] concurrent-deduplication (ConcurrentDictionary)
- [ ] undo-redo (Stack)
- [ ] sliding-window-buffer (Queue)
- [ ] priority-processing (PriorityQueue)

## Advanced Topics
- [ ] large-object-heap (LOH)
- [ ] struct-dictionary-keys (boxing)
