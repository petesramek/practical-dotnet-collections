```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26300.9711)
virt-10.0 1.00GHz, 1 CPU, 4 logical and 4 physical cores
.NET SDK 10.0.403
  [Host]     : .NET 10.0.14 (10.0.14, 10.0.1426.52326), Arm64 RyuJIT armv8.0-a
  Job-LGHQEI : .NET 10.0.14 (10.0.14, 10.0.1426.52326), Arm64 RyuJIT armv8.0-a

IterationCount=8  WarmupCount=3  

```
| Method                    | N     | Mean      | Error      | StdDev     | Median    | Gen0    | Gen1    | Gen2    | Allocated |
|-------------------------- |------ |----------:|-----------:|-----------:|----------:|--------:|--------:|--------:|----------:|
| **ConcurrentQueueThroughput** | **10000** |  **3.831 ms** |  **0.8015 ms** |  **0.4192 ms** |  **3.876 ms** |       **-** |       **-** |       **-** |  **70.88 KB** |
| QueueWithLockThroughput   | 10000 | 14.480 ms | 29.3278 ms | 13.0217 ms |  9.678 ms | 46.8750 |       - |       - |  35.14 KB |
| **ConcurrentQueueThroughput** | **50000** | **34.101 ms** | **42.4219 ms** | **22.1875 ms** | **25.491 ms** | **93.7500** | **31.2500** | **31.2500** | **255.74 KB** |
| QueueWithLockThroughput   | 50000 | 13.068 ms |  6.0249 ms |  2.6751 ms | 12.008 ms | 62.5000 |       - |       - | 227.99 KB |
