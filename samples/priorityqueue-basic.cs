using System;
using System.Collections.Generic;

// Processing items by priority
var pq = new PriorityQueue<string, int>();

pq.Enqueue("low", 3);
pq.Enqueue("medium", 2);
pq.Enqueue("high", 1);

while (pq.Count > 0)
{
    Console.WriteLine(pq.Dequeue());
}
