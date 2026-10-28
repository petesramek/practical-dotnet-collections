```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26300.9711)
virt-10.0 1.00GHz, 1 CPU, 4 logical and 4 physical cores
.NET SDK 10.0.403
  [Host]     : .NET 10.0.14 (10.0.14, 10.0.1426.52326), Arm64 RyuJIT armv8.0-a
  Job-LGHQEI : .NET 10.0.14 (10.0.14, 10.0.1426.52326), Arm64 RyuJIT armv8.0-a

IterationCount=8  WarmupCount=3  

```
| Method                  | N     | Mean      | Error     | StdDev    | Gen0    | Allocated |
|------------------------ |------ |----------:|----------:|----------:|--------:|----------:|
| **StackThroughput**         | **1000**  |  **6.616 μs** | **0.3903 μs** | **0.2041 μs** |  **4.0207** |   **8.23 KB** |
| PreSizedStackThroughput | 1000  |  6.253 μs | 0.4390 μs | 0.2296 μs |  1.9379 |   3.96 KB |
| ListAddRemoveFromEnd    | 1000  |  6.342 μs | 0.8112 μs | 0.4243 μs |  4.0207 |   8.23 KB |
| **StackThroughput**         | **10000** | **67.579 μs** | **4.1846 μs** | **1.8580 μs** | **62.3779** | **128.32 KB** |
| PreSizedStackThroughput | 10000 | 60.444 μs | 2.8204 μs | 1.2523 μs | 18.7988 |  39.12 KB |
| ListAddRemoveFromEnd    | 10000 | 59.482 μs | 1.5605 μs | 0.8162 μs | 62.3779 | 128.32 KB |
