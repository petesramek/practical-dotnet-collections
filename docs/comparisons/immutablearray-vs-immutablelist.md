# ImmutableArray<T> vs ImmutableList<T>

## What this comparison answers

When should you use a flat immutable array vs a tree-based immutable list?

---

## Key differences

| Aspect | ImmutableArray<T> | ImmutableList<T> |
|-------|------------------|------------------|
| Storage | Single contiguous array | Tree structure |
| Mutation | Copies entire array | Rebuilds small branch |
| Memory | Compact per instance | Shared structure between versions |
| Iteration | Very fast | Slightly slower |
| Updates | Expensive | More efficient |

---

## Internal behavior

### ImmutableArray<T>
- Wraps a standard array
- Every change allocates a new array
- Copies all elements to new memory

### ImmutableList<T>
- Uses a tree (AVL)
- Updates only affect part of the structure
- Reuses most of existing nodes

---

## Benchmark scenarios

### 1. Iteration
- ImmutableArray benefits from contiguous memory
- ImmutableList involves pointer traversal

### 2. Repeated Add
- ImmutableArray reallocates every time
- ImmutableList updates a small part of the tree

---

## Practical guidance

### Use ImmutableArray<T> when
- Data is created once and reused
- You read frequently
- You want best iteration performance

### Use ImmutableList<T> when
- You modify data repeatedly
- You build collections incrementally
- You want to reduce allocations

---

## The trap

Using ImmutableArray<T> inside loops:

`csharp
var array = ImmutableArray<int>.Empty;

for (int i = 0; i < 10000; i++)
{
    array = array.Add(i); // allocates new array each time
}
`

This creates thousands of allocations.

Better:

`csharp
var builder = ImmutableArray.CreateBuilder<int>();

for (int i = 0; i < 10000; i++)
{
    builder.Add(i);
}

var array = builder.ToImmutable();
`

---

## Rule of thumb

Use ImmutableArray<T> for read-heavy, stable data. Use ImmutableList<T> for frequently updated immutable collections.
