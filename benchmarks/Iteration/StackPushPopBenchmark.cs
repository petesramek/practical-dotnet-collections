using BenchmarkDotNet.Attributes;

namespace PracticalDotNetCollections.Benchmarks;

[MemoryDiagnoser]
public class StackPushPopBenchmark {
    [Params(10, 100, 1_000, 10_000, 100_000)]
    public int N;

    [Benchmark]
    public void StackPushPop() {
        var stack = new Stack<int>();

        for (int i = 0; i < N; i++)
            stack.Push(i);

        while (stack.Count > 0)
            stack.Pop();
    }

    [Benchmark]
    public void ListAddRemoveFromEnd() {
        var list = new List<int>();

        for (int i = 0; i < N; i++)
            list.Add(i);

        while (list.Count > 0)
            list.RemoveAt(list.Count - 1);
    }
}
