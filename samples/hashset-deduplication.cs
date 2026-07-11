using System;
using System.Collections.Generic;

// Demonstrates deduplication using HashSet<T>

var input = new[] { 1, 2, 1, 3, 2, 4, 3, 5 };

var set = new HashSet<int>();

foreach (var item in input)
{
    set.Add(item);
}

foreach (var item in set)
{
    Console.WriteLine(item);
}
