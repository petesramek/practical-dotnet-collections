using System;
using System.Collections.Frozen;
using System.Linq;

// High-performance read-only lookup using FrozenSet<T>
var input = Enumerable.Range(1, 5);

var frozen = input.ToFrozenSet();

Console.WriteLine(frozen.Contains(3)); // true
Console.WriteLine(frozen.Contains(10)); // false
