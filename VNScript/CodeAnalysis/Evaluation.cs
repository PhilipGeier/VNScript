using System.Resources;
using VNScript.CodeAnalysis.Binding;
using VNScript.CodeAnalysis.Binding.Enums;

namespace VNScript.CodeAnalysis;

internal sealed class Evaluation
{
    private readonly BoundStatement _root;
    private readonly Dictionary<VariableSymbol, object> _variables;
    private object _lastValue = null!;

    public Evaluation(BoundStatement root, Dictionary<VariableSymbol, object> variables)
    {
        _root = root;
        _variables = variables;
    }

    public object Evaluate()
    {
        EvaluateStatement(_root);
        return _lastValue;
    }

    private object EvaluateExpression(BoundExpression node)
    {
        return node.Kind switch
        {
            BoundNodeKind.LiteralExpression => EvaluateLiteralExpression((BoundLiteralExpression)node),
            BoundNodeKind.VariableExpression => EvaluateVariableExpression((BoundVariableExpression)node),
            BoundNodeKind.AssignmentExpression => EvaluateAssignmentExpression((BoundAssignmentExpression)node),
            BoundNodeKind.BinaryExpression => EvaluateBinaryExpression((BoundBinaryExpression)node),
            BoundNodeKind.UnaryExpression => EvaluateUnaryExpression((BoundUnaryExpression)node),
            _ => throw new Exception($"Unexpected Node {node.Kind}")
        };
    }


    private void EvaluateStatement(BoundStatement statement)
    {
        switch (statement.Kind)
        {
            case BoundNodeKind.BlockStatement:
                EvaluateBlockStatement((BoundBlockStatement)statement);
                break;
            case BoundNodeKind.ExpressionStatement:
                EvaluateExpressionStatement((BoundExpressionStatement)statement);
                break;
            case BoundNodeKind.VariableDeclaration:
                EvaluateVariableDeclaration((BoundVariableDeclaration)statement);
                break;
            case BoundNodeKind.IfStatement:
                EvaluateIfStatement((BoundIfStatement)statement);
                break;
            case BoundNodeKind.WhileStatement:
                EvaluateWhileStatement((BoundWhileStatement)statement);
                break;
            case BoundNodeKind.ForStatement:
                EvaluateForStatement((BoundForStatement)statement);
                break;
            default:
                throw new Exception($"Unexpected statement {statement.Kind}");
        }
    }

    #region EvaluateStatement Switch Methods

    private void EvaluateBlockStatement(BoundBlockStatement statement)
    {
        foreach (var st in statement.Statements)
        {
            EvaluateStatement(st);
        }
    }

    private void EvaluateExpressionStatement(BoundExpressionStatement statement)
    {
        _lastValue = EvaluateExpression(statement.Expression);
    }

    private void EvaluateVariableDeclaration(BoundVariableDeclaration statement)
    {
        var value = EvaluateExpression(statement.Initializer);
        _variables[statement.Variable!] = value;
        _lastValue = value;
    }

    private void EvaluateIfStatement(BoundIfStatement statement)
    {
        var condition = (bool)EvaluateExpression(statement.Condition);
        if (condition)
        {
            EvaluateStatement(statement.ThenStatement);
        }
        else if (statement.ElseStatement is not null)
        {
            EvaluateStatement(statement.ElseStatement);
        }
    }

    private void EvaluateWhileStatement(BoundWhileStatement statement)
    {
        while ((bool)EvaluateExpression(statement.Condition))
        {
            EvaluateStatement(statement.Body);
        }
    }
    
    private void EvaluateForStatement(BoundForStatement statement)
    {
        var lowerBound = (int)EvaluateExpression(statement.LowerBound);
        var upperBound = (int)EvaluateExpression(statement.UpperBound);

        for (var i = lowerBound; i <= upperBound; i++)
        {
            _variables[statement.Variable] = i;
            EvaluateStatement(statement.Body);
        }
    }

    #endregion

    private object EvaluateUnaryExpression(BoundUnaryExpression u)
    {
        var operand = EvaluateExpression(u.Operand);

        switch (u.Op.Kind)
        {
            case BoundUnaryOperatorKind.Identity:
                return (int)operand;
            case BoundUnaryOperatorKind.Negation:
                return -(int)operand;
            case BoundUnaryOperatorKind.LogicalNegation:
                return !(bool)operand;
            case BoundUnaryOperatorKind.OnesComplement:
                return ~(int)operand;
            default:
                throw new Exception($"Unexpected unary operator {u.Op.Kind}");
        }
    }

    private object EvaluateBinaryExpression(BoundBinaryExpression b)
    {
        var left = EvaluateExpression(b.Left);
        var right = EvaluateExpression(b.Right);

        switch (b.Op.Kind)
        {
            case BoundBinaryOperatorKind.Addition:
                return (int)left + (int)right;
            case BoundBinaryOperatorKind.Subtraction:
                return (int)left - (int)right;
            case BoundBinaryOperatorKind.Multiplication:
                return (int)left * (int)right;
            case BoundBinaryOperatorKind.Division:
                return (int)left / (int)right;
            case BoundBinaryOperatorKind.LogicalAnd:
                return (bool)left && (bool)right;
            case BoundBinaryOperatorKind.LogicalOr:
                return (bool)left || (bool)right;
            case BoundBinaryOperatorKind.LogicalEquals:
                return Equals(left, right);
            case BoundBinaryOperatorKind.LogicalNotEquals:
                return !Equals(left, right);
            case BoundBinaryOperatorKind.Exponentation:
                return (int)Math.Pow((int)left, (int)right);
            case BoundBinaryOperatorKind.LessThan:
                return (int)left < (int)right;
            case BoundBinaryOperatorKind.LessThanOrEqualTo:
                return (int)left <= (int)right;
            case BoundBinaryOperatorKind.GreaterThan:
                return (int)left > (int)right;
            case BoundBinaryOperatorKind.GreaterThanOrEqualTo:
                return (int)left >= (int)right;
            case BoundBinaryOperatorKind.BitwiseAnd:
                if (b.Type == typeof(int))
                {
                    return (int)left & (int)right;
                }
                return (bool)left & (bool)right;
            case BoundBinaryOperatorKind.BitwiseOr:
                if (b.Type == typeof(int))
                {
                    return (int)left | (int)right;
                }
                return (bool)left | (bool)right;
            case BoundBinaryOperatorKind.BitwiseXor:
                if (b.Type == typeof(int))
                {
                    return (int)left ^ (int)right;
                }
                return (bool)left ^ (bool)right;
            default:
                throw new Exception($"Unexpected binary operator {b.Op.Kind}");
        }
    }

    private object EvaluateAssignmentExpression(BoundAssignmentExpression a)
    {
        var value = EvaluateExpression(a.Expression);
        _variables[a.Variable] = value;
        return value;
    }

    private object EvaluateVariableExpression(BoundVariableExpression v)
    {
        return _variables[v.Variable];
    }

    private static object EvaluateLiteralExpression(BoundLiteralExpression n)
    {
        return n.Value;
    }
}