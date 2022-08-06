using SearchSharp.Attributes;
using SearchSharp.Memory;
using SearchSharp.Engine.Commands;

namespace SearchSharp.Demo;

[Command("skip", EffectiveIn.Query)]
public class SkipCommand : CommandTemplate<Game, MemoryRepository<Game>>
{
    [Argument("count", 0)]
    public int SkipCount { get; set; }
    [Argument("random", 1)]
    public string Ignore { get; set; } = string.Empty;

    public override void Affect(MemoryRepository<Game> repo, EffectiveIn at)
    {
        repo.Apply((query) => query.Skip(SkipCount));
    }
}