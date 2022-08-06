using SearchSharp.Attributes;
using SearchSharp.Memory;
using SearchSharp.Engine.Commands;

namespace SearchSharp.Demo.Commands;

[Command("exclusive", EffectiveIn.Query)]
public class ExclusiveCommand : CommandTemplate<Game, MemoryRepository<Game>>
{
    public enum ExclusiveType {
        Pc = 0,
        Console = 1,
        Mobile = 2,
        Sony = 3,
        Xbox = 4,
        Nintendo = 5,
        Android = 6,
        Apple = 7
    }

    [Argument("exclusivity", 0)]
    public ExclusiveType Exclusivity { get; set; }

    public override void Affect(MemoryRepository<Game> repo, EffectiveIn at)
    {
        repo.Apply((query) => Exclusivity switch {
                ExclusiveType.Pc => query.Where(game => game.AvailableOn == Game.Platform.Pc),
                ExclusiveType.Sony => query.Where(game => game.AvailableOn == Game.Platform.Playstation),
                ExclusiveType.Xbox => query.Where(game => game.AvailableOn == Game.Platform.Xbox),
                ExclusiveType.Nintendo => query.Where(game => game.AvailableOn == Game.Platform.Nintendo),
                ExclusiveType.Android => query.Where(game => game.AvailableOn == Game.Platform.Android),
                ExclusiveType.Apple => query.Where(game => game.AvailableOn == Game.Platform.Apple),

                
                ExclusiveType.Console => query.Where(game => !game.AvailableOn.HasFlag(Game.Platform.Pc)
                    && !game.AvailableOn.HasFlag(Game.Platform.Apple)
                    && !game.AvailableOn.HasFlag(Game.Platform.Android)),
                    
                ExclusiveType.Mobile => query.Where(game => !game.AvailableOn.HasFlag(Game.Platform.Pc)
                    && !game.AvailableOn.HasFlag(Game.Platform.Xbox)
                    && !game.AvailableOn.HasFlag(Game.Platform.Playstation)
                    && !game.AvailableOn.HasFlag(Game.Platform.Nintendo)),

                _ => query
            });
    }
}