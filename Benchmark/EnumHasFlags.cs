using BenchmarkDotNet.Attributes;

namespace Benchmark
{
    public class EnumHasFlags
    {
        private const TestEnum EnumValue = TestEnum.Fourth & TestEnum.Fifth;

        [Benchmark]
        public bool DotNetHasFlagsTrue()
        {
            return EnumValue.HasFlag(TestEnum.Fourth);
        }

        [Benchmark]
        public bool DotNetHasFlagsFalse()
        {
            return EnumValue.HasFlag(TestEnum.First);
        }

        [Benchmark]
        public bool CustomHasFlagsTrue()
        {
            return EnumValue.HasBitFlags(TestEnum.Fourth);
        }

        [Benchmark]
        public bool CustomHasFlagsFalse()
        {
            return EnumValue.HasBitFlags(TestEnum.First);
        }
    }
}