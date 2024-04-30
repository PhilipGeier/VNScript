using System.Collections.Immutable;
using System.ComponentModel;
using VNScript.CodeAnalysis.Syntax;

namespace VNScript.CodeAnalysis.Binding;

internal sealed class Binder
{
    private BoundScope _scope;
    public DiagnosticBag Diagnostics { get; } = [];

    public Binder(BoundScope? parent)
    {
        _scope = new BoundScope(parent);
    }

    private BoundExpression BindExpression(ExpressionSyntax syntax)
    {
        switch (syntax.Kind)
        {
            case SyntaxKind.ParenthesizedExpression:
                return BindParenthesizedExpression((ParenthesizedExpressionSyntax)syntax);
            case SyntaxKind.LiteralExpression:
                return BindLiteralExpression((LiteralExpressionSyntax)syntax);
            case SyntaxKind.NameExpression:
                return BindNameExpression((NameExpressionSyntax)syntax);
            case SyntaxKind.AssignmentExpression:
                return BindAssignmentExpression((AssignmentExpressionSyntax)syntax);
            case SyntaxKind.UnaryExpression:
                return BindUnaryExpression((UnaryExpressionSyntax)syntax);
            case SyntaxKind.BinaryExpression:
                return BindBinaryExpression((BinaryExpressionSyntax)syntax);
            default:
                throw new Exception($"Unexpected syntax {syntax.Kind}");
        }
    }

    private BoundExpression BindExpression(ExpressionSyntax syntax, Type targetType)
    {
        var result = BindExpression(syntax);

        if (result.Type != targetType)
        {
            Diagnostics.ReportCannotConvert(syntax.Span, result.Type, targetType);
        }

        return result;
    }

    private BoundStatement BindStatement(StatementSyntax syntax) =>
        syntax.Kind switch
        {
            SyntaxKind.BlockStatement => BindBlockStatement((BlockStatementSyntax)syntax),
            SyntaxKind.ExpressionStatement => BindExpressionStatement((ExpressionStatementSyntax)syntax),
            SyntaxKind.VariableDeclaration => BindVariableDeclaration((VariableDeclarationSyntax)syntax),
            SyntaxKind.IfStatement => BindIfStatement((IfStatementSyntax)syntax),
            SyntaxKind.WhileStatement => BindWhileStatement((WhileStatementSyntax)syntax),
            SyntaxKind.ForStatement => BindForStatement((ForStatementSyntax)syntax),
            _ => throw new Exception($"Unexpected syntax {syntax.Kind}")
        };

    #region BindStatement Switch Methods

    private BoundStatement BindBlockStatement(BlockStatementSyntax syntax)
    {
        var statements = ImmutableArray.CreateBuilder<BoundStatement>();
        _scope = new BoundScope(_scope);

        foreach (var statementSyntax in syntax.Statements)
        {
            var statement = BindStatement(statementSyntax);
            statements.Add(statement);
        }

        _scope = _scope.Parent!;

        return new BoundBlockStatement(statements.ToImmutable());
    }

    private BoundStatement BindExpressionStatement(ExpressionStatementSyntax syntax)
    {
        var expression = BindExpression(syntax.Expression);
        return new BoundExpressionStatement(expression);
    }

    private BoundStatement BindVariableDeclaration(VariableDeclarationSyntax syntax)
    {
        var name = syntax.Identifier.Text!;
        var isReadOnly = syntax.Keyword.Kind == SyntaxKind.LetKeyword;
        var initializer = BindExpression(syntax.Initializer);
        var variable = new VariableSymbol(name, isReadOnly, initializer.Type);

        if (!_scope.TryDeclare(variable))
        {
            Diagnostics.ReportVariableAlreadyDeclared(syntax.Identifier.Span, name);
        }

        return new BoundVariableDeclaration(variable, initializer);
    }


    private BoundStatement BindIfStatement(IfStatementSyntax syntax)
    {
        var condition = BindExpression(syntax.Condition, typeof(bool));
        var thenStatement = BindStatement(syntax.ThenStatement);
        var elseStatement = syntax.ElseClause is null
            ? null
            : BindStatement(syntax.ElseClause.ElseStatement);

        return new BoundIfStatement(condition, thenStatement, elseStatement);
    }

    private BoundStatement BindWhileStatement(WhileStatementSyntax syntax)
    {
        var condition = BindExpression(syntax.Condition, typeof(bool));
        var body = BindStatement(syntax.Body);
        return new BoundWhileStatement(condition, body);
    }
    
    private BoundStatement BindForStatement(ForStatementSyntax syntax)
    {
        var lowerBound = BindExpression(syntax.LowerBound, typeof(int));
        var upperBound = BindExpression(syntax.UpperBound, typeof(int));
        
        _scope = new BoundScope(_scope);
        
        var name = syntax.Identifier.Text!;
        var variable = new VariableSymbol(name, true, typeof(int));

        if (!_scope.TryDeclare(variable))
        {
            Diagnostics.ReportVariableAlreadyDeclared(syntax.Identifier.Span, name);
        }

        var body = BindStatement(syntax.Body);
        
        _scope = _scope.Parent!;

        return new BoundForStatement(variable, lowerBound, upperBound, body);
    }

    #endregion

    public static BoundGlobalScope BindGlobalScope(BoundGlobalScope? previous, CompilationUnitSyntax syntax)
    {
        var parentScope = CreateParentScopes(previous);
        var binder = new Binder(parentScope);

        var expression = binder.BindStatement(syntax.Statement);
        var variables = binder._scope.GetDeclaredVariables();
        var diagnostics = binder.Diagnostics.ToImmutableArray();

        if (previous is not null)
            diagnostics = diagnostics.InsertRange(0, previous.Diagnostics);

        return new BoundGlobalScope(previous, diagnostics, variables, expression);
    }

    private static BoundScope? CreateParentScopes(BoundGlobalScope? previous)
    {
        var stack = new Stack<BoundGlobalScope>();

        while (previous is not null)
        {
            stack.Push(previous);
            previous = previous.Previous;
        }

        BoundScope? current = null;
        while (stack.Count > 0)
        {
            previous = stack.Pop();
            var scope = new BoundScope(current);

            foreach (var v in previous.Variables)
            {
                scope.TryDeclare(v);
            }

            current = scope;
        }

        return current;
    }

    private BoundExpression BindNameExpression(NameExpressionSyntax syntax)
    {
        var name = syntax.IdentifierToken.Text!;

        if (!_scope.TryLookup(name, out var variable))
        {
            Diagnostics.ReportUndefinedName(syntax.IdentifierToken.Span, name);
            return new BoundLiteralExpression(0);
        }

        return new BoundVariableExpression(variable);
    }

    private BoundExpression BindAssignmentExpression(AssignmentExpressionSyntax syntax)
    {
        var name = syntax.IdentifierToken.Text!;
        var boundExpression = BindExpression(syntax.Expression);

        if (!_scope.TryLookup(name, out var variable))
        {
            Diagnostics.ReportUndefinedName(syntax.IdentifierToken.Span, name);
            return boundExpression;
        }

        if (variable.IsReadOnly)
        {
            Diagnostics.ReportCannotAssign(syntax.EqualsToken.Span, name);
        }

        if (boundExpression.Type != variable.Type)
        {
            Diagnostics.ReportCannotConvert(syntax.Expression.Span, boundExpression.Type, variable.Type);
            return boundExpression;
        }

        return new BoundAssignmentExpression(variable, boundExpression);
    }

    private BoundExpression BindParenthesizedExpression(ParenthesizedExpressionSyntax syntax)
    {
        return BindExpression(syntax.Expression);
    }

    private BoundExpression BindLiteralExpression(LiteralExpressionSyntax syntax)
    {
        var value = syntax.Value ?? 0;

        return new BoundLiteralExpression(value);
    }

    private BoundExpression BindUnaryExpression(UnaryExpressionSyntax syntax)
    {
        var boundOperand = BindExpression(syntax.Operand);
        var boundUnaryOperator = BoundUnaryOperator.Bind(syntax.OperatorToken.Kind, boundOperand.Type);

        if (boundUnaryOperator is null)
        {
            Diagnostics.ReportUndefinedUnaryOperator(syntax.OperatorToken.Span, syntax.OperatorToken.Text,
                boundOperand.Type);
            return boundOperand;
        }

        return new BoundUnaryExpression(boundUnaryOperator, boundOperand);
    }


    private BoundExpression BindBinaryExpression(BinaryExpressionSyntax syntax)
    {
        var boundLeft = BindExpression(syntax.Left);
        var boundRight = BindExpression(syntax.Right);
        var boundBinaryOperator = BoundBinaryOperator.Bind(syntax.OperatorToken.Kind, boundLeft.Type, boundRight.Type);

        if (boundBinaryOperator is null)
        {
            Diagnostics.ReportUndefinedBinaryOperator(syntax.OperatorToken.Span, syntax.OperatorToken.Text,
                boundLeft.Type, boundRight.Type);
            return boundLeft;
        }

        return new BoundBinaryExpression(boundLeft, boundBinaryOperator, boundRight);
    }
}