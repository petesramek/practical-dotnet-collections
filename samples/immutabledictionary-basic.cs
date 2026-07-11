using System;
using System.Collections.Immutable;

// ImmutableDictionary with structural sharing
var dict = ImmutableDictionary.Create<int, string>();

dict = dict.Add(1, "one");
dict = dict.Add(2, "two");

var updated = dict.Add(3, "three");

Console.WriteLine("Original:");
foreach (var pair in dict)
    Console.WriteLine($"{pair.Key} -> {pair.Value}");

Console.WriteLine("Updated:");
foreach (var pair in updated)
    Console.WriteLine($"{pair.Key} -> {pair.Value}");
