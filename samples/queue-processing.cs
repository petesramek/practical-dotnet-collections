using System;
using System.Collections.Generic;

// FIFO processing using Queue<T>
var queue = new Queue<string>();

queue.Enqueue("task-1");
queue.Enqueue("task-2");
queue.Enqueue("task-3");

while (queue.Count > 0)
{
    var item = queue.Dequeue();
    Console.WriteLine($"Processing {item}");
}
