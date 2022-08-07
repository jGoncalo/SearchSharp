using SearchSharp.Attributes;
using SearchSharp.Engine.Commands;

namespace SearchSharp.Demo.Commands;

[Command("exclusive", EffectiveIn.Query)]
public class ExclusiveCommand : CommandTemplate<Game, IQueryable<Game>>
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

    public override IQueryable<Game> Affect(IQueryable<Game> dataSet, EffectiveIn at)
    {
        return Exclusivity switch {
                ExclusiveType.Pc => dataSet.Where(game => game.AvailableOn == Game.Platform.Pc),
                ExclusiveType.Sony => dataSet.Where(game => game.AvailableOn == Game.Platform.Playstation),
                ExclusiveType.Xbox => dataSet.Where(game => game.AvailableOn == Game.Platform.Xbox),
                ExclusiveType.Nintendo => dataSet.Where(game => game.AvailableOn == Game.Platform.Nintendo),
                ExclusiveType.Android => dataSet.Where(game => game.AvailableOn == Game.Platform.Android),
                ExclusiveType.Apple => dataSet.Where(game => game.AvailableOn == Game.Platform.Apple),

                
                ExclusiveType.Console => dataSet.Where(game => !game.AvailableOn.HasFlag(Game.Platform.Pc)
                    && !game.AvailableOn.HasFlag(Game.Platform.Apple)
                    && !game.AvailableOn.HasFlag(Game.Platform.Android)),
                    
                ExclusiveType.Mobile => dataSet.Where(game => !game.AvailableOn.HasFlag(Game.Platform.Pc)
                    && !game.AvailableOn.HasFlag(Game.Platform.Xbox)
                    && !game.AvailableOn.HasFlag(Game.Platform.Playstation)
                    && !game.AvailableOn.HasFlag(Game.Platform.Nintendo)),

                _ => dataSet
        };
    }
}