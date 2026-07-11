using System;
using System.Collections.Generic;

// Sequential processing using List<T>
var results = new List<int>();

for (int i = 0; i < 5; i++)
{
    results.Add(i * 10);
}

foreach (var item in results)
{
    Console.WriteLine(item);
}
