# List<T> vs LinkedList<T>

## What this comparison answers

When should you use a contiguous array (List) vs a node-based structure (LinkedList)?

---

## Key differences

| Aspect | List<T> | LinkedList<T> |
|-------|--------|--------------|
| Storage | Single contiguous array | Nodes linked via pointers |
| Memory | Compact | High overhead (per node) |
| Cache locality | Excellent | Poor |
| Insert/remove (middle) | Requires shifting | Adjusts pointers |
| Iteration | Very fast | Slower (pointer chasing) |

---

## Internal behavior

### List<T>
- Backed by a single array
- Adding beyond capacity allocates a new array and copies data
- Removing from middle shifts elements

### LinkedList<T>
- Each element is a separate object
- Each node stores references to next/previous
- Insert/remove updates pointers only

---

## Benchmark scenarios

### 1. Iteration
- List benefits from CPU cache locality
- LinkedList suffers from pointer traversal

### 2. Insert in middle
- List shifts elements in memory
- LinkedList updates node references

---

## Practical guidance

### Use List<T> when
- You iterate frequently
- You access by index
- You want minimal memory overhead

### Use LinkedList<T> when
- You already have node reference
- You insert/remove in the middle frequently
- Order matters but indexing does not

---

## The trap

LinkedList looks attractive for “fast inserts”, but:

- You rarely have direct node reference
- Finding position requires traversal
- Memory overhead is significant

In most real applications, List<T> is faster even for many modifications.

---

## Rule of thumb

Start with List<T>. Only switch to LinkedList<T> if you have a proven need for node-based operations.
