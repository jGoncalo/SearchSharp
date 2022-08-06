using SearchSharp.Attributes;
using SearchSharp.Memory;
using SearchSharp.Engine.Commands;

namespace SearchSharp.Demo.Commands;

[Command("skip", EffectiveIn.Provider)]
public class SkipCommand : CommandTemplate<Game, MemoryRepository<Game>>
{
    [Argument("count", 0)]
    public int Count { get; set; }

    public override void Affect(MemoryRepository<Game> repo, EffectiveIn at)
    {
        repo.Apply((query) => query.Skip(Count));
    }
}