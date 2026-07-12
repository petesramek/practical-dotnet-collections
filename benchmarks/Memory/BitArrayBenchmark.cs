using BenchmarkDotNet.Attributes;
using System.Collections;

namespace PracticalDotNetCollections.Benchmarks;

/// <summary>
/// Benchmarks comparing <see cref="BitArray"/> and <see cref="bool"/> arrays.
/// Evaluates instantiation costs, raw single-element access, and bulk logical set operations.
/// </summary>
[MemoryDiagnoser]
public class BitArrayBenchmark {
    private BitArray _bits = null!;
    private bool[] _bools = null!;

    private BitArray _userPermissions = null!;
    private BitArray _requiredPermissions = null!;
    private BitArray _resultBits = null!;

    private bool[] _userPermissionsBools = null!;
    private bool[] _requiredPermissionsBools = null!;
    private bool[] _resultBools = null!;

    /// <summary>
    /// The number of elements/flags processed in the collection.
    /// </summary>
    [Params(10_000, 100_000)]
    public int N;

    /// <summary>
    /// Allocates and pre-populates all benchmark arrays and bit arrays.
    /// </summary>
    [GlobalSetup]
    public void Setup() {
        _bits = new BitArray(N);
        _bools = new bool[N];

        _userPermissions = new BitArray(N);
        _requiredPermissions = new BitArray(N);
        _resultBits = new BitArray(N);

        _userPermissionsBools = new bool[N];
        _requiredPermissionsBools = new bool[N];
        _resultBools = new bool[N];

        for (int i = 0; i < N; i++) {
            bool isEven = (i % 2 == 0);
            _bits[i] = isEven;
            _bools[i] = isEven;

            bool userHas = (i % 3 == 0);
            bool systemRequires = (i % 5 != 0);

            _userPermissions[i] = userHas;
            _requiredPermissions[i] = systemRequires;

            _userPermissionsBools[i] = userHas;
            _requiredPermissionsBools[i] = systemRequires;
        }
    }

    /// <summary>
    /// Measures memory allocation and setup overhead when instantiating a BitArray.
    /// </summary>
    [Benchmark]
    public BitArray CreateBitArray() {
        var bits = new BitArray(N);
        for (int i = 0; i < N; i++) {
            bits[i] = (i % 2 == 0);
        }
        return bits;
    }

    /// <summary>
    /// Measures memory allocation and setup overhead when instantiating a boolean array.
    /// </summary>
    [Benchmark]
    public bool[] CreateBoolArray() {
        var bools = new bool[N];
        for (int i = 0; i < N; i++) {
            bools[i] = (i % 2 == 0);
        }
        return bools;
    }

    /// <summary>
    /// Measures sequential single-element reading performance from a BitArray.
    /// </summary>
    [Benchmark]
    public int ReadBitArray() {
        int count = 0;
        for (int i = 0; i < N; i++)
            if (_bits[i])
                count++;
        return count;
    }

    /// <summary>
    /// Measures sequential single-element reading performance from a boolean array.
    /// </summary>
    [Benchmark]
    public int ReadBoolArray() {
        int count = 0;
        for (int i = 0; i < N; i++)
            if (_bools[i])
                count++;
        return count;
    }

    /// <summary>
    /// Measures sequential single-element writing performance to a BitArray.
    /// </summary>
    [Benchmark]
    public void WriteBitArray() {
        for (int i = 0; i < N; i++)
            _bits[i] = (i & 1) == 0;
    }

    /// <summary>
    /// Measures sequential single-element writing performance to a boolean array.
    /// </summary>
    [Benchmark]
    public void WriteBoolArray() {
        for (int i = 0; i < N; i++)
            _bools[i] = (i & 1) == 0;
    }

    /// <summary>
    /// Simulates checking permissions by iterating and matching conditions element-by-element.
    /// </summary>
    [Benchmark]
    public bool[] ScenarioPermissionValidationBoolLoop() {
        for (int i = 0; i < N; i++) {
            _resultBools[i] = _userPermissionsBools[i] && _requiredPermissionsBools[i];
        }
        return _resultBools;
    }

    /// <summary>
    /// Simulates checking permissions by performing bulk bitwise operations on full integer blocks.
    /// </summary>
    [Benchmark]
    public BitArray ScenarioPermissionValidationBitArrayBulk() {
        _resultBits.SetAll(false);
        _resultBits.Or(_userPermissions);
        _resultBits.And(_requiredPermissions);
        return _resultBits;
    }
}
