using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnostics.Windows;
using BenchmarkDotNet.Running;

namespace Benchmark
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<EnumHasFlags>(
                ManualConfig.Create(DefaultConfig.Instance)
                    .With(new MemoryDiagnoser()));
        }
    }
}