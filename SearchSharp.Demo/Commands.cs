using SearchSharp.Attributes;
using SearchSharp.Engine.Commands;

namespace SearchSharp.Demo;

[Command("skip")]
public class SkipCommand : CommandTemplate<Game>
{
    [CommandArgument("count")]
    public int SkipCount { get; set; }

    public override IQueryable<Game> Affect(IQueryable<Game> query, EffectiveIn at)
    {
        return query.Skip(SkipCount);
    }
}