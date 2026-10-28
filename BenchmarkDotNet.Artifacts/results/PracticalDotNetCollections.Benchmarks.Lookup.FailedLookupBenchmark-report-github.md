```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26300.9711)
virt-10.0 1.00GHz, 1 CPU, 4 logical and 4 physical cores
.NET SDK 10.0.403
  [Host]     : .NET 10.0.14 (10.0.14, 10.0.1426.52326), Arm64 RyuJIT armv8.0-a
  Job-LGHQEI : .NET 10.0.14 (10.0.14, 10.0.1426.52326), Arm64 RyuJIT armv8.0-a

IterationCount=8  WarmupCount=3  

```
| Method                  | N      | Mean          | Error       | StdDev      | Allocated |
|------------------------ |------- |--------------:|------------:|------------:|----------:|
| **List_FailedLookup**       | **10**     |      **1.519 ns** |   **0.2498 ns** |   **0.1109 ns** |         **-** |
| HashSet_FailedLookup    | 10     |      2.880 ns |   0.1771 ns |   0.0926 ns |         - |
| Dictionary_FailedLookup | 10     |      2.493 ns |   0.0407 ns |   0.0213 ns |         - |
| **List_FailedLookup**       | **100**    |     **18.989 ns** |   **0.3629 ns** |   **0.1898 ns** |         **-** |
| HashSet_FailedLookup    | 100    |      2.490 ns |   0.0541 ns |   0.0283 ns |         - |
| Dictionary_FailedLookup | 100    |      2.676 ns |   0.1694 ns |   0.0752 ns |         - |
| **List_FailedLookup**       | **1000**   |    **155.684 ns** |   **4.1442 ns** |   **1.4779 ns** |         **-** |
| HashSet_FailedLookup    | 1000   |      2.675 ns |   0.1092 ns |   0.0485 ns |         - |
| Dictionary_FailedLookup | 1000   |      2.666 ns |   0.0983 ns |   0.0436 ns |         - |
| **List_FailedLookup**       | **10000**  |  **1,254.955 ns** | **112.9401 ns** |  **59.0698 ns** |         **-** |
| HashSet_FailedLookup    | 10000  |      2.986 ns |   2.5307 ns |   1.1236 ns |         - |
| Dictionary_FailedLookup | 10000  |      2.662 ns |   0.3221 ns |   0.1430 ns |         - |
| **List_FailedLookup**       | **100000** | **12,010.048 ns** | **676.3112 ns** | **353.7238 ns** |         **-** |
| HashSet_FailedLookup    | 100000 |      2.648 ns |   0.6737 ns |   0.3524 ns |         - |
| Dictionary_FailedLookup | 100000 |      2.940 ns |   1.0557 ns |   0.5521 ns |         - |
