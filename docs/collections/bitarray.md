# BitArray

## What it is

BitArray stores boolean values as individual bits packed tightly into integers, allowing thousands of binary flags to be stored in a very small memory footprint.

## Typical use cases
- Large sets of binary flags (feature toggles, granular permissions, system status bits).
- High-density tracking systems (like bloom filters or tracking processing states for millions of items).
- Memory-constrained or cache-sensitive scenarios.

## Sample usage

See:
[samples/bitarray-permission-validation.cs](../../samples/bitarray-permission-validation.cs)

### How to run the sample

```bash
dotnet run samples/bitarray-permission-validation.cs
```

## Internal implementation

Backed internally by a standard `int[]` array. Because a single 32-bit integer handles 32 independent boolean flags using bit masks, access requires bitwise shifting and masking operations under the hood. 

Additionally, the `BitArray` class features built-in bitwise methods (`And`, `Or`, `Xor`, `Not`) that modify the collection in-place. In modern .NET, these bulk operations are highly optimized via SIMD (Single Instruction, Multiple Data), allowing the CPU to process up to 32 or 64 flags in a single hardware instruction cycle.

## Memory characteristics
- **Very compact**: Uses exactly 1 bit per flag (32 flags per 4-byte integer), plus minimal object overhead.
- **Substantial footprint savings**: Consumes up to 8x less memory than a standard `bool[]` array, since .NET allocates a full 8-bit byte for every single standalone boolean variable.
- **Slight CPU overhead**: Incurs a minor CPU math cost during individual reads and writes due to the bit-shifting masking math required to find the exact bit.

## Complexity overview

- **Individual Read/Write Access**: Constant-time, requiring only a simple array index lookup followed by bitwise masking math.
- **Bulk Bitwise Operations (`And`/`Or`/`Xor`)**: Highly parallelized via hardware-level SIMD optimization.

## Benchmark results

### Scenario

Compare dense vs standard boolean storage and manipulation performance across millions of flags:
- BitArray
- bool[]

### How to run this benchmark only

```bash
cd benchmarks
dotnet run -c Release -- --filter *BitArrayBenchmark*
```

### Benchmark code

[benchmarks/Memory/BitArrayBenchmark.cs](../../benchmarks/Memory/BitArrayBenchmark.cs)

### Results

Environment: BenchmarkDotNet 0.15.8; Windows 11 (10.0.26300.9711); virtual CPU `virt-10.0` at 1.00 GHz (1 CPU; 4 logical / 4 physical cores); .NET SDK 10.0.403; .NET 10.0.14, Arm64 RyuJIT (`armv8.0-a`); 3 warmup and 8 measurement iterations. [Full report](../../BenchmarkDotNet.Artifacts/results/PracticalDotNetCollections.Benchmarks.Memory.BitArrayBenchmark-report-github.md).

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

### Interpretation

The metrics expose three clear performance realities:

1. **Memory savings:** At 100,000 items, `CreateBitArray` allocates 12,560 B versus 100,034 B for `CreateBoolArray` (about 87% less). The boolean-array row also records Gen 1 and Gen 2 collections; the BitArray row does not.
2. **Individual access cost:** In this run, reading and writing individual bits took longer than accessing a `bool[]` (120 us vs 78 us for reads; 259 us vs 60 us for writes). BitArray must mask and shift bits, while a boolean array accesses individual bytes.
3. **Bulk operation:** `ScenarioPermissionValidationBitArrayBulk` took 860 ns versus 113 us for the loop, about 130x faster in this run. Bulk bitwise operations process packed values rather than checking each flag separately.

## Practical optimizations
- **Prefer over `bool[]` when memory footprint affects CPU caching**: Minimizing memory size keeps your data packed tightly in the CPU L1/L2 cache, which often speeds up execution more than the bit-shifting math slows it down.
- **Leverage bulk methods**: Whenever you need to compare, invert, or combine sets of flags, use the native `.And()`, `.Or()`, and `.Not()` methods instead of looping through elements individually to take full advantage of hardware acceleration.

## Common mistakes
- **Assuming it is thread-safe**: `BitArray` is completely mutable and lacks internal synchronization. Concurrent writes will corrupt the bit storage.
- **Iterating via `foreach` in performance-critical hot paths**: The `foreach` loop on `BitArray` does not return boolean values directly; it enumerates elements as boxed generic objects, triggering unnecessary heap allocations. Use a standard `for` loop with index access `array[i]` to keep lookups completely allocation-free.
- **Using individual loop updates to merge flag sets**: Writing a manual loop to merge two flag structures together misses out on hardware shortcuts. Always prefer the built-in bitwise methods.

## When I would choose it
- When managing hundreds of thousands or millions of binary flags.
- When memory density dictates application throughput or scaling limits.
- When you frequently perform bulk logical calculations across entire sets of flags.

## When I would avoid it
- When working with small collections of flags (under a few hundred items), where a readable custom `enum` with the `[Flags]` attribute or a simple structure is cleaner.
- When raw, unhindered individual element write performance is your highest priority.

## Rule of thumb

Use BitArray when memory density or bulk hardware-accelerated bitwise operations matter more than raw individual element access speed.
