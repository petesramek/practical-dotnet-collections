using BenchmarkDotNet.Attributes;
using System.Collections;

namespace PracticalDotNetCollections.Benchmarks;

[MemoryDiagnoser]
public class BitArrayBenchmark
{
    private BitArray _bits = null!;
    private bool[] _bools = null!;

    [Params(1_000, 10_000, 100_000)]
    public int N;

    [GlobalSetup]
    public void Setup()
    {
        _bits = new BitArray(N);
        _bools = new bool[N];

        for (int i = 0; i < N; i++)
        {
            _bits[i] = (i % 2 == 0);
            _bools[i] = (i % 2 == 0);
        }
    }

    [Benchmark]
    public int ReadBitArray()
    {
        int count = 0;
        for (int i = 0; i < N; i++)
            if (_bits[i]) count++;
        return count;
    }

    [Benchmark]
    public int ReadBoolArray()
    {
        int count = 0;
        for (int i = 0; i < N; i++)
            if (_bools[i]) count++;
        return count;
    }

    [Benchmark]
    public void WriteBitArray()
    {
        for (int i = 0; i < N; i++)
            _bits[i] = (i & 1) == 0;
    }

    [Benchmark]
    public void WriteBoolArray()
    {
        for (int i = 0; i < N; i++)
            _bools[i] = (i & 1) == 0;
    }
}
