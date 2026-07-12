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
- **Extremely compact**: Uses exactly 1 bit per flag (32 flags per 4-byte integer), plus minimal object overhead.
- **Substantial footprint savings**: Consumes up to 8x less memory than a standard `bool[]` array, since .NET allocates a full 8-bit byte for every single standalone boolean variable.
- **Slight CPU overhead**: Incurs a minor CPU math cost during individual reads and writes due to the bit-shifting masking math required to find the exact bit.

## Complexity overview

- **Individual Read/Write Access**: Near-instant, requiring only a simple array index lookup followed by bitwise masking math.
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

| Method | N | Mean | Gen 0 | Gen 1 | Gen 2 | Allocated |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| **CreateBitArray** | 10000 | 22,449.74 ns | 0.6104 | - | - | 1,312 B |
| **CreateBoolArray** | 10000 | 5,295.74 ns | 4.7836 | - | - | 10,024 B |
| **ReadBitArray** | 10000 | 9,820.63 ns | - | - | - | - |
| **ReadBoolArray** | 10000 | 4,909.43 ns | - | - | - | - |
| **WriteBitArray** | 10000 | 21,495.52 ns | - | - | - | - |
| **WriteBoolArray** | 10000 | 5,263.31 ns | - | - | - | - |
| **ScenarioPermissionValidationBoolLoop** | 10000 | 8,742.56 ns | - | - | - | - |
| **ScenarioPermissionValidationBitArrayBulk** | 10000 | 75.13 ns | - | - | - | - |
| | | | | | | |
| **CreateBitArray** | 100000 | 216,681.91 ns | 5.8594 | - | - | 12,560 B |
| **CreateBoolArray** | 100000 | 90,868.47 ns | 31.1279 | 31.1279 | 31.1279 | 100,034 B |
| **ReadBitArray** | 100000 | 96,427.83 ns | - | - | - | - |
| **ReadBoolArray** | 100000 | 64,699.84 ns | - | - | - | - |
| **WriteBitArray** | 100000 | 211,771.78 ns | - | - | - | - |
| **WriteBoolArray** | 100000 | 49,970.95 ns | - | - | - | - |
| **ScenarioPermissionValidationBoolLoop** | 100000 | 86,149.80 ns | - | - | - | - |
| **ScenarioPermissionValidationBitArrayBulk** | 100000 | 688.70 ns | - | - | - | - |

### Interpretation

The metrics expose three clear performance realities:

1. **Massive Memory Savings:** At a size of 100,000 items, the standard boolean array requires over 100 KB of heap memory, while the BitArray requires only 12.5 KB. This is a massive 87.4% reduction in memory size. This compaction prevents heavy garbage collection pressure. As seen at 100,000 elements, the boolean array triggers multiple rounds of Gen 0, Gen 1, and Gen 2 garbage collection, while the BitArray covers the exact same number of flags without causing any multi-generation garbage collection activity.
2. **Individual Access Cost:** Modifying or reading independent elements one-by-one is noticeably slower using BitArray. Individual reads take roughly 1.5x longer, and individual writes take up to 4x longer. This happens because finding or altering a specific bit forces the CPU to run bit-shifting and masking math routines on the internal integer structure, whereas a standard boolean array accesses direct byte addresses instantly.
3. **Extreme Bulk Operation Speed:** The real value of BitArray is shown in the PermissionValidation test. At 100,000 items, filtering permissions using a standard loop takes 86,149 ns. Using the bulk BitArray method reduces this to just 688 ns—making it over 125 times faster. Instead of checking indices one by one with slow branching logic, the hardware directly merges whole blocks of 32 flags at the exact same time inside CPU registers.

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
- When raw, unhindered individual element write performance is your absolute highest priority.

## Rule of thumb

Use BitArray when memory density or bulk hardware-accelerated bitwise operations matter more than raw individual element access speed.
