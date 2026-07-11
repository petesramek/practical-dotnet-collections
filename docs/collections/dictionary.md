# Dictionary<TKey, TValue>

## What it is
  
Dictionary<TKey, TValue> is a hash-based key-value collection optimized for fast lookups by key.
It provides near constant-time access to values based on a unique key.

## Typical use cases
- Caching (avoid repeated work for the same key)
- ID → entity lookup (e.g. userId → user)
- Deduplication with associated data
- Indexing data for fast access  
Example from this repository:
- /samples/dictionary-caching.cs

## Sample usage
  
See:
/samples/dictionary-caching.cs

### How to run the sample
  
From repository root:  
    dotnet run samples/dictionary-caching.cs

Notes:
- Requires .NET 10 or later
- Runs as a standalone script

## Internal implementation
  
Dictionary<TKey, TValue> is implemented using:
- buckets array
- entries array

### Lookup flow

- Compute hash code
- Map to bucket
- Traverse collision chain
- Compare keys

## Memory characteristics
- Extra arrays (buckets + entries)
- Higher overhead than List<T>
- Resizes cause rehashing

## Complexity overview
  
Lookup: O(1) average  
Insert: O(1) average  
Remove: O(1) average  
Worst case: O(n)

## Benchmark results

### Scenario
  
Compare lookup performance:
- Dictionary<int, int>
- List<int>.Contains

### How to run this benchmark only
  
From repository root:  
    cd benchmarks  
    dotnet run -c Release -- --filter *DictionaryLookupBenchmark*

Notes:
- Runs only the Dictionary lookup benchmark
- Output is stored in BenchmarkDotNet.Artifacts

### Benchmark code
  
/benchmarks/Lookup/DictionaryLookupBenchmark.cs

### Results
  
| N | Dictionary (ns) | List (ns) |
|---|-----------------|-----------|
| 10 | 2.7 | 1.0 |
| 100 | 2.3 | 5.1 |
| 1,000 | 2.2 | 57.6 |
| 10,000 | 2.5 | 468.6 |
| 100,000 | 2.4 | 4670.2 |

### Interpretation
  
- Dictionary lookup remains effectively constant (~2–3 ns) regardless of size
- List lookup grows linearly with collection size
- At very small sizes (N=10), List is faster due to lower overhead and better cache locality
- Around N≈100, Dictionary overtakes List
- Beyond that point, the gap increases rapidly
- At 100,000 elements, List lookup is ~2000x slower

## Practical optimizations
- Use TryGetValue
- Pre-size when possible

## Common mistakes
- Using for small datasets
- Using mutable keys
- Ignoring hash quality

## When I would choose it
- Frequent key-based lookup
- Repeated access patterns
- Caching

## When I would avoid it
- Need ordering
- Very small datasets
- Sequential access only

## Rule of thumb
  
Use Dictionary<TKey, TValue> when you need fast lookup by key.
