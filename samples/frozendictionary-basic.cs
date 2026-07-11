using System;
using System.Collections.Frozen;
using System.Linq;

// High-performance read-only lookup using FrozenDictionary<TKey, TValue>
var input = Enumerable.Range(1, 5).ToDictionary(x => x, x => $"value-{x}");

var frozen = input.ToFrozenDictionary();

Console.WriteLine(frozen[3]);
Console.WriteLine(frozen.ContainsKey(10));
