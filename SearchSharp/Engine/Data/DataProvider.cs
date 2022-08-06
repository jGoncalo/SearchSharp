using System.Linq.Expressions;
using SearchSharp.Domain;
using SearchSharp.Engine.Commands;
using SearchSharp.Engine.Commands.Runtime;
using SearchSharp.Exceptions;
using SearchSharp.Engine.Parser.Components;

namespace SearchSharp.Engine.Data;

public class DataProvider<TQueryData, TDataRepository> : IDataProvider<TQueryData>
    where TQueryData : QueryData 
    where TDataRepository : IDataRepository<TQueryData> {

    public class Builder {
        private readonly string Name;

        private IDataProviderFactory<TQueryData, TDataRepository> _repositoryFactory;

        private readonly Dictionary<string, ICommand<TQueryData, TDataRepository>> _commands = new();

        public Builder(string name, IDataProviderFactory<TQueryData, TDataRepository> factory) {
            Name = name;
            _repositoryFactory = factory;
        }

        #region Commands
        public Builder WithCommand(ICommand<TQueryData, TDataRepository> command){
            _commands[command.Identifier] = command;
            return this;
        }
        public Builder WithCommand<TCommandSpec>() where TCommandSpec : CommandTemplate<TQueryData, TDataRepository>, new() {
            var templatedCommand = new Command<TQueryData, TDataRepository, TCommandSpec>();
            _commands[templatedCommand.Identifier] = templatedCommand;
            return this;
        }
        public Builder RemoveCommand(string identifier) {
            _commands.Remove(identifier);
            return this;
        }
        #endregion
    
        public DataProvider<TQueryData, TDataRepository> Build() {
            return new DataProvider<TQueryData, TDataRepository>(Name, 
                _repositoryFactory,
                _commands.Values.ToArray());
        }
    }

    public string Name { get; }

    private readonly IDataProviderFactory<TQueryData, TDataRepository> _repositoryFactory;

    public IReadOnlyDictionary<string, ICommand<TQueryData>> Commands => _commands
        .ToDictionary(keySelector: kv => kv.Key, elementSelector: kv => kv.Value as ICommand<TQueryData>);
    private readonly IReadOnlyDictionary<string, ICommand<TQueryData, TDataRepository>> _commands;

    private DataProvider(string name, 
        IDataProviderFactory<TQueryData, TDataRepository> repositoryFactory,
        params ICommand<TQueryData, TDataRepository>[] commands){
        Name = name;

        _repositoryFactory = repositoryFactory;

        _commands = commands.ToDictionary(keySelector: cmd => cmd.Identifier, elementSelector: cmd => cmd);
    }

    private void ApplyRules(Query query, TDataRepository dataRepo, EffectiveIn effectIn){
        foreach(var command in query.Commands) {
            if(_commands.TryGetValue(command.Identifier, out var cmd) && cmd.EffectAt.HasFlag(effectIn)){
                var arguments = cmd.With(command.Arguments.Literals);
                try{
                    cmd.Effect(new Parameters<TQueryData, TDataRepository>(effectIn, dataRepo, arguments));
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
        var repo = (TDataRepository) _repositoryFactory.Instance();
        ApplyRules(query, repo, EffectiveIn.Provider);
        repo.Apply(expression);
        ApplyRules(query, repo, EffectiveIn.Query);
        
        return new SearchResult<TQueryData>{
            Total = repo.Count(),
            Content = repo.Fetch()
        };
    }
}