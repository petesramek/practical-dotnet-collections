using BenchmarkDotNet.Attributes;

namespace PracticalDotNetCollections.Benchmarks;

[MemoryDiagnoser]
public class QueueDequeueBenchmark {
    [Params(10, 100, 1_000, 10_000, 100_000)]
    public int N;

    [Benchmark]
    public void QueueEnqueueDequeue() {
        var queue = new Queue<int>();

        for (int i = 0; i < N; i++)
            queue.Enqueue(i);

        while (queue.Count > 0)
            queue.Dequeue();
    }

    [Benchmark]
    public void ListRemoveAtBeginning() {
        var list = new List<int>();

        for (int i = 0; i < N; i++)
            list.Add(i);

        while (list.Count > 0)
            list.RemoveAt(0);
    }
}
