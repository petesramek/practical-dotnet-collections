using BenchmarkDotNet.Attributes;

namespace PracticalDotNetCollections.Benchmarks.Memory;

/// <summary>
/// Compares building a <see cref="List{T}"/> with default capacity versus a preallocated capacity.
/// Each list is allocated inside the measured method so both variants pay for all of their allocations,
/// and it is returned so the JIT cannot eliminate the work.
/// </summary>
[MemoryDiagnoser]
public class ListCapacityBenchmark {
    [Params(10, 100, 1_000, 10_000, 100_000)]
    public int N;

    [Benchmark]
    public List<int> DefaultListAdd() {
        var list = new List<int>();
        for (int i = 0; i < N; i++)
            list.Add(i);
        return list;
    }

    [Benchmark]
    public List<int> PreallocatedListAdd() {
        var list = new List<int>(N);
        for (int i = 0; i < N; i++)
            list.Add(i);
        return list;
    }
}
