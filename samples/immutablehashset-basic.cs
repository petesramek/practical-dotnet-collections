using System;
using System.Collections.Immutable;

// ImmutableHashSet with structural sharing
var set = ImmutableHashSet.Create(1, 2, 3);

var updated = set.Add(4);

Console.WriteLine("Original:");
foreach (var item in set)
    Console.WriteLine(item);

Console.WriteLine("Updated:");
foreach (var item in updated)
    Console.WriteLine(item);
