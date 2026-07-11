using BenchmarkDotNet.Running;

namespace PracticalDotNetCollections.Benchmarks;

public class Program
{
    public static void Main(string[] args)
    {
        BenchmarkRunner.Run(typeof(Program).Assembly);
    }
}
