using BenchmarkDotNet.Attributes;

namespace PracticalDotNetCollections.Benchmarks.Memory;

/// <summary>
/// Compares populating a default <see cref="List{T}"/> with populating one created with a capacity hint.
/// The list is created inside each benchmark so that allocation and resizing are part of the measurement.
/// </summary>
[MemoryDiagnoser]
public class ListCapacityBenchmark {
    [Params(10, 100, 1_000, 10_000, 100_000)]
    public int N;

    [Benchmark(Baseline = true)]
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
