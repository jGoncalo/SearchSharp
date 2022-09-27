# SearchSharp
Is a framework library to allow human redable queries to be transformed to any compliant module and processed in a uniform way.

A simple use case, is a frontend pushing a string that can be executed against an MySQL, PostgressDb, MariaDb, MongoDb, etc in a consistent simple sigle definition case.

# Dynamic Query Language (DQL)

Dynamic query language is the baseline language used by SearchSharp allowing users to query any given data repository with a consistent language. It contains a simple definition for logic operations, wildcase searches and allows for the execution of commands during any step of the query handling process (pre/post porcessing, ex: pagination)

## Primitives

DQL supports three data types as primitives and literals:
- **Numeric** -> Integer and float numbers (possitive and negative), can be used interchangibly, ex:
  - 1
  - 2
  - -1
  - -7
  - 1.2
  - -0.4
- **String** -> Strings bound by quotation marks (**"**), ex:
  - ""
  - "Hello world!"
  - "This is an \\"escape\\" sequence"
- **Boolean** -> Boolean values (case insensitive), ex:
  - True
  - False
  - TRUE
  - false

## Constraints

DQL can be simplified as a subset of constraints and directives, where a constraint is one or more directives.

**Directive** - Are a user defined structure containing:
 - Identifier: a unique identifier used to facilitate debuging and matching rules from a given query
 - Operator: The type of operator (comparison, numeric, range, list)
   - Comparison: are constraints signaled by the operators: 
     - **:** -> **rule** no expected classification with directive argument
     - **~** -> **aproximate** expects some aproximation to directive argument
     - **=** -> **equality** expects to match exactly to directive argument
   - Numeric: are constraints signaled by operators:
     - **>** -> **greater** expects to be greater then directive argument
     - **>=** -> **greater or equal** expects to be greater or equal then directive
     - **<=** -> **lesser or equal** expects to be greater or equal then directive
     - **<** -> **lesser** expects to be greater then directive argument
   - Range: are constraints signaled by operators:
     - **[1..2]** -> **tight range** expects data to be between lower (1) and upper (2) directive arguments
     - **[..2]** -> **upper range** expects data to be lower or equal to upper (2) directive argument
     - **[1..]** -> **lower range** expects data to be lower or equal to upper (1) directive argument
   - List: are constraints bound by an argument list:
     - **[1,2,3,4]** -> Numeric list (float and integer can be mixed)
     - **["simple", "dummy", "invalid"]** -> String list
     - **[True, False, false, true]** -> Boolean list

Some examples for valid syntax in DQL for directives are:
```
email="john@email.com"
name~"John"
profit[..1500]
id[14,1223,4166]
children<2
tax_payed>=1500.44
"Match me!"
```

**NOTE**: there is no constraint on what operator can be used in what actual directive implementation, this is only a guideline for the user.

And a **constraint** is a combination of one or more directive connected via logic operators:
- **&** -> AND logic operator
- **|** -> OR logic operator
- **^** -> XOR logic operator
- **!** -> Negated logic operator

Some examples of valid syntax for DQL constraints are:
```
email~"@money" | role["admin","accountant"]
!email~"debug"
!(email~".gov" | email~".com") and id[..5000]
```

## Configuration

DQL can be loaded into Search engine via fluent api (check wiki for more details), this loading is where directives have their rules matched, this means that the user can create a configuration where the directive:

```
id=13
```
will translate into a C# expression like:
```c#
(data, numLit) => data.Id == numLit.AsInt;
```
this expression will in itself be simplified into
```c#
(data) => data.Id == 13
```
And finnaly feed into the configured provider's repository that can handle how to obtain the data, a simple example would be SearchSharp.EntityFramework that will translate the expression into an IQueryable to eventualy execute the SQL query:
```sql
select * from [dbo].[data] where id = 13;
```

## Query

An DQL query is can have one or more directives, and even some extra data like commands and provider targeting.

The most basic query is:
```
id=13
```

and advance query would be one with multiple directives and logical operators:
```
email~"@money" | role["admin","accountant"]
```

but there is also the possibility to add extra commands and information to a given query (commands are configurable, just like directives)

where the full query syntax is:
```
<engineAlias:providerId> #command() #command(1,2) email~"@money" | role["admin","accountant"]
```
**NOTE** both provider and commands are optional

### Provider
The provider works as a target mechanism for where a query should run (multiple engines and data sources can be configured), as such **engineAlias** specifies what search engine to use while **providerId** specifies what provider to use inside any given engine.

### Commands
Before a directive and after the provider specification, commands can be pushed, these are just like directives and providers loaded by the frameworks user.
These commands work to allow some computation before, or/and after query execution, they are prefixed with the **#** symbol, and can be invoked with or without an argument list:

```
#haveEmailAuthorized
#haveEmailAuthorized()
#skip(12)
#page(1, 20)
#authoredBy(1985, "Carl Sagan")
```

commands are intended for post or pre processing jobs after a query has executed, but they can also be tough of as macros, running specific queries with fewer user input

