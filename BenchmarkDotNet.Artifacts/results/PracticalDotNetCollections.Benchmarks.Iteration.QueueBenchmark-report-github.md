```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26300.9711)
virt-10.0 1.00GHz, 1 CPU, 4 logical and 4 physical cores
.NET SDK 10.0.403
  [Host]     : .NET 10.0.14 (10.0.14, 10.0.1426.52326), Arm64 RyuJIT armv8.0-a
  Job-LGHQEI : .NET 10.0.14 (10.0.14, 10.0.1426.52326), Arm64 RyuJIT armv8.0-a

IterationCount=8  WarmupCount=3  

```
| Method                  | N     | Mean         | Error      | StdDev     | Gen0    | Allocated |
|------------------------ |------ |-------------:|-----------:|-----------:|--------:|----------:|
| **QueueThroughput**         | **1000**  |     **7.410 μs** |  **1.1776 μs** |  **0.5229 μs** |  **4.0283** |   **8.23 KB** |
| PreSizedQueueThroughput | 1000  |     6.686 μs |  0.5207 μs |  0.1857 μs |  1.9379 |   3.97 KB |
| ListRemoveAtBeginning   | 1000  |    84.214 μs | 26.7387 μs | 13.9848 μs |  3.9063 |   8.23 KB |
| **QueueThroughput**         | **10000** |    **61.822 μs** |  **3.4371 μs** |  **1.7976 μs** | **62.3779** | **128.33 KB** |
| PreSizedQueueThroughput | 10000 |    58.729 μs |  8.7286 μs |  3.8755 μs | 18.8599 |  39.13 KB |
| ListRemoveAtBeginning   | 10000 | 5,727.860 μs | 45.5733 μs | 23.8357 μs | 54.6875 | 128.32 KB |
