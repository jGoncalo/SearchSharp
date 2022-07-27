﻿using SearchSharp.Domain;
using SearchSharp.Engine;
using SearchSharp.Engine.Config;
using SearchSharp.Engine.Rules;
using SearchSharp.Engine.Commands;
using SearchSharp.Memory;
using SearchSharp.Demo.EF.Context;
using SearchSharp.Demo.EF.Tables;

using Microsoft.Extensions.Logging;
using Serilog;

using Microsoft.EntityFrameworkCore;
using SearchSharp.Engine.Parser.Components;

namespace SearchSharp.Demo;

public class Data : QueryData {
    public enum Provider {
        Internal = 0,
        External = 1
    }

    public string Email { get; set; } = string.Empty;
    public int Id { get; set; } = 0;
    public string Description { get;set; } = string.Empty;
    public bool HasAuth { get; set; } = false;

    public Provider ProviderType { get; set; } = default;

    public override string ToString()
    {
        return $"[{Id}@{ProviderType}] -> {Email} [hasAuth:{HasAuth}]::= {Description}";
    }
}

public class Game : QueryData {
    [Flags]
    public enum Platform {
        Pc = 0,
        Xbox = 1,
        Playstation = 2,
        Nintendo = 4,
        Apple = 8,
        Android = 16
    }
    
    public string Name { get; set; } = string.Empty;
    public Platform AvailableOn { get; set; } = Platform.Pc;

    public override string ToString() => $"{Name} [{AvailableOn.ToString()}]";
}

internal class Program
{
    static void PrintException(Exception exp) {
        var cur = exp;
        var depth = 1;

        while(cur != null) {
            var tabs = new string('\t', depth);
            Console.WriteLine($"{tabs}->[{cur.GetType().Name}] {cur.Message}");

            depth++;
            cur = cur.InnerException;
        }
    }

    static void Main(string[] args)
    {
        var logFactory = new LoggerFactory()
            .AddSerilog(new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger());

        var searchDomain = new SearchDomain.Builder()
            .With<Data>("user", engineBuilder => engineBuilder.With(config => config.AddLogger(logFactory)
                .SetDefaultHandler(_ => false)
                .SetStringRule((d, text) => d.Description.Contains(text))
                    
                //Rules
                .WithRule("email", ruleSpec => ruleSpec.AddDescription("Match with stored email")
                    .AddOperator<StringLiteral>(DirectiveComparisonOperator.Equal, (data, str) => data.Email == str.Value)
                    .AddOperator<StringLiteral>(DirectiveComparisonOperator.Similar, (data, str) => data.Email.Contains(str.Value)))
                .WithRule("hasAuth", ruleSpec => ruleSpec.AddDescription("Check for user Auth flag")
                    .AddOperator<BooleanLiteral>(DirectiveComparisonOperator.Equal, (data, @bool) => data.HasAuth == (bool) @bool.Value))
                .WithRule("id", ruleSpec => ruleSpec.AddDescription("")
                    .AddOperator<NumericLiteral>(DirectiveComparisonOperator.Equal, (data, id) => data.Id == id.AsInt)
                    .AddOperator(DirectiveNumericOperator.GreaterOrEqual, (data, id) => data.Id >= id.AsInt)
                    .AddOperator(DirectiveNumericOperator.Greater, (data, id) => data.Id > id.AsInt)
                    .AddOperator(DirectiveNumericOperator.Lesser, (data, id) => data.Id < id.AsInt)
                    .AddOperator(DirectiveNumericOperator.LesserOrEqual, (data, id) => data.Id <= id.AsInt)
                    .AddOperator((data, lower, upper) => data.Id >= lower.AsInt && data.Id <= upper.AsInt))
                .WithRule("description", ruleSpec => ruleSpec.AddDescription("Match with stored description")
                    .AddOperator<StringLiteral>(DirectiveComparisonOperator.Equal, (data, str) => data.Description == str.Value)
                    .AddOperator<StringLiteral>(DirectiveComparisonOperator.Similar, (data, str) => data.Description.Contains(str.Value)))
                .WithRule("provider", ruleSpec => ruleSpec.AddDescription("Match with a user provider")
                    .AddOperator<StringLiteral>(DirectiveComparisonOperator.Equal, (data, str) => data.ProviderType == str.AsEnum<Data.Provider>())
                    .AddOperator<NumericLiteral>(DirectiveComparisonOperator.Equal, (data, num) => data.ProviderType == num.AsEnum<Data.Provider>())
                    .AddOperator((data, lower, upper) => lower.AsInt >= (int) data.ProviderType || (int) data.ProviderType <= upper.AsInt))
                
                //Commands
                .WithCommand("internal", commandSpec => commandSpec
                    .SetRuntime(EffectiveIn.Provider)
                    .SetEffect(arg => arg.SourceQuery.Where(d => d.ProviderType == Data.Provider.Internal)))
                .WithCommand("external", commandSpec => commandSpec
                    .SetRuntime(EffectiveIn.Provider)
                    .SetEffect(arg => arg.SourceQuery.Where(d => d.ProviderType == Data.Provider.External)))
                .WithCommand("take", commandSpec => commandSpec
                    .SetRuntime(EffectiveIn.Query)
                    .AddArgument<NumericLiteral>("count")
                    .SetEffect(arg => {
                        var takeCount = (arg["count"].Literal as NumericLiteral)!.AsInt;
                        return arg.SourceQuery.Take(takeCount);
                    })
                )
                .WithCommand("fail", commandSpec => commandSpec
                    .SetRuntime(EffectiveIn.Query | EffectiveIn.Provider)
                    .SetEffect(arg => throw new Exception("Ops..."))))
                
                .AddMemoryProvider(new [] {
                    new Data { Id = 1,  Email = "john@email.com", Description = "John Sheppard, some space guy" },
                    new Data { Id = 7,  Email = "jane@email.com", Description = "Jane Sheppard, a great explorer" },
                    new Data { Id = 9,  Email = "dave@addrs.com", Description = "Dave the viking, he is a very friendly barbarian" },
                    new Data { Id = 13, Email = "admin@zone.com", Description = "Always arrives at 1° place in eating contests" },
                    new Data { Id = 17, Email = "sec@safety.com", Description = "Trusts no one", HasAuth = true},
                    new Data { Id = 18, Email = "zoe@mnstrs.com", Description = "Wraith like teeth", ProviderType = Data.Provider.External},
                    new Data { Id = 22, Email = "liv@safety.com", Description = "PowerOn people!", ProviderType = Data.Provider.External}
                }, "users")
                .AddMemoryProvider(new [] {
                    new Data {Id = 100, Email = "frank@inspection.com", Description = "Hard but fair", HasAuth = true },
                    new Data {Id = 101, Email = "milly@inspection.com", Description = "Demanding", HasAuth = true },
                    new Data {Id = 102, Email = "luis@inspection.com", Description = "Never arrives late", HasAuth = false }
                }, "inspectors"))
            .With<Game>("games", engineBuilder => engineBuilder.With(config => config.AddLogger(logFactory)
                .SetDefaultHandler(_ => false)
                .SetStringRule((data, str) => data.Name.Contains(str))
                
                //Rules
                .WithRule("name", ruleSpec => ruleSpec.AddDescription("Match with stored description")
                    .AddOperator<StringLiteral>(DirectiveComparisonOperator.Equal, (data, str) => data.Name == str.Value)
                    .AddOperator<StringLiteral>(DirectiveComparisonOperator.Similar, (data, str) => data.Name.Contains(str.Value)))
                .WithRule("platform", ruleSpec => ruleSpec.AddDescription("Match with game platforms")
                    .AddOperator<StringLiteral>(DirectiveComparisonOperator.Equal, (data, str) => data.AvailableOn.HasFlag(str.AsEnum<Game.Platform>()))
                    .AddOperator<NumericLiteral>(DirectiveComparisonOperator.Equal, (data, num) => data.AvailableOn.HasFlag(num.AsEnum<Game.Platform>()))
                    .AddOperator((data, lower, upper) => lower.AsInt >= (int) data.AvailableOn || upper.AsInt <= (int) data.AvailableOn)))
                .AddMemoryProvider(new [] {
                    new Game { Name = "Zelda: Breath of the Wild", AvailableOn = Game.Platform.Nintendo },
                    new Game { Name = "Fallout 4", AvailableOn = Game.Platform.Xbox | Game.Platform.Playstation | Game.Platform.Pc },
                    new Game { Name = "Call of Duty - Black Ops 4", AvailableOn = Game.Platform.Xbox | Game.Platform.Playstation | Game.Platform.Pc },
                    new Game { Name = "Halo 5", AvailableOn = Game.Platform.Xbox },
                    new Game { Name = "God Of War", AvailableOn = Game.Platform.Playstation | Game.Platform.Pc },
                    new Game { Name = "Bloodborn", AvailableOn = Game.Platform.Playstation }
                }, "Gen8")
                .AddMemoryProvider(new [] {
                    new Game { Name = "Zelda: Breath of the Wild 2", AvailableOn = Game.Platform.Nintendo },
                    new Game { Name = "Starfield", AvailableOn = Game.Platform.Xbox | Game.Platform.Pc },
                    new Game { Name = "Assassins Creed - Valhalla", AvailableOn = Game.Platform.Xbox | Game.Platform.Playstation | Game.Platform.Pc },
                    new Game { Name = "Halo Infinite", AvailableOn = Game.Platform.Xbox | Game.Platform.Pc },
                    new Game { Name = "Destiny 3", AvailableOn = Game.Platform.Playstation | Game.Platform.Pc },
                    new Game { Name = "God Of War Ragnarok", AvailableOn = Game.Platform.Playstation }
                }, "Gen9"))
                .Build();


        Console.WriteLine("---SearchEngine---");

        while(true){
            Console.WriteLine("Input query:");
            var query = Console.ReadLine();
            if(string.IsNullOrWhiteSpace(query)) {
                Console.WriteLine("No input detected...");
                continue;
            }

            if(query == "quit") break;

            try{
                var results = searchDomain.Search(query);

                Console.WriteLine($"\n\nFound: {results.Total} for query \"{results.Input.Query}\"\n\n");
                foreach(var res in results.Content){
                    Console.WriteLine(res);
                }
            }
            catch(Exception exp){
                PrintException(exp);
            }
        }
    }
}