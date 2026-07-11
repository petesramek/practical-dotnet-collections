# Dictionary<TKey, TValue>

## What it is
Hash-based key-value collection optimized for fast lookups.

## Typical use cases
- ID â†’ entity lookup
- Caching
- Deduplication with metadata

## Internal implementation
- Buckets + entries arrays
- Hash-based indexing

## Memory characteristics
- Extra memory for buckets
- Resizes as capacity grows

## Complexity overview
- Lookup: O(1) average
- Insert: O(1) average
- Worst case: O(n)

## Benchmark results
(TODO)

## Practical optimizations
- Pre-size when possible

## Common mistakes
- Ignoring hash distribution

## When I would choose it
- Fast key-based lookup required

## When I would avoid it
- Ordering required

## Rule of thumb
Use for fast key-based access.
