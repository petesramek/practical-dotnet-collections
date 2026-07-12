using BenchmarkDotNet.Attributes;

namespace PracticalDotNetCollections.Benchmarks.Add;

[MemoryDiagnoser]
public class LinkedListAddInsertBenchmark {
    [Params(10, 100, 1_000, 10_000, 100_000)]
    public int N;

    [Benchmark]
    public void ListInsertAtBeginning() {
        var list = new List<int>();
        for (int i = 0; i < N; i++)
            list.Insert(0, i);
    }

    [Benchmark]
    public void LinkedListAddFirst() {
        var list = new LinkedList<int>();
        for (int i = 0; i < N; i++)
            list.AddFirst(i);
    }
}
