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

    public Provider ProviderType { get; set; } = default;

    public override string ToString()
    {
        return $"[{Id}@{ProviderType}] -> {Email} ::= {Description}";
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

        var memSe = new SearchEngine<Data>.Builder( new Config<Data>.Builder()
            .AddLogger(logFactory)
            .SetDefaultHandler(_ => false)
            .SetStringRule((d, text) => d.Description.Contains(text))
            
            //rules
            .AddRule(new Rule<Data>.Builder("email").AddDescription("Match with stored email")
                .AddOperator(DirectiveComparisonOperator.Equal, (data, str) => data.Email == str.Value)
                .AddOperator(DirectiveComparisonOperator.Similar, (data, str) => data.Email.Contains(str.Value))
                .Build())
            .AddRule(new Rule<Data>.Builder("id")
                .AddOperator(DirectiveComparisonOperator.Equal, (data, id) => data.Id == id.AsInt)
                .AddOperator(DirectiveNumericOperator.GreaterOrEqual, (data, id) => data.Id >= id.AsInt)
                .AddOperator(DirectiveNumericOperator.Greater, (data, id) => data.Id > id.AsInt)
                .AddOperator(DirectiveNumericOperator.Lesser, (data, id) => data.Id < id.AsInt)
                .AddOperator(DirectiveNumericOperator.LesserOrEqual, (data, id) => data.Id <= id.AsInt)
                .AddOperator((data, lower, upper) => data.Id >= lower.AsInt && data.Id <= upper.AsInt)
                .Build())
            .AddRule(new Rule<Data>.Builder("description")
                .AddOperator(DirectiveComparisonOperator.Equal, (data, str) => data.Description == str.Value)
                .AddOperator(DirectiveComparisonOperator.Similar, (data, str) => data.Description.Contains(str.Value))
                .Build())

            //Commands
            .AddCommand(Command<Data>.AtProvider("internal", (args) => args.SourceQuery.Where(data => data.ProviderType == Data.Provider.Internal)))
            .AddCommand(Command<Data>.AtProvider("external", (args) => args.SourceQuery.Where(data => data.ProviderType == Data.Provider.External)))
            .AddCommand(Command<Data>.AtQuery("take", (args) => {
                var takeCount = (args["count"].Literal as NumericLiteral)!.AsInt;
                return args.SourceQuery.Take(Math.Max(0, takeCount));
            }, new Argument("count", LiteralType.Numeric)))
            .AddCommand(Command<Data>.AtAll("fail", (args) => throw new Exception("Ops...")))
            .Build())
            .AddMemoryProvider(new [] {
                new Data { Id = 1,  Email = "john@email.com", Description = "John Sheppard, some space guy" },
                new Data { Id = 7,  Email = "jane@email.com", Description = "Jane Sheppard, a great explorer" },
                new Data { Id = 9,  Email = "dave@addrs.com", Description = "Dave the viking, he is a very friendly barbarian" },
                new Data { Id = 13, Email = "admin@zone.com", Description = "Always arrives at 1° place in eating contests" }
            }, "staticProvider")
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
                var results = memSe.Query(query);

                Console.WriteLine($"\n\nFound: {results.Count()} for query \"{query}\"\n\n");
                foreach(var res in results){
                    Console.WriteLine(res);
                }
            }
            catch(Exception exp){
                PrintException(exp);
            }
        }
    }
}