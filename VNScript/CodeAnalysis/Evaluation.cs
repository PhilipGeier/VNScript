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
            default:
                throw new Exception($"Unexpected unary operator {u.Op.Kind}");
        }
    }

    private object EvaluateBinaryExpression(BoundBinaryExpression b)
    {
        var left = EvaluateExpression(b.Left);
        var right = EvaluateExpression(b.Right);

        return b.Op.Kind switch
        {
            BoundBinaryOperatorKind.Addition => (int)left + (int)right,
            BoundBinaryOperatorKind.Subtraction => (int)left - (int)right,
            BoundBinaryOperatorKind.Multiplication => (int)left * (int)right,
            BoundBinaryOperatorKind.Division => (int)left / (int)right,
            BoundBinaryOperatorKind.LogicalAnd => (bool)left && (bool)right,
            BoundBinaryOperatorKind.LogicalOr => (bool)left || (bool)right,
            BoundBinaryOperatorKind.LogicalEquals => Equals(left, right),
            BoundBinaryOperatorKind.LogicalNotEquals => !Equals(left, right),
            BoundBinaryOperatorKind.Exponentation => (int)Math.Pow((int)left, (int)right),
            BoundBinaryOperatorKind.LessThan => (int)left < (int)right,
            BoundBinaryOperatorKind.LessThanOrEqualTo => (int)left <= (int)right,
            BoundBinaryOperatorKind.GreaterThan => (int)left > (int)right,
            BoundBinaryOperatorKind.GreaterThanOrEqualTo => (int)left >= (int)right,
            _ => throw new Exception($"Unexpected binary operator {b.Op.Kind}")
        };
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