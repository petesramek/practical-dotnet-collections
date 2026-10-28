```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26300.9711)
virt-10.0 1.00GHz, 1 CPU, 4 logical and 4 physical cores
.NET SDK 10.0.403
  [Host]     : .NET 10.0.14 (10.0.14, 10.0.1426.52326), Arm64 RyuJIT armv8.0-a
  Job-LGHQEI : .NET 10.0.14 (10.0.14, 10.0.1426.52326), Arm64 RyuJIT armv8.0-a

IterationCount=8  WarmupCount=3  

```
| Method                 | N      | Mean           | Error           | StdDev         | Gen0     | Gen1     | Gen2     | Allocated |
|----------------------- |------- |---------------:|----------------:|---------------:|---------:|---------:|---------:|----------:|
| **CreateFrozenDictionary** | **10000**  |  **31,423.650 ns** |   **3,224.6842 ns** |  **1,431.7798 ns** |  **37.7197** |        **-** |        **-** |   **80088 B** |
| FrozenDictionaryLookup | 10000  |       1.489 ns |       0.1038 ns |      0.0543 ns |        - |        - |        - |         - |
| DictionaryLookup       | 10000  |       5.136 ns |       1.2699 ns |      0.6642 ns |        - |        - |        - |         - |
| **CreateFrozenDictionary** | **100000** | **573,612.289 ns** | **110,460.0047 ns** | **57,772.7183 ns** | **191.4063** | **190.4297** | **190.4297** |  **800572 B** |
| FrozenDictionaryLookup | 100000 |       1.442 ns |       0.0511 ns |      0.0267 ns |        - |        - |        - |         - |
| DictionaryLookup       | 100000 |       5.795 ns |       0.5273 ns |      0.1881 ns |        - |        - |        - |         - |
