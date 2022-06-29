using SearchSharp.Engine;
using SearchSharp.Engine.Config;
using SearchSharp.Engine.Rules;
using SearchSharp.Memory;
using SearchSharp.EntityFramework;

using SearchSharp.Demo.EF.Context;
using SearchSharp.Demo.EF.Tables;

using Microsoft.Extensions.Logging;
using Serilog;

using Microsoft.EntityFrameworkCore;

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
        var logFactory = new LoggerFactory()
            .AddSerilog(new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger());

        var memSe = new SearchEngine<Data>.Builder( new Config<Data>.Builder()
            .AddLogger(logFactory)
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
            }, "staticProvider")
            .Build();

        var dbSe = new SearchEngine<UserAccount>.Builder(new Config<UserAccount>.Builder()
            .AddLogger(logFactory)
            .SetDefaultHandler(_ => false)
            .SetStringRule((d, text) => d.Name.Contains(text) || d.Email.Contains(text))
            .AddRule(new Rule<UserAccount>.Builder("email").AddDescription("Match user email")
                .AddOperator(DirectiveComparisonOperator.Equal, (acc, text) => acc.Email == text.Value)
                .AddOperator(DirectiveComparisonOperator.Similar, (acc, text) => acc.Email.Contains(text.Value))
                .Build())
            .Build())
        .AddEntityFrameworkProvider<SimpleContext, UserAccount>(() => SimpleContext.MemoryContext(ctx => {
            ctx.UserAccounts.AddRange(new [] {
                new UserAccount {
                    Id = Guid.NewGuid(),
                    Name = "John Doe",
                    Email = "john@email.com",
                    IsEnabled = false
                },
                new UserAccount {
                    Id = Guid.NewGuid(),
                    Name = "Jane Doe",
                    Email = "jane@email.com",
                    IsEnabled = true,
                    Addresses = new [] {
                        new UserAddress {
                            Id = Guid.NewGuid(),
                            Street = "Some street, number 5",
                            Zip = "5555-555"
                        }
                    }
                },
                new UserAccount {
                    Id = Guid.NewGuid(),
                    Name = "Jeff Doe",
                    Email = "jeff@email.com",
                    IsEnabled = false,
                    Addresses = new [] {
                        new UserAddress {
                            Id = Guid.NewGuid(),
                            Street = "Some aveneu, number 5",
                            Zip = "5555-444"
                        },
                        new UserAddress {
                            Id = Guid.NewGuid(),
                            Street = "Some zone, number 5",
                            Zip = "5555-333"
                        }
                    }
                }
            });
        }), (ctx) => ctx.UserAccounts, "databaseProvider")
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

            var results = dbSe.Query(query, "databaseProvider");

            Console.WriteLine($"\n\nFound: {results.Count()} for query \"{query}\"\n\n");
            foreach(var res in results){
                Console.WriteLine(res);
            }
        }
    }
}