```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26300.9711)
virt-10.0 1.00GHz, 1 CPU, 4 logical and 4 physical cores
.NET SDK 10.0.403
  [Host]     : .NET 10.0.14 (10.0.14, 10.0.1426.52326), Arm64 RyuJIT armv8.0-a
  Job-LGHQEI : .NET 10.0.14 (10.0.14, 10.0.1426.52326), Arm64 RyuJIT armv8.0-a

IterationCount=8  WarmupCount=3  

```
| Method                            | N     | Mean             | Error           | StdDev          | Gen0      | Gen1     | Gen2    | Allocated |
|---------------------------------- |------ |-----------------:|----------------:|----------------:|----------:|---------:|--------:|----------:|
| **ImmutableDictionaryAdd**            | **1000**  |   **296,422.238 ns** |   **9,544.4261 ns** |   **4,237.7843 ns** |  **286.1328** |        **-** |       **-** |  **598712 B** |
| ImmutableDictionaryWithBuilderAdd | 1000  |   156,239.530 ns |   1,215.5818 ns |     635.7728 ns |   26.6113 |        - |       - |   56040 B |
| DictionaryAdd                     | 1000  |    11,824.197 ns |     947.1825 ns |     420.5549 ns |   34.4696 |        - |       - |   73168 B |
| ImmutableDictionaryLookup         | 1000  |       128.098 ns |       9.2092 ns |       4.8166 ns |         - |        - |       - |         - |
| DictionaryLookup                  | 1000  |         5.419 ns |       0.4656 ns |       0.2435 ns |         - |        - |       - |         - |
| **ImmutableDictionaryAdd**            | **10000** | **4,897,166.260 ns** | **875,293.4180 ns** | **457,795.3824 ns** | **1851.5625** | **664.0625** |       **-** | **7882552 B** |
| ImmutableDictionaryWithBuilderAdd | 10000 | 2,281,617.900 ns | 305,571.0190 ns | 159,819.5515 ns |  117.1875 |  66.4063 |       - |  560040 B |
| DictionaryAdd                     | 10000 |   392,942.991 ns |  37,012.3263 ns |  19,358.1623 ns |  149.9023 |  53.7109 | 41.0156 |  673218 B |
| ImmutableDictionaryLookup         | 10000 |       110.865 ns |      20.0456 ns |       8.9004 ns |         - |        - |       - |         - |
| DictionaryLookup                  | 10000 |         5.149 ns |       0.0254 ns |       0.0091 ns |         - |        - |       - |         - |
