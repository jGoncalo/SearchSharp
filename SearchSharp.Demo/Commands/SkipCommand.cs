using SearchSharp.Attributes;
using SearchSharp.Engine.Commands;

namespace SearchSharp.Demo.Commands;

[Command("skip", EffectiveIn.Provider)]
public class SkipCommand : CommandTemplate<Game, IQueryable<Game>>
{
    [Argument("count", 0)]
    public int Count { get; set; }

    public override IQueryable<Game> Affect(IQueryable<Game> dataSet, EffectiveIn at)
    {
        return dataSet.Skip(Math.Max(0, Count));
    }
}