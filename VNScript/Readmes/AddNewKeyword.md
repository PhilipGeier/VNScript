# Steps to add a new Keyword
### Make it Lexable
1. Add the Keyword to the `SyntaxKind` enum in `SyntaxKind.cs` file.
2. In `SyntaxFacts.cs`:
    - Add the new keyword to the `GetText` method.
    - Add the new keyword to the `GetKeywordKind` method.

### Make it Parseable
1. Add the case to the switch statement in the method `ParseStatement()` in `Parser.cs`:
2. Create a method to parse the Statement
3. Create a type inheriting of `StatementSyntax`

### Binding
1. Create a type inheriting of `BoundStatement`
2. In `Binder.cs` add case in switch of the method `BindStatement(StatementSyntax)`
    