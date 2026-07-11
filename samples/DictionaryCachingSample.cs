using System.Collections.Generic;

namespace PracticalDotNetCollections.Samples;

public static class DictionaryCachingSample
{
    public static void Run()
    {
        var cache = new Dictionary<int, string>();

        cache[1] = "Alice";
        cache[2] = "Bob";

        if (cache.TryGetValue(1, out var value))
        {
            System.Console.WriteLine(value);
        }
    }
}
