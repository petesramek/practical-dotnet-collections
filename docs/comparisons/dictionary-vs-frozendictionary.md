# Dictionary<TKey, TValue> vs FrozenDictionary<TKey, TValue>

## What this comparison answers

When should you use a general-purpose dictionary vs a precomputed, read-only dictionary optimized for lookups?

---

## Key differences

| Aspect | Dictionary<TKey, TValue> | FrozenDictionary<TKey, TValue> |
|-------|--------------------------|--------------------------------|
| Mutability | Mutable | Read-only |
| Build cost | Low | High (precomputation) |
| Lookup | Very fast | Fastest possible |
| Memory layout | General-purpose | Optimized for specific dataset |
| Updates | Supported | Not supported |

---

## Internal behavior

### Dictionary<TKey, TValue>
- Uses hash buckets
- Designed to balance inserts, updates, and lookups
- May resize and rehash as it grows

### FrozenDictionary<TKey, TValue>
- Built once from existing data
- Precomputes optimal layout for given keys
- Removes all mutation logic
- Optimized for repeated lookups

---

## Benchmark scenarios

### 1. Lookup-heavy workloads
- Dictionary: fast
- FrozenDictionary: slightly faster due to optimized layout

### 2. Build cost
- Dictionary: cheap to create
- FrozenDictionary: more expensive upfront

---

## Practical guidance

### Use Dictionary<TKey, TValue> when
- You need to add/update/remove items
- Data changes over time
- You build collections dynamically

### Use FrozenDictionary<TKey, TValue> when
- Data is loaded once (startup/config)
- You perform many lookups
- You want maximum lookup performance

---

## The trap

Using Dictionary for static data:

`csharp
var dict = LoadConfig();

// used millions of times
var value = dict[key];
`

This works, but you are not using the most optimized structure.

Better:

`csharp
var dict = LoadConfig();
var frozen = dict.ToFrozenDictionary();

var value = frozen[key];
`

---

## Rule of thumb

If data never changes after creation, convert it to FrozenDictionary<TKey, TValue> for optimal lookup performance.
