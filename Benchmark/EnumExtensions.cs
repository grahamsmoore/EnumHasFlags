namespace Benchmark
{
    public static class EnumExtensions
    {
        public static bool HasBitFlags(this TestEnum value, TestEnum flag)
        {
            return (value & flag) == flag;
        }
    }
}