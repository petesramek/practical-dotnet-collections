# Priority Processing (PriorityQueue<TElement, TPriority>)

## Scenario

Process items based on priority instead of arrival order.

Examples:
- job scheduling
- task prioritization
- message processing with urgency levels

---

## Problem

Standard queues process items in FIFO order:
- no prioritization
- important work can be delayed

---

## Solution

Use PriorityQueue<TElement, TPriority>:

`csharp
var queue = new PriorityQueue<string, int>();

queue.Enqueue("low", 3);
queue.Enqueue("medium", 2);
queue.Enqueue("high", 1);

while (queue.Count > 0)
{
    var item = queue.Dequeue();
    Console.WriteLine(item);
}
`

---

## Why it works

- internally uses a binary heap
- smallest priority value is processed first
- maintains ordering efficiently during inserts/removals

---

## When to use

- task scheduling systems
- background workers with priorities
- algorithms (Dijkstra, A*)

---

## When NOT to use

- strict FIFO processing → use Queue<T>
- simple sorting → use List<T>.Sort

---

## The trap

Using sorting repeatedly:

`csharp
list.Add(item);
list.Sort();
`

- repeatedly sorts entire dataset
- inefficient for continuous processing

PriorityQueue processes incrementally instead.

---

## Rule of thumb

Use PriorityQueue<TElement, TPriority> when processing order depends on priority, not insertion order.
