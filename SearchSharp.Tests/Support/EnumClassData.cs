using System.Collections;

namespace SearchSharp.Tests.Support;

public class EnumClassData<TEnum> : IEnumerable<object[]> where TEnum : struct, Enum
{
    public IEnumerator<object[]> GetEnumerator()
    {        
        foreach(var val in Enum.GetValues<TEnum>()){
            yield return new object[] { val };
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}