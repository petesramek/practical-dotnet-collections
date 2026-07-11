# BitArray

## What it is

BitArray stores boolean values as individual bits packed into integers, allowing many flags to be stored in a very small memory footprint.

## Typical use cases
- Large sets of flags (feature toggles, permissions, status bits)
- Memory-constrained scenarios

## Internal implementation

Backed by an int[]. Each integer stores 32 boolean values using bit masks. Access requires shifting and masking operations.

## Memory characteristics
- Extremely compact (32 flags per int)
- Much smaller than ool[]
- Slight CPU overhead for bit operations

## Complexity overview

Read: O(1)
Write: O(1)

## Benchmark results

### Scenario

Compare dense vs standard boolean storage:
- BitArray
- ool[]

### How to run this benchmark only

`ash
cd benchmarks
dotnet run -c Release -- --filter *BitArrayBenchmark*
`

### Benchmark code

[benchmarks/Memory/BitArrayBenchmark.cs](../../benchmarks/Memory/BitArrayBenchmark.cs)

### Results

(To be filled after running benchmark)

### Interpretation

(To be filled after running benchmark)

## Practical optimizations
- Use when tracking large number of flags
- Prefer over ool[] when memory matters

## Common mistakes
- Using for small datasets (overkill)
- Expecting same speed as direct array access

## When I would choose it
- Millions of flags
- Memory-sensitive systems

## When I would avoid it
- Small collections → use ool[]

## Rule of thumb

Use BitArray when memory density matters more than raw access speed.
