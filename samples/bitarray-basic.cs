using System;
using System.Collections;

// Dense boolean storage using BitArray
var flags = new BitArray(8);

flags[0] = true;
flags[3] = true;
flags[7] = true;

for (int i = 0; i < flags.Length; i++)
{
    Console.WriteLine($"Index {i}: {flags[i]}");
}
