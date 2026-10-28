```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26300.9711)
virt-10.0 1.00GHz, 1 CPU, 4 logical and 4 physical cores
.NET SDK 10.0.403
  [Host]     : .NET 10.0.14 (10.0.14, 10.0.1426.52326), Arm64 RyuJIT armv8.0-a
  Job-LGHQEI : .NET 10.0.14 (10.0.14, 10.0.1426.52326), Arm64 RyuJIT armv8.0-a

IterationCount=8  WarmupCount=3  

```
| Method                      | N     | Mean             | Error           | StdDev         | Gen0      | Gen1     | Allocated |
|---------------------------- |------ |-----------------:|----------------:|---------------:|----------:|---------:|----------:|
| **ImmutableListAdd**            | **1000**  |   **210,658.097 ns** |   **8,544.0492 ns** |  **3,793.6108 ns** |  **240.4785** |        **-** |  **502896 B** |
| ImmutableListWithBuilderAdd | 1000  |   121,376.604 ns |   8,364.2537 ns |  4,374.6664 ns |   22.9492 |        - |   48024 B |
| ListAdd                     | 1000  |     3,549.350 ns |     350.1170 ns |    183.1180 ns |    4.0207 |        - |    8424 B |
| ImmutableListRead           | 1000  |       140.030 ns |      27.3203 ns |     14.2890 ns |         - |        - |         - |
| ListRead                    | 1000  |         1.108 ns |       0.1031 ns |      0.0539 ns |         - |        - |         - |
| **ImmutableListAdd**            | **10000** | **3,646,441.298 ns** | **240,180.7835 ns** | **85,650.7810 ns** | **1734.3750** | **648.4375** | **6653616 B** |
| ImmutableListWithBuilderAdd | 10000 | 1,611,571.920 ns |  96,405.3003 ns | 50,421.8362 ns |  103.5156 |  62.5000 |  480024 B |
| ListAdd                     | 10000 |    34,370.388 ns |   1,063.0599 ns |    556.0009 ns |   62.4390 |        - |  131400 B |
| ImmutableListRead           | 10000 |       121.154 ns |      29.1173 ns |     15.2289 ns |         - |        - |         - |
| ListRead                    | 10000 |         1.070 ns |       0.0269 ns |      0.0119 ns |         - |        - |         - |
