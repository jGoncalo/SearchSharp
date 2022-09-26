using SearchSharp.Engine.Parser.Components.Literals;

namespace SearchSharp.Engine.Commands;

/// <summary>
/// Command specification
/// </summary>
/// <typeparam name="TQueryData">Data type associated with command</typeparam>
public interface ICommand<TQueryData> where TQueryData : QueryData {
    /// <summary>
    /// Unique command identifier
    /// </summary>
    string Identifier { get; }
    /// <summary>
    /// When the command should be run
    /// </summary>
    EffectiveIn EffectAt { get; }
    /// <summary>
    /// What arguments does the command expect (order is important)
    /// </summary>
    ArgumentDeclaration[] Arguments { get; }

    /// <summary>
    /// Applies a literal list to the expected declared arugments
    /// </summary>
    /// <param name="literals">DQL Arguments</param>
    /// <returns>DQL Arugment with specification</returns>
    Argument[] With(params Literal[] literals);
}

/// <summary>
/// Command Specification
/// </summary>
/// <typeparam name="TQueryData">Data type associated with command</typeparam>
/// <typeparam name="TDataStructure">Data structure command will affect</typeparam>
public interface ICommand<TQueryData, TDataStructure> : ICommand<TQueryData>
    where TQueryData : QueryData
    where TDataStructure : class {

    /// <summary>
    /// Effect command will have on Data structure based on a given Parameter list
    /// </summary>
    Func<Parameters<TQueryData, TDataStructure>, TDataStructure> Effect { get; }
}