using System.Linq.Expressions;
using SearchSharp.Engine.Commands;
using SearchSharp.Engine.Repositories;
using SearchSharp.Exceptions;
using SearchSharp.Engine.Parser.Components;

namespace SearchSharp.Engine.Providers;

public class Provider<TQueryData, TDataRepository, TDataStructure> : IProvider<TQueryData>
    where TQueryData : QueryData 
    where TDataStructure : class
    where TDataRepository : IRepository<TQueryData, TDataStructure> {

    public class Builder {
        private readonly string Name;

        private IRepositoryFactory<TQueryData, TDataRepository, TDataStructure> _repositoryFactory;

        private readonly Dictionary<string, ICommand<TQueryData, TDataStructure>> _commands = new();

        public Builder(string name, IRepositoryFactory<TQueryData, TDataRepository,  TDataStructure> factory) {
            Name = name;
            _repositoryFactory = factory;
        }

        #region Commands
        public Builder WithCommand(ICommand<TQueryData, TDataStructure> command){
            _commands[command.Identifier] = command;
            return this;
        }
        public Builder WithCommand(string identifier, Action<Command<TQueryData, TDataStructure>.Builder> config){
            var builder = Command<TQueryData, TDataStructure>.Builder.For(identifier);
            config(builder);
            _commands[identifier] = builder.Build();
            return this;
        }
        public Builder WithCommand<TCommandSpec>() where TCommandSpec : CommandTemplate<TQueryData, TDataStructure>, new() {
            var templatedCommand = new Command<TQueryData, TDataStructure, TCommandSpec>();
            _commands[templatedCommand.Identifier] = templatedCommand;
            return this;
        }
        public Builder RemoveCommand(string identifier) {
            _commands.Remove(identifier);
            return this;
        }
        #endregion
    
        public Provider<TQueryData, TDataRepository, TDataStructure> Build() {
            return new Provider<TQueryData, TDataRepository, TDataStructure>(Name, 
                _repositoryFactory,
                _commands.Values.ToArray());
        }
    }

    public string Name { get; }

    private readonly IRepositoryFactory<TQueryData, TDataRepository, TDataStructure> _repositoryFactory;

    public IReadOnlyDictionary<string, ICommand<TQueryData>> Commands => _commands
        .ToDictionary(keySelector: kv => kv.Key, elementSelector: kv => kv.Value as ICommand<TQueryData>);
    private readonly IReadOnlyDictionary<string, ICommand<TQueryData, TDataStructure>> _commands;

    private Provider(string name, 
        IRepositoryFactory<TQueryData, TDataRepository, TDataStructure> repositoryFactory,
        params ICommand<TQueryData, TDataStructure>[] commands){
        Name = name;

        _repositoryFactory = repositoryFactory;

        _commands = commands.ToDictionary(keySelector: cmd => cmd.Identifier, elementSelector: cmd => cmd);
    }

    private TDataStructure ApplyRules(Command[] commands, TDataStructure dataSet, EffectiveIn effectIn){
        var affectedSet = dataSet;

        foreach(var command in commands) {
            if(_commands.TryGetValue(command.Identifier, out var cmd) && cmd.EffectAt.HasFlag(effectIn)){
                var arguments = cmd.With(command.Arguments.Literals);
                try{
                    affectedSet = cmd.Effect(new Parameters<TQueryData, TDataStructure>(effectIn, dataSet, arguments));
                }
                catch(Exception exp){
                    var argumentStr = arguments.Count() == 0 ? string.Empty : arguments
                        .Select(arg => $"{arg.Identifier}[{arg.Literal.RawValue}]:{arg.Literal.Type}")
                        .Aggregate((left, right) => $"{left},{right}");
                    throw new CommandExecutionException($"Command execution failed: #{cmd.Identifier}({argumentStr})", exp);
                }
            }
        }
        
        return affectedSet;
    }

    public Result<TQueryData> Get(Command[] commands, Expression<Func<TQueryData, bool>>? expression){
        var task = GetAsync(commands, expression);
        Task.WhenAll(task);
        return task.Result;
    }

    public async Task<Result<TQueryData>> GetAsync(Command[] commands, Expression<Func<TQueryData, bool>>? expression,
        CancellationToken ct = default){
        if(ct.IsCancellationRequested) return Result<TQueryData>.Empty;

        var repo = (TDataRepository) _repositoryFactory.Instance();
        var count = await repo.CountAsync(ct);

        if(ct.IsCancellationRequested) return Result<TQueryData>.Empty;
        
        await repo.ModifyAsync(dataSet => ApplyRules(commands, dataSet, EffectiveIn.Provider), ct);
        if(ct.IsCancellationRequested) return Result<TQueryData>.Empty;
        if(expression != null) await repo.ApplyAsync(expression, ct);
        if(ct.IsCancellationRequested) return Result<TQueryData>.Empty;
        await repo.ModifyAsync(dataSet => ApplyRules(commands, dataSet, EffectiveIn.Query), ct);
        if(ct.IsCancellationRequested) return Result<TQueryData>.Empty;

        var content = await repo.FetchAsync(ct);
        if(ct.IsCancellationRequested) return Result<TQueryData>.Empty;
        
        return new Result<TQueryData>(count, await repo.FetchAsync(ct));
    }
}