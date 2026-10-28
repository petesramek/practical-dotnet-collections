```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26300.9711)
virt-10.0 1.00GHz, 1 CPU, 4 logical and 4 physical cores
.NET SDK 10.0.403
  [Host]     : .NET 10.0.14 (10.0.14, 10.0.1426.52326), Arm64 RyuJIT armv8.0-a
  Job-LGHQEI : .NET 10.0.14 (10.0.14, 10.0.1426.52326), Arm64 RyuJIT armv8.0-a

IterationCount=8  WarmupCount=3  

```
| Method                            | N     | Mean        | Error     | StdDev    | Gen0   | Allocated |
|---------------------------------- |------ |------------:|----------:|----------:|-------:|----------:|
| **IterateListForeach**                | **1000**  |    **547.8 ns** |   **6.77 ns** |   **3.01 ns** |      **-** |         **-** |
| IterateReadOnlyCollectionForeach  | 1000  |  2,626.1 ns |  43.02 ns |  19.10 ns | 0.0191 |      40 B |
| IterateListForIndex               | 1000  |    650.3 ns |  47.91 ns |  25.06 ns |      - |         - |
| IterateReadOnlyCollectionForIndex | 1000  |  1,622.5 ns |  39.73 ns |  17.64 ns |      - |         - |
| **IterateListForeach**                | **10000** |  **5,425.6 ns** |  **91.30 ns** |  **40.54 ns** |      **-** |         **-** |
| IterateReadOnlyCollectionForeach  | 10000 | 26,266.8 ns | 641.90 ns | 335.73 ns |      - |      40 B |
| IterateListForIndex               | 10000 |  6,340.0 ns | 105.29 ns |  55.07 ns |      - |         - |
| IterateReadOnlyCollectionForIndex | 10000 | 15,840.8 ns | 188.65 ns |  83.76 ns |      - |         - |
