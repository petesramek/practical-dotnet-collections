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
- [x] ReadOnlyCollection<T>
- [x] ReadOnlyDictionary<TKey, TValue>
- [x] BitArray

## Comparisons
- [x] list-vs-linkedlist
- [x] dictionary-vs-sorteddictionary
- [x] hashset-vs-list-lookup
- [x] immutablearray-vs-immutablelist
- [x] list-default-vs-capacity
- [x] channel-vs-blockingcollection
- [x] sorteddictionary-vs-sortedlist
- [x] dictionary-vs-frozendictionary
- [x] hashset-vs-frozenset

## Benchmarks
- [x] dictionary-vs-list-lookup
- [x] hashset-vs-list-lookup
- [x] list-add-vs-insert
- [x] linkedlist-add-vs-list-insert
- [x] queue-vs-list-removeat
- [x] stack-vs-list-end-ops
- [x] list-capacity-benchmark
- [x] immutable-builder-benchmark
- [ ] failed-lookup-benchmark

## Scenarios
- [x] caching
- [x] deduplication
- [x] producer-consumer
- [x] configuration-caching (FrozenDictionary)
- [x] async-pipeline (Channel)
- [x] concurrent-deduplication (ConcurrentDictionary)
- [x] undo-redo (Stack)
- [x] sliding-window-buffer (Queue)
- [ ] priority-processing (PriorityQueue)

## Advanced Topics
- [ ] large-object-heap (LOH)
- [ ] struct-dictionary-keys (boxing)
