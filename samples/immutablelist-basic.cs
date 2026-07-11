using System;
using System.Collections.Immutable;

// ImmutableList with structural sharing
var list = ImmutableList.Create(1, 2, 3);

var updated = list.Add(4);

Console.WriteLine("Original:");
foreach (var item in list)
    Console.WriteLine(item);

Console.WriteLine("Updated:");
foreach (var item in updated)
    Console.WriteLine(item);
