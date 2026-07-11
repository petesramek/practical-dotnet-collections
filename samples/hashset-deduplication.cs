using System;
using System.Collections.Generic;

// Input with duplicates (e.g. imported data)
var input = new[] { 1, 2, 1, 3, 2, 4, 3, 5 };

// Track processed items
var seen = new HashSet<int>();

foreach (var item in input) {
    if (!seen.Add(item))
        continue; // already processed

    Console.WriteLine($"Processing {item}");
}