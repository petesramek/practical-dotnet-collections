```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26300.9711)
virt-10.0 1.00GHz, 1 CPU, 4 logical and 4 physical cores
.NET SDK 10.0.403
  [Host]     : .NET 10.0.14 (10.0.14, 10.0.1426.52326), Arm64 RyuJIT armv8.0-a
  Job-LGHQEI : .NET 10.0.14 (10.0.14, 10.0.1426.52326), Arm64 RyuJIT armv8.0-a

IterationCount=8  WarmupCount=3  

```
| Method            | N     | Mean          | Error          | StdDev         | Gen0    | Allocated |
|------------------ |------ |--------------:|---------------:|---------------:|--------:|----------:|
| **SortedSetAdd**      | **1000**  | **70,559.745 ns** |  **7,091.4642 ns** |  **3,708.9729 ns** | **19.0430** |   **39888 B** |
| HashSetAdd        | 1000  | 21,627.633 ns |  4,993.2547 ns |  2,611.5688 ns | 27.7710 |   58664 B |
| SortedSetContains | 1000  |     45.231 ns |      0.8191 ns |      0.4284 ns |       - |         - |
| HashSetContains   | 1000  |      4.649 ns |      0.1696 ns |      0.0605 ns |       - |         - |
| **SortedSetAdd**      | **10000** | **66,618.298 ns** | **29,551.4230 ns** | **15,455.9656 ns** | **19.0430** |   **39888 B** |
| HashSetAdd        | 10000 | 12,378.080 ns |    490.7115 ns |    256.6516 ns | 27.7710 |   58664 B |
| SortedSetContains | 10000 |     84.225 ns |      5.1229 ns |      2.6794 ns |       - |         - |
| HashSetContains   | 10000 |      4.790 ns |      0.3367 ns |      0.1761 ns |       - |         - |
