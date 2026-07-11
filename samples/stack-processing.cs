using System;
using System.Collections.Generic;

// LIFO processing using Stack<T>
var stack = new Stack<string>();

stack.Push("step-1");
stack.Push("step-2");
stack.Push("step-3");

while (stack.Count > 0)
{
    var item = stack.Pop();
    Console.WriteLine($"Processing {item}");
}
