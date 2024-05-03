using System.Resources;
using VNScript.CodeAnalysis.Binding;
using VNScript.CodeAnalysis.Binding.Enums;

namespace VNScript.CodeAnalysis;

internal sealed class Evaluation
{
    private readonly BoundBlockStatement _root;
    private readonly Dictionary<VariableSymbol, object> _variables;
    private object _lastValue = null!;

    public Evaluation(BoundBlockStatement root, Dictionary<VariableSymbol, object> variables)
    {
        _root = root;
        _variables = variables;
    }

    public object Evaluate()
    {
        var labelToIndex = new Dictionary<LabelSymbol, int>();

        for (var i = 0; i < _root.Statements.Length; i++)
        {
            if (_root.Statements[i] is BoundLabelStatement l)
            {
                labelToIndex.Add(l.Symbol, i + 1);
            }
        }

        var index = 0;
        while (index < _root.Statements.Length)
        {
            var s = _root.Statements[index];
            switch (s.Kind)
            {
                case BoundNodeKind.ExpressionStatement:
                    EvaluateExpressionStatement((BoundExpressionStatement)s);
                    index++;
                    break;
                case BoundNodeKind.VariableDeclaration:
                    EvaluateVariableDeclaration((BoundVariableDeclaration)s);
                    index++;
                    break;
                case BoundNodeKind.ConditionalGotoStatement:
                    var cgs = (BoundConditionalGotoStatement)s;
                    var condition = (bool)EvaluateExpression(cgs.Condition);
                    if (condition && !cgs.JumpIfFalse || !condition && cgs.JumpIfFalse)
                    {
                        index = labelToIndex[cgs.Label];
                    }
                    else
                    {
                        index++;
                    }
                    break;
                case BoundNodeKind.GotoStatement:
                    var gs = (BoundGotoStatement)s;
                    index = labelToIndex[gs.Label];
                    break;
                case BoundNodeKind.LabelStatement:
                    index++;
                    break;
                default:
                    throw new Exception($"Unexpected statement {s.Kind}");
            }
        }
        
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

    #region EvaluateStatement Switch Methods
    
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