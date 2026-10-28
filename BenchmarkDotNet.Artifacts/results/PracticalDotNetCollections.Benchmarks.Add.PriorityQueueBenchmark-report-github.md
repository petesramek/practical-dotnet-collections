```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26300.9711)
virt-10.0 1.00GHz, 1 CPU, 4 logical and 4 physical cores
.NET SDK 10.0.403
  [Host]     : .NET 10.0.14 (10.0.14, 10.0.1426.52326), Arm64 RyuJIT armv8.0-a
  Job-LGHQEI : .NET 10.0.14 (10.0.14, 10.0.1426.52326), Arm64 RyuJIT armv8.0-a

IterationCount=8  WarmupCount=3  

```
| Method                          | N     | Mean        | Error        | StdDev       | Gen0    | Gen1    | Gen2    | Allocated |
|-------------------------------- |------ |------------:|-------------:|-------------:|--------:|--------:|--------:|----------:|
| **PriorityQueueThroughput**         | **1000**  |    **45.43 μs** |     **4.992 μs** |     **2.216 μs** |  **7.9346** |       **-** |       **-** |  **16.23 KB** |
| PreSizedPriorityQueueThroughput | 1000  |    46.52 μs |     2.997 μs |     1.567 μs |  3.8452 |       - |       - |   7.88 KB |
| ListStreamingSort               | 1000  |    92.54 μs |     9.461 μs |     4.201 μs |  3.9063 |       - |       - |    8.2 KB |
| **PriorityQueueThroughput**         | **10000** | **1,093.68 μs** |   **358.277 μs** |   **187.386 μs** | **82.0313** | **41.0156** | **41.0156** | **256.33 KB** |
| PreSizedPriorityQueueThroughput | 10000 |   757.51 μs |    43.659 μs |    19.385 μs | 36.1328 |       - |       - |   78.2 KB |
| ListStreamingSort               | 10000 | 7,407.75 μs | 2,100.426 μs | 1,098.564 μs | 54.6875 |       - |       - | 128.29 KB |
