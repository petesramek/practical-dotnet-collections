using BenchmarkDotNet.Attributes;

namespace PracticalDotNetCollections.Benchmarks.Add;

[MemoryDiagnoser]
public class PriorityQueueBenchmark {
    [Params(10, 100, 1_000, 10_000)]
    public int N;

    [Benchmark]
    public void PriorityQueueEnqueueDequeue() {
        var pq = new PriorityQueue<int, int>();

        for (int i = 0; i < N; i++)
            pq.Enqueue(i, i);

        while (pq.Count > 0)
            pq.Dequeue();
    }

    [Benchmark]
    public void ListSort() {
        var list = new List<int>();

        for (int i = 0; i < N; i++)
            list.Add(i);

        list.Sort();
    }
}
