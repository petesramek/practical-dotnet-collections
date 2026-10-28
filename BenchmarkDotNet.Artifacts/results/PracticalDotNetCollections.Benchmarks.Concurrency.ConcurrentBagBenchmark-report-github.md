```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26300.9711)
virt-10.0 1.00GHz, 1 CPU, 4 logical and 4 physical cores
.NET SDK 10.0.403
  [Host]     : .NET 10.0.14 (10.0.14, 10.0.1426.52326), Arm64 RyuJIT armv8.0-a
  Job-LGHQEI : .NET 10.0.14 (10.0.14, 10.0.1426.52326), Arm64 RyuJIT armv8.0-a

IterationCount=8  WarmupCount=3  

```
| Method                      | N     | Mean        | Error     | StdDev    | Gen0     | Gen1     | Gen2     | Allocated |
|---------------------------- |------ |------------:|----------:|----------:|---------:|---------:|---------:|----------:|
| **ThreadLocalConcurrentBag**    | **10000** |    **120.1 μs** |  **16.01 μs** |   **7.11 μs** |  **25.5127** |  **25.3906** |   **3.7842** | **126.07 KB** |
| ThreadLocalListWithLock     | 10000 |    579.4 μs |  81.80 μs |  42.78 μs |  33.2031 |        - |        - |  63.19 KB |
| WorkStealingConcurrentBag   | 10000 |    774.3 μs | 179.01 μs |  93.62 μs |  25.3906 |  24.4141 |   2.9297 | 129.56 KB |
| WorkStealingConcurrentQueue | 10000 |  2,245.0 μs | 167.33 μs |  87.51 μs |  58.5938 |  11.7188 |        - | 132.81 KB |
| **ThreadLocalConcurrentBag**    | **50000** |    **454.1 μs** |  **58.49 μs** |  **30.59 μs** |  **85.9375** |  **83.9844** |  **31.2500** | **508.04 KB** |
| ThreadLocalListWithLock     | 50000 |  3,470.5 μs | 219.05 μs |  97.26 μs | 105.4688 | 105.4688 | 105.4688 | 470.51 KB |
| WorkStealingConcurrentBag   | 50000 |  4,238.2 μs | 666.07 μs | 348.37 μs |  78.1250 |  70.3125 |  15.6250 | 515.01 KB |
| WorkStealingConcurrentQueue | 50000 | 12,437.6 μs | 625.99 μs | 327.40 μs | 109.3750 | 109.3750 | 109.3750 | 517.76 KB |
