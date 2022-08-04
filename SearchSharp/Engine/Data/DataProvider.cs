using System.Linq.Expressions;
using SearchSharp.Domain;
using SearchSharp.Engine.Commands;
using SearchSharp.Engine.Commands.Runtime;
using SearchSharp.Exceptions;
using SearchSharp.Engine.Parser.Components;
using SearchSharp.Engine.Data.Repository;

namespace SearchSharp.Engine.Data;

public abstract class DataProviderBuilder<TSelf, TQueryData, TProvider> 
    where TQueryData : QueryData
    where TProvider : DataProvider<TQueryData>
    where TSelf : DataProviderBuilder<TSelf, TQueryData, TProvider> {

    public readonly string Name;
    protected readonly Dictionary<string, ICommand<TQueryData>> Commands;

    protected DataProviderBuilder(string name) {
        Name = name;
        Commands = new Dictionary<string, ICommand<TQueryData>>();
    }

    public TSelf WithCommand(ICommand<TQueryData> command){
        Commands[command.Identifier] = command;
        return (TSelf) this;
    }
    public TSelf WithCommand<TCommandSpec>() where TCommandSpec : CommandTemplate<TQueryData>, new() {
        var templatedCommand = new Command<TQueryData, TCommandSpec>();
        Commands[templatedCommand.Identifier] = templatedCommand;
        return (TSelf) this;
    }
    public TSelf RemoveCommand(string identifier) {
        Commands.Remove(identifier);
        return (TSelf) this;
    }

    public abstract TProvider Build();
}

public abstract class DataProvider<TQueryData> : IDataProvider<TQueryData>
    where TQueryData : QueryData {

    public string Name { get; }

    public IReadOnlyDictionary<string, ICommand<TQueryData>> Commands { get; }

    protected DataProvider(string name, params ICommand<TQueryData>[] commands){
        Name = name;
        Commands = commands.ToDictionary(keySelector: cmd => cmd.Identifier, elementSelector: cmd => cmd);
    }

    protected abstract IDataRepository<TQueryData> GetRepository();

    private void ApplyRules(Query query, IDataRepository<TQueryData> dataRepo, EffectiveIn effectIn){
        foreach(var command in query.Commands) {
            if(Commands.TryGetValue(command.Identifier, out var cmd) && cmd.EffectAt.HasFlag(effectIn)){
                var arguments = cmd.With(command.Arguments.Literals);
                try{
                    cmd.Effect(new Parameters<TQueryData>(effectIn, dataRepo, arguments));
                }
                catch(Exception exp){
                    var argumentStr = arguments.Count() == 0 ? string.Empty : arguments
                        .Select(arg => $"{arg.Identifier}[{arg.Literal.RawValue}]:{arg.Literal.Type}")
                        .Aggregate((left, right) => $"{left},{right}");
                    throw new CommandExecutionException($"Command execution failed: #{cmd.Identifier}({argumentStr})", exp);
                }
            }
        }
    }

    public ISearchResult<TQueryData> ExecuteQuery(Query query, Expression<Func<TQueryData, bool>> expression){
        var repo = GetRepository();
        ApplyRules(query, repo, EffectiveIn.Provider);
        repo.Apply(expression);
        ApplyRules(query, repo, EffectiveIn.Query);
        
        return new SearchResult<TQueryData>{
            Total = repo.Count(),
            Content = repo.Fetch()
        };
    }
}