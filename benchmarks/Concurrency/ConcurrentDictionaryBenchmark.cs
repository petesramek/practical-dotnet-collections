using BenchmarkDotNet.Attributes;
using System.Collections.Concurrent;

namespace PracticalDotNetCollections.Benchmarks.Concurrency;

/// <summary>
/// Benchmarks evaluating <see cref="ConcurrentDictionary{TKey, TValue}"/> performance against a locked standard <see cref="Dictionary{TKey, TValue}"/>.
/// Measures multi-threaded contention, insertion speeds, and factory lambda closure allocation optimization traps.
/// </summary>
[MemoryDiagnoser]
public class ConcurrentDictionaryBenchmark {
    private ConcurrentDictionary<int, int> _concurrentCache = null!;
    private ConcurrentDictionary<int, int> _stateCache = null!;

    /// <summary>
    /// The number of entries updated or added into the map storage.
    /// </summary>
    [Params(10_000, 50_000)]
    public int N;

    /// <summary>
    /// Sets up pre-populated cache instances for the retrieval and factory allocation benchmarks.
    /// </summary>
    [GlobalSetup]
    public void Setup() {
        _concurrentCache = new ConcurrentDictionary<int, int>();
        _stateCache = new ConcurrentDictionary<int, int>();

        for (int i = 0; i < 50_000; i++) {
            _concurrentCache[i] = i;
            _stateCache[i] = i;
        }
    }

    /// <summary>
    /// Measures thread-safe concurrent element insertions using lock-free bucket structures.
    /// </summary>
    [Benchmark]
    public void ConcurrentDictionaryAdd() {
        var dict = new ConcurrentDictionary<int, int>();
        Parallel.For(0, N, i => {
            dict[i] = i;
        });
    }

    /// <summary>
    /// Measures performance of a standard dictionary utilizing heavy global monitor locking rules across threads.
    /// </summary>
    [Benchmark]
    public void DictionaryAddWithLock() {
        var dict = new Dictionary<int, int>();
        var locker = new object();

        Parallel.For(0, N, i => {
            lock (locker) {
                dict[i] = i;
            }
        });
    }

    /// <summary>
    /// Demonstrates the common mistake: calling GetOrAdd with a lambda referencing external scope variables, which secretly triggers heap closure allocations.
    /// </summary>
    [Benchmark]
    public void GetOrAddWithAllocatingClosure() {
        Parallel.For(0, N, i => {
            int localMultiplier = 2;

            // The reference to localMultiplier forces the compiler to allocate a hidden closure object on the heap
            _concurrentCache.GetOrAdd(i, key => key * localMultiplier);
        });
    }

    /// <summary>
    /// Demonstrates the optimization fix: using the stateful overload of GetOrAdd to pass context data safely with zero heap allocations.
    /// </summary>
    [Benchmark]
    public void GetOrAddWithStatefulAllocationFree() {
        Parallel.For(0, N, i => {
            int localMultiplier = 2;

            // Passing localMultiplier via the state parameter allows the delegate to remain completely allocation-free
            _stateCache.GetOrAdd(i, (key, state) => key * state, localMultiplier);
        });
    }
}
