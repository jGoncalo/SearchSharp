using SearchSharp.Engine;
using SearchSharp.Engine.Config;
using SearchSharp.Engine.Rules;
using SearchSharp.Memory;

namespace SearchSharp.Demo;

public class Data {
    public string Email { get; set; } = string.Empty;
    public int Id { get; set; } = 0;
    public string Description { get;set; } = string.Empty;

    public override string ToString()
    {
        return $"[{Id}] -> {Email} ::= {Description}";
    }
}

internal class Program
{
    static void Main(string[] args)
    {
        var se = new SearchEngine<Data>( new Config<Data>.Builder()
            .SetDefaultHandler(_ => false)
            .SetStringRule((d, text) => d.Description.Contains(text))
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
            .Build())
            .AddMemoryProvider(new [] {
                new Data { Id = 1,  Email = "john@email.com", Description = "John Sheppard, some space guy" },
                new Data { Id = 7,  Email = "jane@email.com", Description = "Jane Sheppard, a great explorer" },
                new Data { Id = 9,  Email = "dave@addrs.com", Description = "Dave the viking, he is a very friendly barbarian" },
                new Data { Id = 13, Email = "admin@zone.com", Description = "Always arrives at 1° place in eating contests" }
            }, "staticProvider");

        Console.WriteLine("---SearchEngine---");

        while(true){
            Console.WriteLine("Input query:");
            var query = Console.ReadLine();

            if(query == "quit") break;

            var results = se.Query("staticProvider", query);

            Console.WriteLine($"\n\nFound: {results.Count()} for query \"{query}\"\n\n");
            foreach(var res in results){
                Console.WriteLine(res);
            }
        }
    }
}