using BenchmarkDotNet.Attributes;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PracticalDotNetCollections.Benchmarks;

[MemoryDiagnoser]
public class ConcurrentDictionaryBenchmark
{
    private ConcurrentDictionary<int, int> _concurrent = null!;
    private Dictionary<int, int> _dictionary = null!;

    [Params(10, 100, 1_000, 10_000)]
    public int N;

    [GlobalSetup]
    public void Setup()
    {
        _concurrent = new ConcurrentDictionary<int, int>();
        _dictionary = new Dictionary<int, int>();
    }

    [Benchmark]
    public void ConcurrentDictionaryAdd()
    {
        Parallel.For(0, N, i =>
        {
            _concurrent[i] = i;
        });
    }

    [Benchmark]
    public void DictionaryAddWithLock()
    {
        var dict = new Dictionary<int, int>();
        var locker = new object();

        Parallel.For(0, N, i =>
        {
            lock (locker)
            {
                dict[i] = i;
            }
        });
    }
}
