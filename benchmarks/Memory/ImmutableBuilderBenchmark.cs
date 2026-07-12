using BenchmarkDotNet.Attributes;
using System.Collections.Immutable;

namespace PracticalDotNetCollections.Benchmarks.Memory;

[MemoryDiagnoser]
public class ImmutableBuilderBenchmark {
    [Params(10, 100, 1_000, 10_000)]
    public int N;

    [Benchmark]
    public ImmutableArray<int> ImmutableArray_Add_Loop() {
        var array = ImmutableArray<int>.Empty;

        for (int i = 0; i < N; i++)
            array = array.Add(i);

        return array;
    }

    [Benchmark]
    public ImmutableArray<int> ImmutableArray_Builder() {
        var builder = ImmutableArray.CreateBuilder<int>(N);

        for (int i = 0; i < N; i++)
            builder.Add(i);

        return builder.ToImmutable();
    }

    [Benchmark]
    public ImmutableList<int> ImmutableList_Add_Loop() {
        var list = ImmutableList<int>.Empty;

        for (int i = 0; i < N; i++)
            list = list.Add(i);

        return list;
    }
}
