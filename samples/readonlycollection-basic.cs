using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

var list = new List<int> { 1, 2, 3 };

var readOnly = list.AsReadOnly();

// readOnly.Add(4); // compile-time error

list.Add(4); // underlying collection still changes

foreach (var item in readOnly)
{
    Console.WriteLine(item);
}
