using BenchmarkDotNet.Attributes;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace PracticalDotNetCollections.Benchmarks.Iteration;

/// <summary>
/// Benchmarks evaluating iteration performance differences between a raw <see cref="List{T}"/> and a wrapped <see cref="ReadOnlyCollection{T}"/>.
/// Measures value-type struct enumeration speeds against interface-bound reference-type heap allocations and index lookups.
/// </summary>
[MemoryDiagnoser]
public class ReadOnlyCollectionBenchmark {
    private List<int> _list = null!;
    private ReadOnlyCollection<int> _readOnly = null!;

    /// <summary>
    /// The number of entries processed inside the active iteration sequences.
    /// </summary>
    [Params(1_000, 10_000)]
    public int N;

    /// <summary>
    /// Sets up and populates the base collections for performance evaluations.
    /// </summary>
    [GlobalSetup]
    public void Setup() {
        _list = Enumerable.Range(0, N).ToList();
        _readOnly = _list.AsReadOnly();
    }

    /// <summary>
    /// Measures iteration speed using List's native value-type struct enumerator.
    /// </summary>
    [Benchmark]
    public int IterateListForeach() {
        int sum = 0;
        foreach (int item in _list) {
            sum += item;
        }
        return sum;
    }

    /// <summary>
    /// Measures the overhead of iterating through a generic interface-bound reference-type enumerator wrapper.
    /// </summary>
    [Benchmark]
    public int IterateReadOnlyCollectionForeach() {
        int sum = 0;
        foreach (int item in _readOnly) {
            sum += item;
        }
        return sum;
    }

    /// <summary>
    /// Measures index-bound loop performance directly hitting raw array bounds.
    /// </summary>
    [Benchmark]
    public int IterateListForIndex() {
        int sum = 0;
        for (int i = 0; i < _list.Count; i++) {
            sum += _list[i];
        }
        return sum;
    }

    /// <summary>
    /// Measures index-bound loop performance traversing a wrapped collection interface boundary.
    /// </summary>
    [Benchmark]
    public int IterateReadOnlyCollectionForIndex() {
        int sum = 0;
        for (int i = 0; i < _readOnly.Count; i++) {
            sum += _readOnly[i];
        }
        return sum;
    }
}
