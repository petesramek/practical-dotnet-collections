```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26300.9711)
virt-10.0 1.00GHz, 1 CPU, 4 logical and 4 physical cores
.NET SDK 10.0.403
  [Host]     : .NET 10.0.14 (10.0.14, 10.0.1426.52326), Arm64 RyuJIT armv8.0-a
  Job-LGHQEI : .NET 10.0.14 (10.0.14, 10.0.1426.52326), Arm64 RyuJIT armv8.0-a

IterationCount=8  WarmupCount=3  

```
| Method                                | N     | Mean        | Error      | StdDev    | Median       | Gen0      | Allocated  |
|-------------------------------------- |------ |------------:|-----------:|----------:|-------------:|----------:|-----------:|
| **ThroughputConcurrentQueue**             | **10000** |    **13.57 ms** |  **37.975 ms** | **13.542 ms** |     **8.408 ms** |   **31.2500** |   **78.14 KB** |
| ThroughputBlockingCollectionUnbounded | 10000 |    11.35 ms |  10.905 ms |  5.704 ms |     9.945 ms |   46.8750 |   64.07 KB |
| ThroughputBlockingCollectionBounded   | 10000 |    11.61 ms |   4.343 ms |  1.549 ms |    11.710 ms |  171.8750 |  329.87 KB |
| SlowConsumerConcurrentQueue           | 10000 |   293.32 ms |  95.139 ms | 49.760 ms |   283.103 ms |         - |  132.81 KB |
| SlowConsumerBlockingCollection        | 10000 |   210.69 ms |  17.827 ms |  7.915 ms |   211.919 ms |         - | 1511.81 KB |
| **ThroughputConcurrentQueue**             | **50000** |    **40.71 ms** |  **31.343 ms** | **13.917 ms** |    **37.965 ms** |         **-** |  **263.01 KB** |
| ThroughputBlockingCollectionUnbounded | 50000 |    49.52 ms |  16.076 ms |  8.408 ms |    50.141 ms |         - |  264.12 KB |
| ThroughputBlockingCollectionBounded   | 50000 |    33.55 ms |   7.760 ms |  4.059 ms |    34.249 ms | 1000.0000 | 2324.12 KB |
| SlowConsumerConcurrentQueue           | 50000 | 1,127.55 ms |  92.508 ms | 41.074 ms | 1,149.757 ms |         - |  517.73 KB |
| SlowConsumerBlockingCollection        | 50000 | 1,065.13 ms | 143.553 ms | 75.081 ms | 1,029.394 ms | 2000.0000 | 4080.78 KB |
