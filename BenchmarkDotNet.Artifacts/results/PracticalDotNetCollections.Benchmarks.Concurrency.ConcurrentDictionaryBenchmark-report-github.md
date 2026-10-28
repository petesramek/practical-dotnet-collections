```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26300.9711)
virt-10.0 1.00GHz, 1 CPU, 4 logical and 4 physical cores
.NET SDK 10.0.403
  [Host]     : .NET 10.0.14 (10.0.14, 10.0.1426.52326), Arm64 RyuJIT armv8.0-a
  Job-LGHQEI : .NET 10.0.14 (10.0.14, 10.0.1426.52326), Arm64 RyuJIT armv8.0-a

IterationCount=8  WarmupCount=3  

```
| Method                             | N     | Mean         | Error       | StdDev       | Gen0      | Gen1     | Gen2     | Allocated  |
|----------------------------------- |------ |-------------:|------------:|-------------:|----------:|---------:|---------:|-----------:|
| **ConcurrentDictionaryAdd**            | **10000** |  **2,526.25 μs** |   **587.72 μs** |   **307.389 μs** |  **218.7500** |  **93.7500** |  **31.2500** | **1299.02 KB** |
| DictionaryAddWithLock              | 10000 |    978.41 μs |   249.46 μs |   130.473 μs |  214.8438 |  85.9375 |  82.0313 |  664.96 KB |
| GetOrAddWithAllocatingClosure      | 10000 |    266.57 μs |    31.52 μs |    16.487 μs |  425.2930 |        - |        - |  861.77 KB |
| GetOrAddWithStatefulAllocationFree | 10000 |     46.87 μs |    14.08 μs |     7.364 μs |    1.1597 |        - |        - |    2.33 KB |
| **ConcurrentDictionaryAdd**            | **50000** | **17,689.75 μs** | **3,948.38 μs** | **2,065.078 μs** | **1031.2500** | **687.5000** | **250.0000** | **5796.43 KB** |
| DictionaryAddWithLock              | 50000 |  4,167.43 μs | 1,034.45 μs |   459.302 μs |  570.3125 | 429.6875 | 429.6875 | 2853.34 KB |
| GetOrAddWithAllocatingClosure      | 50000 |  1,500.22 μs |   217.68 μs |   113.852 μs | 2115.2344 |        - |        - |  4299.4 KB |
| GetOrAddWithStatefulAllocationFree | 50000 |  6,618.08 μs | 5,724.10 μs | 2,993.818 μs |         - |        - |        - |    2.35 KB |
