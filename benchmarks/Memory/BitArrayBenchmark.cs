using BenchmarkDotNet.Attributes;
using System.Collections;

namespace PracticalDotNetCollections.Benchmarks;

[MemoryDiagnoser]
public class BitArrayBenchmark {
    private BitArray _bits = null!;
    private bool[] _bools = null!;

    private bool[] _userPermissionsBools = null!;
    private bool[] _requiredRolesBools = null!;
    private bool[] _resultBools = null!;

    private BitArray _userPermissionsBits = null!;
    private BitArray _requiredRolesBits = null!;
    private BitArray _resultBits = null!;

    [Params(1_000, 10_000, 100_000)]
    public int N;

    [GlobalSetup]
    public void Setup() {
        _bits = new BitArray(N);
        _bools = new bool[N];

        _userPermissionsBools = new bool[N];
        _requiredRolesBools = new bool[N];
        _resultBools = new bool[N];

        _userPermissionsBits = new BitArray(N);
        _requiredRolesBits = new BitArray(N);
        _resultBits = new BitArray(N);

        for (int i = 0; i < N; i++) {
            bool isEven = (i % 2 == 0);
            _bits[i] = isEven;
            _bools[i] = isEven;

            bool hasPermission = (i % 3 == 0);
            bool isRoleRequired = (i % 5 != 0);

            _userPermissionsBools[i] = hasPermission;
            _requiredRolesBools[i] = isRoleRequired;

            _userPermissionsBits[i] = hasPermission;
            _requiredRolesBits[i] = isRoleRequired;
        }
    }


    [Benchmark]
    public BitArray CreateBitArray() {
        var bits = new BitArray(N);
        for (int i = 0; i < N; i++) {
            bits[i] = (i % 2 == 0);
        }
        return bits;
    }

    [Benchmark]
    public bool[] CreateBoolArray() {
        var bools = new bool[N];
        for (int i = 0; i < N; i++) {
            bools[i] = (i % 2 == 0);
        }
        return bools;
    }

    [Benchmark]
    public int ReadBitArray() {
        int count = 0;
        for (int i = 0; i < N; i++)
            if (_bits[i])
                count++;
        return count;
    }

    [Benchmark]
    public int ReadBoolArray() {
        int count = 0;
        for (int i = 0; i < N; i++)
            if (_bools[i])
                count++;
        return count;
    }

    [Benchmark]
    public void WriteBitArray() {
        for (int i = 0; i < N; i++)
            _bits[i] = (i & 1) == 0;
    }

    [Benchmark]
    public void WriteBoolArray() {
        for (int i = 0; i < N; i++)
            _bools[i] = (i & 1) == 0;
    }

    [Benchmark]
    public bool[] PermissionValidationBoolLoop() {
        for (int i = 0; i < N; i++) {
            _resultBools[i] = _userPermissionsBools[i] && _requiredRolesBools[i];
        }
        return _resultBools;
    }

    [Benchmark]
    public BitArray PermissionValidationBitArrayBulk() {
        _resultBits.SetAll(false);
        _resultBits.Or(_userPermissionsBits);
        _resultBits.And(_requiredRolesBits);

        return _resultBits;
    }
}
