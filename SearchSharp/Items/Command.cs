namespace SearchSharp.Items;

public class Command : QueryItem {
    public class Argument : QueryItem {
        public readonly Literal Literal;

        public Argument(Literal literal){
            Literal = literal;
        }
    }

    public readonly string Identifier;
    public Argument[] Arguments;

    public Command(string identifer, params Argument[] arguments) {
        Identifier = identifer;
        Arguments = arguments ?? Array.Empty<Argument>();
    }
}