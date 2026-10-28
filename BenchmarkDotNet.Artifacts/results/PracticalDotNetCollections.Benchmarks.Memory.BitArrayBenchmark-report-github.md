```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26300.9711)
virt-10.0 1.00GHz, 1 CPU, 4 logical and 4 physical cores
.NET SDK 10.0.403
  [Host]     : .NET 10.0.14 (10.0.14, 10.0.1426.52326), Arm64 RyuJIT armv8.0-a
  Job-LGHQEI : .NET 10.0.14 (10.0.14, 10.0.1426.52326), Arm64 RyuJIT armv8.0-a

IterationCount=8  WarmupCount=3  

```
| Method                                   | N      | Mean          | Error         | StdDev        | Gen0    | Gen1    | Gen2    | Allocated |
|----------------------------------------- |------- |--------------:|--------------:|--------------:|--------:|--------:|--------:|----------:|
| **CreateBitArray**                           | **10000**  |  **26,359.80 ns** |    **881.031 ns** |    **391.183 ns** |  **0.6104** |       **-** |       **-** |    **1312 B** |
| CreateBoolArray                          | 10000  |   7,180.81 ns |    577.588 ns |    256.453 ns |  4.7836 |       - |       - |   10024 B |
| ReadBitArray                             | 10000  |  11,904.23 ns |    295.513 ns |    131.210 ns |       - |       - |       - |         - |
| ReadBoolArray                            | 10000  |   7,940.32 ns |     94.666 ns |     33.759 ns |       - |       - |       - |         - |
| WriteBitArray                            | 10000  |  26,659.86 ns |  1,606.002 ns |    839.970 ns |       - |       - |       - |         - |
| WriteBoolArray                           | 10000  |   5,744.17 ns |    143.027 ns |     63.505 ns |       - |       - |       - |         - |
| ScenarioPermissionValidationBoolLoop     | 10000  |  10,472.86 ns |    223.630 ns |    116.963 ns |       - |       - |       - |         - |
| ScenarioPermissionValidationBitArrayBulk | 10000  |      90.81 ns |      3.560 ns |      1.580 ns |       - |       - |       - |         - |
| **CreateBitArray**                           | **100000** | **256,227.50 ns** |  **4,012.787 ns** |  **2,098.765 ns** |  **5.8594** |       **-** |       **-** |   **12560 B** |
| CreateBoolArray                          | 100000 | 155,351.06 ns | 12,251.478 ns |  6,407.760 ns | 31.0059 | 31.0059 | 31.0059 |  100034 B |
| ReadBitArray                             | 100000 | 120,033.12 ns |  4,110.078 ns |  2,149.650 ns |       - |       - |       - |         - |
| ReadBoolArray                            | 100000 |  78,132.92 ns |    631.638 ns |    330.359 ns |       - |       - |       - |         - |
| WriteBitArray                            | 100000 | 259,202.48 ns |  7,911.205 ns |  2,821.212 ns |       - |       - |       - |         - |
| WriteBoolArray                           | 100000 |  59,646.11 ns |    397.945 ns |    176.690 ns |       - |       - |       - |         - |
| ScenarioPermissionValidationBoolLoop     | 100000 | 113,220.67 ns | 21,638.137 ns | 11,317.164 ns |       - |       - |       - |         - |
| ScenarioPermissionValidationBitArrayBulk | 100000 |     860.34 ns |     14.915 ns |      7.801 ns |       - |       - |       - |         - |
