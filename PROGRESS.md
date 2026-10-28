# Progress

## Core Mutable
- [x] List<T>
- [x] LinkedList<T>
- [x] Queue<T>
- [x] Stack<T>
- [x] PriorityQueue<TElement, TPriority>
- [x] Dictionary<TKey, TValue>
- [x] HashSet<T>
- [x] BitArray

## Sorted
- [x] SortedSet<T>
- [x] SortedDictionary<TKey, TValue>
- [x] SortedList<TKey, TValue>

## Concurrent
- [x] ConcurrentDictionary<TKey, TValue>
- [x] ConcurrentQueue<T>
- [x] ConcurrentStack<T>
- [x] ConcurrentBag<T>
- [x] BlockingCollection<T>

## Immutable
- [x] ImmutableArray<T>
- [x] ImmutableList<T>
- [x] ImmutableHashSet<T>
- [x] ImmutableDictionary<TKey, TValue>

## Frozen
- [x] FrozenSet<T>
- [x] FrozenDictionary<TKey, TValue>

## Read-only Views
- [x] ReadOnlyCollection<T>
- [x] ReadOnlyDictionary<TKey, TValue>

## Related Types
- [x] Channel<T>

## Comparisons
- [x] list-vs-linkedlist
- [x] list-default-vs-capacity
- [x] hashset-vs-list-lookup
- [x] hashset-vs-frozenset
- [x] dictionary-vs-frozendictionary
- [x] dictionary-vs-sorteddictionary
- [x] sorteddictionary-vs-sortedlist
- [x] immutablearray-vs-immutablelist
- [x] channel-vs-blockingcollection

## Scenarios
- [x] configuration-caching (FrozenDictionary)
- [x] async-pipeline (Channel)
- [x] concurrent-deduplication (ConcurrentDictionary)
- [x] undo-redo (Stack)
- [x] sliding-window-buffer (Queue)
- [x] priority-processing (PriorityQueue)

## Advanced Topics
- [x] large-object-heap
- [x] struct-dictionary-keys

## Open items
- [x] A benchmark page for `FailedLookupBenchmark` (in the hashset-vs-list-lookup comparison).
- [ ] Re-run all benchmarks on one machine/runtime and record the environment in each page.
