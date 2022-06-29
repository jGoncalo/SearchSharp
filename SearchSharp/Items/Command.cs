namespace SearchSharp.Items;

public class Command : QueryItem {
    public readonly string Identifier;
    public readonly Directive? Directive;

    public Command(string identifer, Directive? directive) {
        Identifier = identifer;
        Directive = directive;
    }
}