```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26300.9711)
virt-10.0 1.00GHz, 1 CPU, 4 logical and 4 physical cores
.NET SDK 10.0.403
  [Host]     : .NET 10.0.14 (10.0.14, 10.0.1426.52326), Arm64 RyuJIT armv8.0-a
  Job-LGHQEI : .NET 10.0.14 (10.0.14, 10.0.1426.52326), Arm64 RyuJIT armv8.0-a

IterationCount=8  WarmupCount=3  

```
| Method            | N      | Mean             | Error           | StdDev          | Gen0     | Gen1     | Gen2     | Allocated |
|------------------ |------- |-----------------:|----------------:|----------------:|---------:|---------:|---------:|----------:|
| **CreateFrozenSet**   | **10000**  |   **604,234.749 ns** | **305,912.2878 ns** | **159,998.0417 ns** |  **90.8203** |  **90.8203** |  **90.8203** |  **370157 B** |
| FrozenSetContains | 10000  |         2.457 ns |       0.1732 ns |       0.0906 ns |        - |        - |        - |         - |
| HashSetContains   | 10000  |         4.324 ns |       0.1893 ns |       0.0990 ns |        - |        - |        - |         - |
| **CreateFrozenSet**   | **100000** | **4,169,601.510 ns** | **736,993.0344 ns** | **327,229.4750 ns** | **414.0625** | **410.1563** | **410.1563** | **3943393 B** |
| FrozenSetContains | 100000 |         4.444 ns |       1.4209 ns |       0.7431 ns |        - |        - |        - |         - |
| HashSetContains   | 100000 |         5.442 ns |       0.1461 ns |       0.0521 ns |        - |        - |        - |         - |
