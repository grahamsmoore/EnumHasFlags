
The `Enum.HasFlag()` method was introduced in .NET 4 to provide an easy way of checking if a bit flag exists for a given value.  A useful addition to the Framework I'm sure you would agree, but may introduce some inherent performance issues in your code.

Check the existence of a bit flag has traditionally be completed with the following code:
```csharp
if ((value & flag) == flag)
```

Now lets take a look at how the `Enum.HasFlag()` method implements the same check:
```csharp
public Boolean HasFlag(Enum flag)
{
    if (!this.GetType().IsEquivalentTo(flag.GetType()))
    {
        throw new ArgumentException(
            Environment.GetResourceString("Argument_EnumTypeDoesNotMatch", flag.GetType(), this.GetType())); 
    }
 
    ulong uFlag = ToUInt64(flag.GetValue()); 
    ulong uThis = ToUInt64(GetValue());
    return ((uThis & uFlag) == uFlag); 
}
```

In the above code we can see multiple issues:
1. There is a type safety check which coverts both the flag and instance to `Enum` types.
2. Boxing occurs when calling `Enum.ToUInt64.GetValue()`, which happens for both the flag and instance values.

### Code
We will test each method of performing a bit flag check to compare the performance differences.  To test this I will be using the excellent [BenchmarkDotNet](https://github.com/PerfDotNet/BenchmarkDotNet) library.  If you haven't used BenchmarkDotNet before, you should definitely check it out, you'll never have to write performance testing boilerplate again!

To test against the Framework method, I created a simple extension method containing the simplfied bit flag check:
```csharp
public static bool HasBitFlags(this TestEnum value, TestEnum flag)
{
    return (value & flag) == flag;
}
```

The code for this performance test can be found in my [GitHub](https://github.com/grahamsmoore/EnumHasFlags) repository.

### Results
The following results are produced by [BenchmarkDotNet](https://github.com/PerfDotNet/BenchmarkDotNet):
```ini
BenchmarkDotNet=v0.9.7.0
OS=Microsoft Windows NT 6.2.9200.0
Processor=Intel(R) Core(TM) i7-6560U CPU 2.20GHz, ProcessorCount=4
Frequency=2156249 ticks, Resolution=463.7683 ns, Timer=TSC
HostCLR=MS.NET 4.0.30319.42000, Arch=64-bit RELEASE [RyuJIT]
JitModules=clrjit-v4.6.1080.0

Type=EnumHasFlags  Mode=Throughput  
```
<table border="1" style="width:auto">
  <tr>
    <th>Method</th>
    <th>Median</th> 
    <th>StdDev</th>
    <th>Gen 0</th>
    <th>Gen 1</th>
    <th>Gen 2</th>
    <th>Bytes Allocated/Op</th>
  </tr>
  <tr>
    <td>DotNetHasFlagsTrue</td>
    <td>23.2160 ns</td> 
    <td>0.4048 ns</td>
    <td>3,092.59</td>
    <td>-</td>
    <td>-</td>
    <td>18.86</td>
  </tr>  
<tr>
    <td>DotNetHasFlagsFalse</td>
    <td>24.0702 ns</td> 
    <td>0.7277 ns</td>
    <td>3,683.00</td>
    <td>-</td>
    <td>-</td>
    <td>22.54</td>
  </tr>
  <tr>
    <td>CustomHasFlagsTrue</td>
    <td>0.0056 ns</td> 
    <td>0.0394 ns</td>
    <td>-</td>
    <td>-</td>
    <td>-</td>
    <td>0.00</td>
  </tr>
  <tr>
    <td>CustomHasFlagsFalse </td>
    <td>0.0007 ns</td> 
    <td>0.0576 ns</td>
    <td>-</td>
    <td>-</td>
    <td>-</td>
    <td>0.00</td>
  </tr>
</table>
### Conclusion
The results clearly show that extra boxing and type-safety check in the Framework-provided `Enum.HasFlag()` cause a massive spike in time taken to execute the check as well as memory allocated.  Although the times and memory sizes are still fairly small, when bit flags are being checked in a hot code path, they could certainly add up enough to cause a large performance issue.