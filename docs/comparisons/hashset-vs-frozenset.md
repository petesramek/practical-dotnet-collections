# HashSet<T> vs FrozenSet<T>

## What this comparison answers

When should you use a mutable set vs a precomputed, read-only set optimized for lookups?

---

## Key differences

| Aspect | HashSet<T> | FrozenSet<T> |
|-------|------------|--------------|
| Mutability | Mutable | Read-only |
| Build cost | Low | Higher (precomputed) |
| Lookup | Very fast | Fastest possible |
| Memory layout | General-purpose | Optimized for specific dataset |
| Updates | Supported | Not supported |

---

## Internal behavior

### HashSet<T>
- Uses hash buckets
- Balances inserts, removals, and lookups
- May resize and rehash as it grows

### FrozenSet<T>
- Built once from existing data
- Precomputes optimal layout for given elements
- Removes mutation logic
- Optimized purely for lookup speed

---

## Benchmark scenarios

### 1. Lookup-heavy workloads
- HashSet: very fast
- FrozenSet: slightly faster due to optimized layout

### 2. Build cost
- HashSet: cheap to create
- FrozenSet: higher upfront cost

---

## Practical guidance

### Use HashSet<T> when
- You add/remove items dynamically
- You need general-purpose set behavior
- Data changes over time

### Use FrozenSet<T> when
- Data is static after creation
- You perform many lookups
- You want maximum lookup performance

---

## The trap

Using HashSet for static, lookup-heavy data:

`csharp
var set = LoadData();

if (set.Contains(value))
{
    // called millions of times
}
`

Better:

`csharp
var set = LoadData();
var frozen = set.ToFrozenSet();

if (frozen.Contains(value))
{
    // optimized lookup
}
`

---

## Rule of thumb

If your dataset does not change after creation, convert it to FrozenSet<T> for optimal lookup performance.
