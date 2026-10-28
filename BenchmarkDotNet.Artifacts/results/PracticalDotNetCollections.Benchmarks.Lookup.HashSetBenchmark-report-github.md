```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26300.9711)
virt-10.0 1.00GHz, 1 CPU, 4 logical and 4 physical cores
.NET SDK 10.0.403
  [Host]     : .NET 10.0.14 (10.0.14, 10.0.1426.52326), Arm64 RyuJIT armv8.0-a
  Job-LGHQEI : .NET 10.0.14 (10.0.14, 10.0.1426.52326), Arm64 RyuJIT armv8.0-a

IterationCount=8  WarmupCount=3  

```
| Method                            | N      | Mean               | Error              | StdDev             | Gen0     | Gen1     | Gen2     | Allocated |
|---------------------------------- |------- |-------------------:|-------------------:|-------------------:|---------:|---------:|---------:|----------:|
| **HashSetLookup**                     | **10000**  |           **4.209 ns** |          **0.1211 ns** |          **0.0538 ns** |        **-** |        **-** |        **-** |         **-** |
| ListLookup                        | 10000  |         611.392 ns |         47.1768 ns |         20.9468 ns |        - |        - |        - |         - |
| ScenarioIntersectionListLoop      | 10000  |   1,347,413.076 ns |     44,182.9656 ns |     23,108.5453 ns |   1.9531 |        - |        - |    4304 B |
| ScenarioIntersectionHashSetFilter | 10000  |      90,045.310 ns |      3,478.8642 ns |      1,819.5132 ns |   1.9531 |        - |        - |    4304 B |
| ScenarioIntersectionHashSetBulk   | 10000  |     564,776.067 ns |     63,781.2607 ns |     33,358.8327 ns |  38.0859 |  38.0859 |  38.0859 |  161821 B |
| **HashSetLookup**                     | **100000** |           **6.931 ns** |          **3.0615 ns** |          **1.6012 ns** |        **-** |        **-** |        **-** |         **-** |
| ListLookup                        | 100000 |       6,837.801 ns |      2,298.2420 ns |      1,202.0250 ns |        - |        - |        - |         - |
| ScenarioIntersectionListLoop      | 100000 | 177,395,053.819 ns | 79,952,184.7570 ns | 41,816,538.5989 ns |        - |        - |        - |   65856 B |
| ScenarioIntersectionHashSetFilter | 100000 |   1,654,388.184 ns |     38,271.3076 ns |     20,016.6339 ns |  29.2969 |        - |        - |   65840 B |
| ScenarioIntersectionHashSetBulk   | 100000 |   5,589,867.350 ns |    349,875.8964 ns |    182,991.8591 ns | 312.5000 | 312.5000 | 312.5000 | 1740547 B |
