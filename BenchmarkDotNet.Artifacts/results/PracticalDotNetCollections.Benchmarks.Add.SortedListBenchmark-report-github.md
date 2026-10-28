```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26300.9711)
virt-10.0 1.00GHz, 1 CPU, 4 logical and 4 physical cores
.NET SDK 10.0.403
  [Host]     : .NET 10.0.14 (10.0.14, 10.0.1426.52326), Arm64 RyuJIT armv8.0-a
  Job-LGHQEI : .NET 10.0.14 (10.0.14, 10.0.1426.52326), Arm64 RyuJIT armv8.0-a

IterationCount=8  WarmupCount=3  

```
| Method                    | N     | Mean            | Error            | StdDev         | Gen0    | Gen1    | Allocated |
|-------------------------- |------ |----------------:|-----------------:|---------------:|--------:|--------:|----------:|
| **SortedListRandomAdd**       | **1000**  |   **130,762.35 ns** |    **24,016.882 ns** |  **12,561.294 ns** |  **7.8125** |       **-** |   **16848 B** |
| SortedDictionaryRandomAdd | 1000  |   126,424.63 ns |    10,800.755 ns |   4,795.602 ns | 22.9492 |       - |   48112 B |
| SortedListLookup          | 1000  |        77.22 ns |        17.405 ns |       9.103 ns |       - |       - |         - |
| SortedDictionaryLookup    | 1000  |        57.62 ns |         7.425 ns |       3.883 ns |       - |       - |         - |
| **SortedListRandomAdd**       | **10000** | **9,849,729.38 ns** | **1,492,302.591 ns** | **662,591.599 ns** | **93.7500** | **31.2500** |  **262800 B** |
| SortedDictionaryRandomAdd | 10000 | 2,584,087.85 ns |   623,939.769 ns | 326,332.564 ns | 93.7500 | 62.5000 |  480112 B |
| SortedListLookup          | 10000 |       102.12 ns |        16.396 ns |       8.575 ns |       - |       - |         - |
| SortedDictionaryLookup    | 10000 |       104.54 ns |        40.959 ns |      21.422 ns |       - |       - |         - |
