using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

var dict = new Dictionary<int, string>
{
    [1] = "one"
};

var readOnly = new ReadOnlyDictionary<int, string>(dict);

// readOnly[2] = "two"; // not allowed

dict[2] = "two"; // underlying dictionary changes

Console.WriteLine(readOnly[2]); // "two"
