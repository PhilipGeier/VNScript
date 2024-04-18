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
    
    // Keywords
    FalseKeyword,
    TrueKeyword,
    
    // Expressions
    LiteralExpression,
    BinaryExpression,
    ParenthesizedExpression,
    UnaryExpression,
    NameExpression,
    AssignmentExpression,
    
    // Nodes
    CompilationUnit,
    
    // Statement
    BlockStatement,
    ExpressionStatement,
}