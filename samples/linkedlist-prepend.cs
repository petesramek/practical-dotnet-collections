using System;
using System.Collections.Generic;

// Prepending items efficiently using LinkedList<T>
var input = new[] { 1, 2, 3, 4, 5 };

var list = new LinkedList<int>();

foreach (var item in input)
{
    list.AddFirst(item);
}

foreach (var item in list)
{
    Console.WriteLine(item);
}
