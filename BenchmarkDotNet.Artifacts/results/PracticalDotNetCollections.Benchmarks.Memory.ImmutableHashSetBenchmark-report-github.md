```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26300.9711)
virt-10.0 1.00GHz, 1 CPU, 4 logical and 4 physical cores
.NET SDK 10.0.403
  [Host]     : .NET 10.0.14 (10.0.14, 10.0.1426.52326), Arm64 RyuJIT armv8.0-a
  Job-LGHQEI : .NET 10.0.14 (10.0.14, 10.0.1426.52326), Arm64 RyuJIT armv8.0-a

IterationCount=8  WarmupCount=3  

```
| Method                         | N     | Mean             | Error           | StdDev         | Gen0      | Gen1     | Gen2    | Allocated |
|------------------------------- |------ |-----------------:|----------------:|---------------:|----------:|---------:|--------:|----------:|
| **ImmutableHashSetAdd**            | **1000**  |   **284,688.814 ns** |  **11,290.6322 ns** |  **5,013.1107 ns** |  **290.0391** |        **-** |       **-** |  **606712 B** |
| ImmutableHashSetWithBuilderAdd | 1000  |   150,838.743 ns |     984.8929 ns |    515.1181 ns |   26.6113 |        - |       - |   56048 B |
| HashSetAdd                     | 1000  |    10,979.132 ns |   1,215.8938 ns |    635.9360 ns |   27.7710 |        - |       - |   58664 B |
| ImmutableHashSetContains       | 1000  |       165.913 ns |      15.6126 ns |      6.9321 ns |         - |        - |       - |         - |
| HashSetContains                | 1000  |         5.110 ns |       0.1366 ns |      0.0607 ns |         - |        - |       - |         - |
| **ImmutableHashSetAdd**            | **10000** | **4,395,268.107 ns** | **190,099.5616 ns** | **99,425.7465 ns** | **1890.6250** | **640.6250** |       **-** | **7962552 B** |
| ImmutableHashSetWithBuilderAdd | 10000 | 1,994,269.999 ns |  34,724.7836 ns | 18,161.7333 ns |  117.1875 |  66.4063 |       - |  560048 B |
| HashSetAdd                     | 10000 |   311,415.255 ns |  34,588.2102 ns | 18,090.3027 ns |  132.8125 |  44.9219 | 34.1797 |  538660 B |
| ImmutableHashSetContains       | 10000 |       159.373 ns |      11.5409 ns |      6.0361 ns |         - |        - |       - |         - |
| HashSetContains                | 10000 |         5.172 ns |       0.1383 ns |      0.0723 ns |         - |        - |       - |         - |
