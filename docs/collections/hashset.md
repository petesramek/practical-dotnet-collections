# HashSet<T>

## What it is

`HashSet<T>` is a hash-based collection that stores unique elements and provides fast existence checks.

## Typical use cases
- Deduplication
- Membership checks
- Set operations

## Sample usage

See:
`/samples/hashset-deduplication.cs`

### How to run the sample

```bash
dotnet run samples/hashset-deduplication.cs
```

Notes:
- Requires .NET 10 or later
- Runs as a standalone script

## Internal implementation

`HashSet<T>` is implemented using:
- buckets array
- entries array

### Lookup flow
- Compute hash code
- Map to bucket
- Traverse collision chain
- Compare keys

## Memory characteristics
- Similar overhead to `Dictionary<TKey, TValue>`
- Stores buckets and entries
- Resizes cause rehashing

## Complexity overview

Lookup: O(1) average  
Add: O(1) average  
Remove: O(1) average  
Worst case: O(n)

## Benchmark results

### Scenario

Compare lookup performance:
- `HashSet<T>.Contains`
- `List<T>.Contains`

### How to run this benchmark only

```bash
cd benchmarks
dotnet run -c Release -- --filter *HashSetLookupBenchmark*
```

### Benchmark code

`/benchmarks/Lookup/HashSetLookupBenchmark.cs`

### Results

| N | HashSet (ns) | List (ns) |
|---|-------------|-----------|
| 10 | 2.1 | 1.0 |
| 100 | 2.2 | 5.0 |
| 1,000 | 2.2 | 59.1 |
| 10,000 | 2.0 | 475.4 |
| 100,000 | 2.2 | 4579.1 |

### Interpretation

- HashSet lookup remains constant (~2 ns)
- List lookup grows linearly
- List is faster for very small datasets
- Around ~100 items HashSet becomes faster
- At large sizes List becomes orders of magnitude slower

## Practical optimizations
- Use `HashSet<T>` for membership checks

## Common mistakes
- Using `List<T>` for frequent `Contains`
- Using `HashSet<T>` when ordering is required

## When I would choose it
- Need uniqueness
- Frequent existence checks
- Deduplication

## When I would avoid it
- Need ordering
- Sequential access only

## Rule of thumb

Use `HashSet<T>` when you need fast existence checks and uniqueness.
