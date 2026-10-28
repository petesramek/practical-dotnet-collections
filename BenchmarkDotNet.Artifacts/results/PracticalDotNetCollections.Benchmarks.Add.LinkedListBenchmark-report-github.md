```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26300.9711)
virt-10.0 1.00GHz, 1 CPU, 4 logical and 4 physical cores
.NET SDK 10.0.403
  [Host]     : .NET 10.0.14 (10.0.14, 10.0.1426.52326), Arm64 RyuJIT armv8.0-a
  Job-LGHQEI : .NET 10.0.14 (10.0.14, 10.0.1426.52326), Arm64 RyuJIT armv8.0-a

IterationCount=8  WarmupCount=3  

```
| Method                | N     | Mean             | Error             | StdDev          | Gen0    | Gen1    | Allocated |
|---------------------- |------ |-----------------:|------------------:|----------------:|--------:|--------:|----------:|
| **ListInsertAtBeginning** | **1000**  |    **71,920.901 ns** |    **19,398.8317 ns** |   **8,613.2015 ns** |  **3.9063** |       **-** |    **8392 B** |
| LinkedListAddFirst    | 1000  |    16,643.207 ns |       755.1503 ns |     335.2914 ns | 22.9492 |       - |   48040 B |
| LinkedListReadIndex   | 1000  |       760.756 ns |        54.8993 ns |      28.7134 ns |       - |       - |         - |
| ListReadIndex         | 1000  |         1.549 ns |         0.2242 ns |       0.0995 ns |       - |       - |         - |
| **ListInsertAtBeginning** | **10000** | **7,564,462.484 ns** | **1,411,572.0659 ns** | **738,279.4849 ns** | **46.8750** |       **-** |  **131368 B** |
| LinkedListAddFirst    | 10000 |   264,712.200 ns |    21,673.7230 ns |  11,335.7762 ns | 98.3887 | 61.0352 |  480040 B |
| LinkedListReadIndex   | 10000 |     9,248.812 ns |     1,517.6221 ns |     673.8336 ns |       - |       - |         - |
| ListReadIndex         | 10000 |         1.346 ns |         0.3520 ns |       0.1841 ns |       - |       - |         - |
