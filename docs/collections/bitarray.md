# BitArray

## What it is

BitArray stores boolean values as individual bits packed tightly into integers, allowing thousands of binary flags to be stored in a very small memory footprint.

## Typical use cases
- Large sets of binary flags (feature toggles, granular permissions, system status bits).
- High-density tracking systems (like bloom filters or tracking processing states for millions of items).
- Memory-constrained or cache-sensitive scenarios.

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

*(To be filled after running benchmark)*

### Interpretation

*(To be filled after running benchmark)*

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
