﻿using VNScript.CodeAnalysis.Binding.Enums;
using VNScript.CodeAnalysis.Syntax;

namespace VNScript.CodeAnalysis.Binding;

internal sealed class BoundUnaryOperator
{
    public SyntaxKind SyntaxKind { get; }
    public BoundUnaryOperatorKind Kind { get; }
    public Type OperandType { get; }
    public Type ResultType { get; }

    private BoundUnaryOperator(SyntaxKind syntaxKind, BoundUnaryOperatorKind kind, Type operandType)
        :this(syntaxKind, kind, operandType, operandType)
    {
    }
    
    private BoundUnaryOperator(SyntaxKind syntaxKind, BoundUnaryOperatorKind kind, Type operandType, Type resultType)
    {
        SyntaxKind = syntaxKind;
        Kind = kind;
        OperandType = operandType;
        ResultType = resultType;
    }

    private static BoundUnaryOperator[] _operators =
    [
        new BoundUnaryOperator(SyntaxKind.BangToken, BoundUnaryOperatorKind.LogicalNegation, typeof(bool)),
        new BoundUnaryOperator(SyntaxKind.PlusToken, BoundUnaryOperatorKind.Identity, typeof(int)),
        new BoundUnaryOperator(SyntaxKind.MinusToken, BoundUnaryOperatorKind.Negation, typeof(int)),
        new BoundUnaryOperator(SyntaxKind.TildeToken, BoundUnaryOperatorKind.OnesComplement, typeof(int)),
    ];

    public static BoundUnaryOperator? Bind(SyntaxKind kind, Type operandType)
    {
        return _operators.FirstOrDefault(op => op.SyntaxKind == kind && op.OperandType == operandType);
    }
}