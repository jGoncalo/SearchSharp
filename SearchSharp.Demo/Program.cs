using SearchSharp.Domain;
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

public class Data {
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
            .With<Data>(engineBuilder => engineBuilder.With(config => config.AddLogger(logFactory)
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
                var results = searchDomain.Search<Data>(query);

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