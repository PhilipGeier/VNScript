namespace VNScript.CodeAnalysis.Syntax;

public enum SyntaxKind
{
    // Tokens
    BadToken,
    EndOfFileToken,
    WhiteSpaceToken,
    IdentifierToken,
    NumberToken,
    PlusToken,
    MinusToken,
    AsteriskToken,
    AsteriskAsteriskToken,
    SlashToken,
    OpenParenthesesToken,
    CloseParenthesesToken,
    BangToken,
    AmpersandAmpersandToken,
    PipePipeToken,
    EqualsEqualsToken,
    BangEqualsToken,
    EqualsToken,
    OpenBraceToken,
    CloseBraceToken,
    LessToken,
    LessOrEqualsToken,
    GreaterToken,
    GreaterOrEqualsToken,
    
    // Keywords
    FalseKeyword,
    TrueKeyword,
    LetKeyword,
    VarKeyword,
    IfKeyword,
    
    // Expressions
    LiteralExpression,
    BinaryExpression,
    ParenthesizedExpression,
    UnaryExpression,
    NameExpression,
    AssignmentExpression,
    
    // Nodes
    CompilationUnit,
    ElseClause,
    
    // Statements
    BlockStatement,
    ExpressionStatement,
    VariableDeclaration,
    IfStatement,
    ElseKeyword
}