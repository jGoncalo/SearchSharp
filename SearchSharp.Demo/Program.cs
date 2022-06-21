using System;
using System.Linq.Expressions;
using SearchSharp.Engine;
using SearchSharp.Engine.Rules;
using SearchSharp.Items;
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
        var se = new SearchEngine<Data>(config => config.SetDefaultHandler(_ => true)
            .SetStringRule((d, text) => d.Description.Contains(text))
            .AddRule(new Rule<Data>.Builder("email")
                .AddStringOperator(DirectiveComparisonOperator.Equal, (data, str) => data.Email == str.Value)
                .AddStringOperator(DirectiveComparisonOperator.Similar, (data, str) => data.Email.Contains(str.Value))
                .Build())
            .AddRule(new Rule<Data>.Builder("id")
                .AddNumericOperator(DirectiveComparisonOperator.Equal, (data, id) => data.Id == id.AsInt)
                .AddRange((data, lower, upper) => data.Id >= (lower == null ? int.MinValue : lower.AsInt) && data.Id <= (upper == null ? int.MaxValue : upper.AsInt))
                .Build())
            .AddRule(new Rule<Data>.Builder("description")
                .AddStringOperator(DirectiveComparisonOperator.Equal, (data, str) => data.Description == str.Value)
                .AddStringOperator(DirectiveComparisonOperator.Similar, (data, str) => data.Description.Contains(str.Value))
                .Build())
            ).AddMemoryProvider(new [] {
                new Data { Id = 1, Email = "john@email.com", Description = "John Sheppard, some space guy" },
                new Data { Id = 7, Email = "jane@email.com", Description = "Jane Sheppard, a great explorer" },
                new Data { Id = 9, Email = "dave@addrs.com", Description = "Dane the viking, he is a very friendly barbarian" }
            }, "staticProvider");

        Console.WriteLine("---SearchEngine---");
        var query = "id=7 | id=9";
        var results = se.Query("staticProvider", query);
        Console.WriteLine($"Found: {results.Count()} for query \"{query}\"");
        foreach(var res in results){
            Console.WriteLine(res);
        }
    }
}