namespace SearchSharp.Tests;

public static class Extensions {
    public static T Let<T>(this T me, Action<T> let) {
        let(me);
        return me;
    }    
}