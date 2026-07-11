using System;
using System.Collections.Immutable;

// Immutable data structure example
var array = ImmutableArray.Create(1, 2, 3);

var updated = array.Add(4);

Console.WriteLine("Original:");
foreach (var item in array)
    Console.WriteLine(item);

Console.WriteLine("Updated:");
foreach (var item in updated)
    Console.WriteLine(item);
