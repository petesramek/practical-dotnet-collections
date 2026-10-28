namespace PracticalDotNetCollections.Benchmarks.Concurrency;

internal static class BenchmarkThreads {
    /// <summary>
    /// Starts work on a dedicated thread so blocking producers/consumers do not depend on thread pool injection.
    /// </summary>
    public static Task Start(Action action)
        => Task.Factory.StartNew(action, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);

    /// <summary>Simulated per-item processing cost shared by every variant so only the collection differs.</summary>
    public const int FastWork = 10;

    /// <summary>Simulated per-item processing cost for the slow consumer scenarios.</summary>
    public const int SlowWork = 500;
}
