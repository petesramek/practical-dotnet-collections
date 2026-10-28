```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26300.9711)
virt-10.0 1.00GHz, 1 CPU, 4 logical and 4 physical cores
.NET SDK 10.0.403
  [Host]     : .NET 10.0.14 (10.0.14, 10.0.1426.52326), Arm64 RyuJIT armv8.0-a
  Job-LGHQEI : .NET 10.0.14 (10.0.14, 10.0.1426.52326), Arm64 RyuJIT armv8.0-a

IterationCount=8  WarmupCount=3  

```
| Method                 | N     | Mean           | Error         | StdDev        | Gen0    | Allocated |
|----------------------- |------ |---------------:|--------------:|--------------:|--------:|----------:|
| **SortedDictionaryAdd**    | **1000**  | **120,496.587 ns** | **6,328.6848 ns** | **3,310.0245 ns** | **22.8271** |   **47920 B** |
| DictionaryAdd          | 1000  |  15,828.885 ns | 1,994.1902 ns | 1,043.0001 ns | 34.4696 |   73168 B |
| SortedDictionaryLookup | 1000  |      46.389 ns |     2.6487 ns |     1.3853 ns |       - |         - |
| DictionaryLookup       | 1000  |       4.201 ns |     0.1180 ns |     0.0617 ns |       - |         - |
| **SortedDictionaryAdd**    | **10000** | **116,342.231 ns** | **2,199.6544 ns** |   **976.6602 ns** | **22.8271** |   **47920 B** |
| DictionaryAdd          | 10000 |  14,198.847 ns | 1,146.7838 ns |   599.7901 ns | 34.4696 |   73168 B |
| SortedDictionaryLookup | 10000 |      92.689 ns |    14.0015 ns |     6.2168 ns |       - |         - |
| DictionaryLookup       | 10000 |       4.825 ns |     0.6361 ns |     0.3327 ns |       - |         - |
