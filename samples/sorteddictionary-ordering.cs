using System;
using System.Collections.Generic;

// Maintaining ordered key/value data using SortedDictionary<TKey, TValue>
var data = new SortedDictionary<int, string>();

data[3] = "three";
data[1] = "one";
data[2] = "two";

foreach (var pair in data)
{
    Console.WriteLine($"{pair.Key} -> {pair.Value}");
}
