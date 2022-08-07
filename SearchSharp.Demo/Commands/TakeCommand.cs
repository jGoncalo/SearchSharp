using SearchSharp.Attributes;
using SearchSharp.Engine.Commands;

namespace SearchSharp.Demo.Commands;

[Command("take", EffectiveIn.Provider)]
public class TakeCommand : CommandTemplate<Game, IQueryable<Game>>
{
    [Argument("count", 0)]
    public int Count { get; set; }

    public override IQueryable<Game> Affect(IQueryable<Game> dataSet, EffectiveIn at)
    {
        return dataSet.Take(Math.Max(0, Count));
    }
}