## HashSet<TKey>

### What it is
  
HashSet<TKey> is a hash-based collection that stores unique elements and provides fast existence checks.

### Typical use cases
- Deduplication
- Membership checks
- Set operations (union, intersection)

### Sample usage
  
See:
/samples/hashset-deduplication.cs

#### How to run the sample
  
From repository root:
dotnet run samples/hashset-deduplication.cs  
Notes:
- Requires .NET 10 or later
- Runs as a standalone script

### Internal implementation
  
HashSet<TKey> is implemented using:
- buckets array
- entries array

#### Lookup flow
- Compute hash code
- Map to bucket
- Traverse collision chain
- Compare keys

### Memory characteristics
- Similar overhead to Dictionary without values
- Stores buckets and entries
- Resizes cause rehashing

### Complexity overview
  
Lookup: O(1) average
Add: O(1) average
Remove: O(1) average
Worst case: O(n)

### Benchmark results

#### Scenario
  
Compare lookup performance:
- HashSet<T>.Contains
- List<T>.Contains

#### How to run this benchmark only
  
From repository root:
cd benchmarks
dotnet run -c Release -- --filter *HashSetLookupBenchmark*  

#### Benchmark code
  
/benchmarks/Lookup/HashSetLookupBenchmark.cs

#### Results

| N | HashSet (ns) | List (ns) |
|---|-------------|-----------|
| 10 | 2.1 | 1.0 |
| 100 | 2.2 | 5.0 |
| 1,000 | 2.2 | 59.1 |
| 10,000 | 2.0 | 475.4 |
| 100,000 | 2.2 | 4579.1 |

#### Interpretation
- HashSet lookup remains effectively constant (~2 ns) regardless of size
- List lookup grows linearly with collection size
- At very small sizes (N=10), List is faster due to lower overhead and better cache locality
- Around N≈100, HashSet overtakes List
- Beyond that point, the gap increases rapidly
- At 100,000 elements, List lookup is ~2000x slower

### Practical optimizations
- Use HashSet for membership checks instead of List

### Common mistakes
- Using List for frequent Contains checks
- Using HashSet when ordering is required

### When I would choose it
- Need uniqueness
- Frequent existence checks
- Deduplication scenarios

### When I would avoid it
- Need ordering
- Sequential access only

### Rule of thumb
  
Use HashSet<TKey> when you need fast existence checks and uniqueness.
