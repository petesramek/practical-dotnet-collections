```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26300.9711)
virt-10.0 1.00GHz, 1 CPU, 4 logical and 4 physical cores
.NET SDK 10.0.403
  [Host]     : .NET 10.0.14 (10.0.14, 10.0.1426.52326), Arm64 RyuJIT armv8.0-a
  Job-LGHQEI : .NET 10.0.14 (10.0.14, 10.0.1426.52326), Arm64 RyuJIT armv8.0-a

IterationCount=8  WarmupCount=3  

```
| Method                         | N     | Mean         | Error       | StdDev     | Gen0      | Allocated  |
|------------------------------- |------ |-------------:|------------:|-----------:|----------:|-----------:|
| **ThroughputChannel**              | **10000** |     **5.648 ms** |   **0.4680 ms** |  **0.2448 ms** |  **156.2500** |   **316.9 KB** |
| ThroughputBlockingCollection   | 10000 |     7.765 ms |   0.9525 ms |  0.4982 ms |  156.2500 |  294.05 KB |
| SlowConsumerChannel            | 10000 |   261.602 ms | 109.1317 ms | 57.0780 ms |         - |    6.26 KB |
| SlowConsumerBlockingCollection | 10000 |   198.916 ms |  12.7234 ms |  5.6493 ms |  666.6667 | 1711.96 KB |
| **ThroughputChannel**              | **50000** |    **30.885 ms** |   **2.7146 ms** |  **1.4198 ms** |  **750.0000** | **1542.63 KB** |
| ThroughputBlockingCollection   | 50000 |    37.731 ms |   3.0132 ms |  1.3379 ms |  692.3077 | 1448.37 KB |
| SlowConsumerChannel            | 50000 | 1,145.841 ms |  47.3133 ms | 21.0074 ms |         - |    6.32 KB |
| SlowConsumerBlockingCollection | 50000 | 1,032.069 ms | 126.3392 ms | 66.0778 ms | 2000.0000 | 5202.84 KB |
