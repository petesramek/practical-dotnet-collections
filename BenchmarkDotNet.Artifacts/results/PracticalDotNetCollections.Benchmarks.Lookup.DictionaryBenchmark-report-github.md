```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26300.9711)
virt-10.0 1.00GHz, 1 CPU, 4 logical and 4 physical cores
.NET SDK 10.0.403
  [Host]     : .NET 10.0.14 (10.0.14, 10.0.1426.52326), Arm64 RyuJIT armv8.0-a
  Job-LGHQEI : .NET 10.0.14 (10.0.14, 10.0.1426.52326), Arm64 RyuJIT armv8.0-a

IterationCount=8  WarmupCount=3  

```
| Method                         | N      | Mean         | Error      | StdDev     | Gen0   | Allocated |
|------------------------------- |------- |-------------:|-----------:|-----------:|-------:|----------:|
| **DictionaryLookup**               | **10000**  |     **4.773 ns** |  **0.4031 ns** |  **0.2108 ns** |      **-** |         **-** |
| ListLookup                     | 10000  |   574.105 ns | 15.5259 ns |  8.1203 ns |      - |         - |
| FourLookupUpdate               | 10000  |    13.303 ns |  0.7678 ns |  0.4016 ns |      - |         - |
| TwoLookupTryGetValueUpdate     | 10000  |     8.729 ns |  0.8710 ns |  0.3867 ns |      - |         - |
| SingleLookupGetValueRefUpdate  | 10000  |     5.790 ns |  0.7577 ns |  0.3364 ns |      - |         - |
| LookupWithUnoptimizedStructKey | 10000  |    43.593 ns |  4.3794 ns |  1.9445 ns | 0.0344 |      72 B |
| LookupWithOptimizedStructKey   | 10000  |     5.001 ns |  0.1366 ns |  0.0607 ns |      - |         - |
| **DictionaryLookup**               | **100000** |     **5.580 ns** |  **0.4076 ns** |  **0.1454 ns** |      **-** |         **-** |
| ListLookup                     | 100000 | 5,591.395 ns | 37.0815 ns | 16.4644 ns |      - |         - |
| FourLookupUpdate               | 100000 |    14.965 ns |  0.6439 ns |  0.3368 ns |      - |         - |
| TwoLookupTryGetValueUpdate     | 100000 |    11.327 ns |  2.2877 ns |  1.1965 ns |      - |         - |
| SingleLookupGetValueRefUpdate  | 100000 |     6.432 ns |  0.2850 ns |  0.1265 ns |      - |         - |
| LookupWithUnoptimizedStructKey | 100000 |    45.715 ns |  4.5121 ns |  2.3599 ns | 0.0344 |      72 B |
| LookupWithOptimizedStructKey   | 100000 |     6.471 ns |  0.7380 ns |  0.3277 ns |      - |         - |
