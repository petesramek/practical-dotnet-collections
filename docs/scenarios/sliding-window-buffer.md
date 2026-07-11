# Sliding Window Buffer (Queue<T>)

## Scenario

Keep only the most recent N items (e.g., logs, metrics, telemetry, recent events).

---

## Problem

You need:
- fixed-size buffer
- automatic removal of oldest items
- efficient append + removal

---

## Solution

Use Queue<T> with a fixed capacity pattern:

`csharp
var capacity = 5;
var buffer = new Queue<int>(capacity);

void Add(int value)
{
    if (buffer.Count == capacity)
        buffer.Dequeue(); // remove oldest

    buffer.Enqueue(value);
}
`

---

## Why it works

- Queue<T> is FIFO (first-in-first-out)
- Enqueue adds newest item
- Dequeue removes oldest item

---

## Example

`csharp
Add(1);
Add(2);
Add(3);
Add(4);
Add(5);
Add(6);

// buffer now contains: 2,3,4,5,6
`

---

## When to use

- rolling logs
- recent events tracking
- telemetry windows
- rate limiting / throttling

---

## When NOT to use

- need random access
- need full history

---

## The trap

Using List<T> and removing from the front:

`csharp
list.RemoveAt(0);
`

- shifts entire array
- becomes expensive at scale

Queue<T> avoids this by moving a pointer.

---

## Rule of thumb

Use Queue<T> for fixed-size rolling buffers where oldest items must be discarded.
