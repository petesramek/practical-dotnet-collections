```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26300.9711)
virt-10.0 1.00GHz, 1 CPU, 4 logical and 4 physical cores
.NET SDK 10.0.403
  [Host]     : .NET 10.0.14 (10.0.14, 10.0.1426.52326), Arm64 RyuJIT armv8.0-a
  Job-LGHQEI : .NET 10.0.14 (10.0.14, 10.0.1426.52326), Arm64 RyuJIT armv8.0-a

IterationCount=8  WarmupCount=3  

```
| Method                    | N     | Mean      | Error     | StdDev    | Median    | Gen0     | Gen1     | Allocated  |
|-------------------------- |------ |----------:|----------:|----------:|----------:|---------:|---------:|-----------:|
| **ConcurrentStackThroughput** | **10000** | **10.493 ms** | **21.438 ms** |  **9.519 ms** |  **5.787 ms** | **156.2500** |        **-** |  **316.43 KB** |
| StackWithLockThroughput   | 10000 | 19.315 ms | 21.858 ms |  9.705 ms | 18.023 ms |  46.8750 |        - |   35.51 KB |
| ConcurrentStackBulkRange  | 10000 | 12.899 ms | 12.192 ms |  5.413 ms |  9.054 ms | 203.1250 |        - |  394.42 KB |
| **ConcurrentStackThroughput** | **50000** | **19.561 ms** | **21.304 ms** | **11.142 ms** | **14.815 ms** | **666.6667** |        **-** | **1566.45 KB** |
| StackWithLockThroughput   | 50000 | 13.692 ms |  5.657 ms |  2.017 ms | 13.109 ms |  62.5000 |        - |  219.97 KB |
| ConcurrentStackBulkRange  | 50000 |  7.307 ms |  7.508 ms |  3.334 ms |  6.086 ms | 375.0000 | 312.5000 | 1956.93 KB |
