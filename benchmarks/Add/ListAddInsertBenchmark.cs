using BenchmarkDotNet.Attributes;

namespace PracticalDotNetCollections.Benchmarks.Add;

[MemoryDiagnoser]
public class ListAddInsertBenchmark {
    [Params(10, 100, 1_000, 10_000, 100_000)]
    public int N;

    [Benchmark]
    public void Add() {
        var list = new List<int>();
        for (int i = 0; i < N; i++)
            list.Add(i);
    }

    [Benchmark]
    public void InsertAtBeginning() {
        var list = new List<int>();
        for (int i = 0; i < N; i++)
            list.Insert(0, i);
    }
}
