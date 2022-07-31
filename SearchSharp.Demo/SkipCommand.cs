using SearchSharp.Attributes;
using SearchSharp.Engine.Commands;

namespace SearchSharp.Demo;

[Command("skip", EffectiveIn.Query)]
public class SkipCommand : CommandTemplate<Game>
{
    [Argument("count", 0)]
    public int SkipCount { get; set; }
    [Argument("random", 1)]
    public string Ignore { get; set; } = string.Empty;

    public override IQueryable<Game> Affect(IQueryable<Game> query, EffectiveIn at)
    {
        return query.Skip(SkipCount);
    }
}