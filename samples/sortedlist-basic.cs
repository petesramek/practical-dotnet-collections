using System;
using System.Collections.Generic;

// Compact sorted key/value storage
var sorted = new SortedList<int, string>();

sorted.Add(3, "three");
sorted.Add(1, "one");
sorted.Add(2, "two");

foreach (var item in sorted)
{
    Console.WriteLine($"{item.Key} -> {item.Value}");
}
