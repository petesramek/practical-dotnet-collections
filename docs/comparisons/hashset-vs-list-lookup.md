# HashSet<T> vs List<T> (Lookup)

## What this comparison answers

When should you use a hash-based lookup vs scanning a list?

---

## Key differences

| Aspect | List<T> | HashSet<T> |
|-------|--------|------------|
| Storage | Contiguous array | Hash buckets |
| Lookup | Linear scan | Hash-based lookup |
| Memory | Compact | Higher overhead |
| Ordering | Preserved | Not guaranteed |
| Duplicates | Allowed | Not allowed |

---

## Internal behavior

### List<T>
- Stored in a single array
- Lookup requires scanning elements one by one
- Best case: first item
- Worst case: last item or not found

### HashSet<T>
- Uses hashing to distribute items into buckets
- Lookup jumps directly to bucket
- May handle collisions internally

---

## Benchmark scenarios

### 1. Successful lookup
- List: may scan partially
- HashSet: direct bucket access

### 2. Failed lookup (IMPORTANT)
- List: scans entire array
- HashSet: still fast lookup

---

## Practical guidance

### Use List<T> when
- Collection is small
- You iterate more than you search
- Ordering matters

### Use HashSet<T> when
- You perform frequent lookups
- You need uniqueness
- Collection grows large

---

## The trap

Using List<T> for repeated lookups in large collections.

`csharp
if (list.Contains(value))
{
    // executed many times
}
`

This scans the list every time.

Replace with:

`csharp
var set = new HashSet<int>(list);

if (set.Contains(value))
{
    // fast lookup
}
`

---

## Rule of thumb

If you call .Contains() frequently, use HashSet<T>. If you mostly iterate, use List<T>.
