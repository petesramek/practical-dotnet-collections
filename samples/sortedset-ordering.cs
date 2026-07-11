using System;
using System.Collections.Generic;

// Maintaining sorted unique values using SortedSet<T>
var input = new[] { 5, 1, 3, 2, 5, 4, 1 };

var set = new SortedSet<int>(input);

foreach (var item in set)
{
    Console.WriteLine(item);
}
