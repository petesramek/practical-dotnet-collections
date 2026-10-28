```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26300.9711)
virt-10.0 1.00GHz, 1 CPU, 4 logical and 4 physical cores
.NET SDK 10.0.403
  [Host]     : .NET 10.0.14 (10.0.14, 10.0.1426.52326), Arm64 RyuJIT armv8.0-a
  Job-LGHQEI : .NET 10.0.14 (10.0.14, 10.0.1426.52326), Arm64 RyuJIT armv8.0-a

IterationCount=8  WarmupCount=3  

```
| Method                       | N     | Mean               | Error             | StdDev          | Gen0       | Allocated   |
|----------------------------- |------ |-------------------:|------------------:|----------------:|-----------:|------------:|
| **ImmutableArrayAdd**            | **1000**  |    **180,367.8962 ns** |    **32,525.6728 ns** |  **11,598.9682 ns** |   **968.9941** |   **2028000 B** |
| ImmutableArrayWithBuilderAdd | 1000  |      4,052.0361 ns |       380.1993 ns |     198.8516 ns |     5.9128 |     12376 B |
| ListAdd                      | 1000  |      3,718.8740 ns |     1,093.2092 ns |     485.3916 ns |     4.0207 |      8424 B |
| ImmutableArrayRead           | 1000  |          0.8288 ns |         0.0287 ns |       0.0102 ns |          - |           - |
| ListRead                     | 1000  |          1.0646 ns |         0.3124 ns |       0.1387 ns |          - |           - |
| **ImmutableArrayAdd**            | **10000** | **17,142,003.9992 ns** | **1,652,866.3749 ns** | **733,882.9147 ns** | **95000.0000** | **200280000 B** |
| ImmutableArrayWithBuilderAdd | 10000 |     41,501.7023 ns |     3,681.4864 ns |   1,925.4886 ns |    81.0547 |    171352 B |
| ListAdd                      | 10000 |     40,664.1170 ns |     4,870.9400 ns |   2,162.7275 ns |    62.4390 |    131400 B |
| ImmutableArrayRead           | 10000 |          0.8722 ns |         0.1292 ns |       0.0573 ns |          - |           - |
| ListRead                     | 10000 |          0.8254 ns |         0.1120 ns |       0.0497 ns |          - |           - |
