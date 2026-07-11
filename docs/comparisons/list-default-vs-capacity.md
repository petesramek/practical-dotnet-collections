# List<T> (Default) vs List<T> (Preallocated Capacity)

## What this comparison answers

When should you preallocate List capacity instead of relying on automatic resizing?

---

## Key differences

| Aspect | Default List<T> | Preallocated List<T> |
|-------|-----------------|---------------------|
| Initial capacity | Small (0 → 4 → grows) | Fixed upfront |
| Resizing | Multiple reallocations | None (if sized correctly) |
| Memory allocation | Many allocations | Single allocation |
| Performance | Slower for large builds | Faster and predictable |

---

## Internal behavior

### Default List<T>
- Starts with small internal array
- When capacity is exceeded:
  - Allocates new larger array (typically doubles)
  - Copies all elements to new array
  - Old array becomes garbage

### Preallocated List<T>
- Allocates required memory once
- No resizing or copying during population

---

## Benchmark scenarios

### 1. Building large collections
- Default List triggers repeated allocations and copies
- Preallocated List performs single allocation

### 2. Memory pressure
- Default List produces multiple temporary arrays
- Preallocated List minimizes garbage collection

---

## Practical guidance

### Use default List<T> when
- Collection size is small or unknown
- Performance is not critical

### Use preallocated List<T> when
- You know approximate size
- You build large collections
- You want to reduce allocations

---

## The trap

Building large lists without capacity:

`csharp
var list = new List<int>();

for (int i = 0; i < 100000; i++)
{
    list.Add(i);
}
`

This causes multiple reallocations and copies.

Better:

`csharp
var list = new List<int>(100000);

for (int i = 0; i < 100000; i++)
{
    list.Add(i);
}
`

---

## Rule of thumb

If you know the size, always set capacity. It eliminates repeated allocations and improves performance.
