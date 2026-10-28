```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26300.9711)
virt-10.0 1.00GHz, 1 CPU, 4 logical and 4 physical cores
.NET SDK 10.0.403
  [Host]     : .NET 10.0.14 (10.0.14, 10.0.1426.52326), Arm64 RyuJIT armv8.0-a
  Job-LGHQEI : .NET 10.0.14 (10.0.14, 10.0.1426.52326), Arm64 RyuJIT armv8.0-a

IterationCount=8  WarmupCount=3  

```
| Method              | N     | Mean         | Error       | StdDev      | Gen0    | Allocated |
|-------------------- |------ |-------------:|------------:|------------:|--------:|----------:|
| **Add**                 | **1000**  |     **4.372 μs** |   **0.4840 μs** |   **0.2149 μs** |  **4.0207** |   **8.23 KB** |
| PreSizedCapacityAdd | 1000  |     4.146 μs |   0.8742 μs |   0.4572 μs |  1.9379 |   3.96 KB |
| InsertAtBeginning   | 1000  |    77.772 μs |   9.3019 μs |   4.1301 μs |  3.9063 |    8.2 KB |
| **Add**                 | **10000** |    **45.933 μs** |   **3.1941 μs** |   **1.4182 μs** | **62.4390** | **128.32 KB** |
| PreSizedCapacityAdd | 10000 |    36.292 μs |   3.3583 μs |   1.4911 μs | 18.8599 |  39.12 KB |
| InsertAtBeginning   | 10000 | 7,526.086 μs | 507.7377 μs | 225.4387 μs | 46.8750 | 128.29 KB |
