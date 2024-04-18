using VNScript.CodeAnalysis.Binding.Enums;
using VNScript.CodeAnalysis.Syntax;

namespace VNScript.CodeAnalysis.Binding;

internal sealed class BoundBinaryOperator
{
    public SyntaxKind SyntaxKind { get; }
    public BoundBinaryOperatorKind Kind { get; }
    public Type LeftType { get; }
    public Type RightType { get; }
    public Type ResultType { get; }

    private BoundBinaryOperator(SyntaxKind syntaxKind, BoundBinaryOperatorKind kind, Type type)
        : this(syntaxKind, kind, type, type, type)
    {
    }

    private BoundBinaryOperator(SyntaxKind syntaxKind, BoundBinaryOperatorKind kind, Type type, Type resultType)
        : this(syntaxKind, kind, type, type, resultType)
    {
        
    }
    
    private BoundBinaryOperator(SyntaxKind syntaxKind, BoundBinaryOperatorKind kind, Type leftType, Type rightType, Type resultType)
    {
        SyntaxKind = syntaxKind;
        Kind = kind;
        LeftType = leftType;
        RightType = rightType;
        ResultType = resultType;
    }

    private static readonly BoundBinaryOperator[] _operators =
    [
        new BoundBinaryOperator(SyntaxKind.PlusToken, BoundBinaryOperatorKind.Addition, typeof(int)),
        new BoundBinaryOperator(SyntaxKind.MinusToken, BoundBinaryOperatorKind.Subtraction, typeof(int)),
        new BoundBinaryOperator(SyntaxKind.AsteriskToken, BoundBinaryOperatorKind.Multiplication, typeof(int)),
        new BoundBinaryOperator(SyntaxKind.SlashToken, BoundBinaryOperatorKind.Division, typeof(int)),
        new BoundBinaryOperator(SyntaxKind.AmpersandAmpersandToken, BoundBinaryOperatorKind.LogicalAnd, typeof(bool)),
        new BoundBinaryOperator(SyntaxKind.PipePipeToken, BoundBinaryOperatorKind.LogicalOr, typeof(bool)),
        
        new BoundBinaryOperator(SyntaxKind.AsteriskAsteriskToken, BoundBinaryOperatorKind.Exponentation, typeof(int)),
        
        new BoundBinaryOperator(SyntaxKind.EqualsEqualsToken, BoundBinaryOperatorKind.LogicalEquals, typeof(bool)),
        new BoundBinaryOperator(SyntaxKind.BangEqualsToken, BoundBinaryOperatorKind.LogicalNotEquals, typeof(bool)),
        
        new BoundBinaryOperator(SyntaxKind.EqualsEqualsToken, BoundBinaryOperatorKind.LogicalEquals, typeof(int), typeof(bool)),
        new BoundBinaryOperator(SyntaxKind.BangEqualsToken, BoundBinaryOperatorKind.LogicalNotEquals, typeof(int), typeof(bool)),
    ];

    public static BoundBinaryOperator? Bind(SyntaxKind kind, Type leftType, Type rightType)
    {
        return _operators.FirstOrDefault(op => op.SyntaxKind == kind && op.LeftType == leftType && op.RightType == rightType);
    }
}