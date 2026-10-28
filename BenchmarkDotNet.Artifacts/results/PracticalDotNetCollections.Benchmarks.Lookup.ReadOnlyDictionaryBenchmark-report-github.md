```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26300.9711)
virt-10.0 1.00GHz, 1 CPU, 4 logical and 4 physical cores
.NET SDK 10.0.403
  [Host]     : .NET 10.0.14 (10.0.14, 10.0.1426.52326), Arm64 RyuJIT armv8.0-a
  Job-LGHQEI : .NET 10.0.14 (10.0.14, 10.0.1426.52326), Arm64 RyuJIT armv8.0-a

IterationCount=8  WarmupCount=3  

```
| Method                           | N     | Mean          | Error         | StdDev      | Gen0   | Allocated |
|--------------------------------- |------ |--------------:|--------------:|------------:|-------:|----------:|
| **DictionaryLookup**                 | **1000**  |      **4.570 ns** |     **0.3263 ns** |   **0.1706 ns** |      **-** |         **-** |
| ReadOnlyDictionaryLookup         | 1000  |      7.420 ns |     3.0771 ns |   1.6094 ns |      - |         - |
| IterateDictionaryForeach         | 1000  |  1,187.709 ns |    13.5001 ns |   5.9941 ns |      - |         - |
| IterateReadOnlyDictionaryForeach | 1000  |  3,138.183 ns |   225.7794 ns | 118.0870 ns | 0.0229 |      48 B |
| **DictionaryLookup**                 | **10000** |      **4.486 ns** |     **0.3159 ns** |   **0.1652 ns** |      **-** |         **-** |
| ReadOnlyDictionaryLookup         | 10000 |      6.295 ns |     1.8386 ns |   0.8163 ns |      - |         - |
| IterateDictionaryForeach         | 10000 | 12,048.081 ns |   343.3509 ns | 152.4499 ns |      - |         - |
| IterateReadOnlyDictionaryForeach | 10000 | 31,103.656 ns | 1,005.5211 ns | 525.9070 ns |      - |      48 B |
