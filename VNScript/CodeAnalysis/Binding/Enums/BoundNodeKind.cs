﻿namespace VNScript.CodeAnalysis.Binding.Enums;

public enum BoundNodeKind
{
    // Expressions
    UnaryExpression,
    LiteralExpression,
    BinaryExpression,
    VariableExpression,
    AssignmentExpression,
    
    // Statements
    BlockStatement,
    ExpressionStatement
}