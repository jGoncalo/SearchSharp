using System.Linq.Expressions;
using SearchSharp.Engine.Commands;
using SearchSharp.Engine.Repositories;
using SearchSharp.Exceptions;
using SearchSharp.Engine.Parser.Components;

namespace SearchSharp.Engine.Providers;

/// <summary>
/// Provider of data for a given Search Engine
/// Specification of the available commands
/// </summary>
/// <typeparam name="TQueryData">Data type of the provider</typeparam>
/// <typeparam name="TDataRepository">Data repository</typeparam>
/// <typeparam name="TDataStructure">Data Structure of the provider</typeparam>
public class Provider<TQueryData, TDataRepository, TDataStructure> : IProvider<TQueryData>
    where TQueryData : QueryData 
    where TDataStructure : class
    where TDataRepository : IRepository<TQueryData, TDataStructure> {
    /// <summary>
    /// Provider Builder
    /// </summary>
    public class Builder {
        private readonly string Name;

        private IRepositoryFactory<TQueryData, TDataRepository, TDataStructure> _repositoryFactory;

        private readonly Dictionary<string, ICommand<TQueryData, TDataStructure>> _commands = new();

        /// <summary>
        /// Create a provider builder
        /// </summary>
        /// <param name="name">Unique name of the provider</param>
        /// <param name="factory">Repository factory</param>
        public Builder(string name, IRepositoryFactory<TQueryData, TDataRepository,  TDataStructure> factory) {
            Name = name;
            _repositoryFactory = factory;
        }

        #region Commands
        /// <summary>
        /// Register a command
        /// </summary>
        /// <param name="command">Command specification</param>
        /// <returns>This builder</returns>
        public Builder WithCommand(ICommand<TQueryData, TDataStructure> command){
            _commands[command.Identifier] = command;
            return this;
        }
        /// <summary>
        /// Register a command
        /// </summary>
        /// <param name="identifier">Unique command identifier</param>
        /// <param name="config">Action to configure command via builder</param>
        /// <returns>This builder</returns>
        public Builder WithCommand(string identifier, Action<Command<TQueryData, TDataStructure>.Builder> config){
            var builder = Command<TQueryData, TDataStructure>.Builder.For(identifier);
            config(builder);
            _commands[identifier] = builder.Build();
            return this;
        }
        /// <summary>
        /// Register a command
        /// </summary>
        /// <typeparam name="TCommandSpec">Comamnd template type</typeparam>
        /// <returns>This builder</returns>
        public Builder WithCommand<TCommandSpec>() where TCommandSpec : CommandTemplate<TQueryData, TDataStructure>, new() {
            var templatedCommand = new Command<TQueryData, TDataStructure, TCommandSpec>();
            _commands[templatedCommand.Identifier] = templatedCommand;
            return this;
        }
        /// <summary>
        /// Removes a command
        /// </summary>
        /// <param name="identifier">Unique command identifier</param>
        /// <returns>This builder</returns>
        public Builder RemoveCommand(string identifier) {
            _commands.Remove(identifier);
            return this;
        }
        #endregion
    
        /// <summary>
        /// Build a provider
        /// </summary>
        /// <returns>Provider</returns>
        public Provider<TQueryData, TDataRepository, TDataStructure> Build() {
            return new Provider<TQueryData, TDataRepository, TDataStructure>(Name, 
                _repositoryFactory,
                _commands.Values.ToArray());
        }
    }

    /// <summary>
    /// Provider name
    /// </summary>
    public string Name { get; }

    private readonly IRepositoryFactory<TQueryData, TDataRepository, TDataStructure> _repositoryFactory;

    /// <summary>
    /// Get a command by identifier
    /// </summary>
    /// <param name="identifier">Command identifier</param>
    /// <returns>Matching command</returns>
    public ICommand<TQueryData> this[string identifier] {
        get => _commands[identifier] as ICommand<TQueryData>;
    }
    /// <summary>
    /// Try to get a command by identifier
    /// </summary>
    /// <param name="identifier">Command identifier</param>
    /// <param name="command">Matching command</param>
    /// <returns>If a command matching identifier was found</returns>
    public bool TryGet(string identifier, out ICommand<TQueryData>? command){
        var hasCommand = _commands.TryGetValue(identifier, out var targetCommand);
        command = targetCommand as ICommand<TQueryData>;
        return hasCommand;
    }

   private readonly IReadOnlyDictionary<string, ICommand<TQueryData, TDataStructure>> _commands;

    /// <summary>
    /// Create a provider
    /// </summary>
    /// <param name="name">Unique name of the provider</param>
    /// <param name="repositoryFactory">Repository factory</param>
    /// <param name="commands">Registered commands</param>
    public Provider(string name, 
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

    /// <summary>
    /// Obtain results for a given expression, applying the list of commands
    /// </summary>
    /// <param name="expression">Constraint expression for the available data</param>
    /// <param name="commands">Commands to be applyed to data</param>
    /// <returns>Results for the given expression/commands</returns>
    public Result<TQueryData> Get(Expression<Func<TQueryData, bool>>? expression, params Command[] commands){
        return GetAsync(expression, commands).Await();
    }
    /// <summary>
    /// Obtain results for a given expression, applying the list of commands
    /// </summary>
    /// <param name="expression">Constraint expression for the available data</param>
    /// <param name="commands">Commands to be applyed to data</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Results for the given expression/commands</returns>
    public async Task<Result<TQueryData>> GetAsync(Expression<Func<TQueryData, bool>>? expression,
        Command[]? commands = null, 
        CancellationToken ct = default){
        if(ct.IsCancellationRequested) return Result<TQueryData>.Empty;

        var repo = (TDataRepository) _repositoryFactory.Instance();
        var count = await repo.CountAsync(ct);

        if(ct.IsCancellationRequested) return Result<TQueryData>.Empty;
        
        await repo.ModifyAsync(dataSet => ApplyRules(commands ?? Array.Empty<Command>(), dataSet, EffectiveIn.Provider), ct);
        if(ct.IsCancellationRequested) return Result<TQueryData>.Empty;
        if(expression != null) await repo.ApplyAsync(expression, ct);
        if(ct.IsCancellationRequested) return Result<TQueryData>.Empty;
        await repo.ModifyAsync(dataSet => ApplyRules(commands ?? Array.Empty<Command>(), dataSet, EffectiveIn.Query), ct);
        if(ct.IsCancellationRequested) return Result<TQueryData>.Empty;

        var content = await repo.FetchAsync(ct);
        if(ct.IsCancellationRequested) return Result<TQueryData>.Empty;
        
        return new Result<TQueryData>(count, content);
    }
}