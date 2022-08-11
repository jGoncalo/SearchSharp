using System.Collections;

namespace SearchSharp.Tests.Support;

public class EnumFlagClassData<TEnum> : IEnumerable<object[]> where TEnum : struct, Enum
{
    public IEnumerator<object[]> GetEnumerator()
    {        
        var minVal = Enum.GetValues<TEnum>().Cast<int>().Min();
        var maxVal = Enum.GetValues<TEnum>().Cast<int>().Min();

        for(var i = minVal; i < maxVal; i++){
            var val = (TEnum) (object) i;
            yield return new object[] { val };
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}