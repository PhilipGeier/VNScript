using VNScript.CodeAnalysis;
using VNScript.CodeAnalysis.Syntax;

namespace VNScript.Tests.CodeAnalysis;

public class EvaluationTests
{
    [Theory]
    [InlineData("1", 1)]
    [InlineData("+1", 1)]
    [InlineData("-1", -1)]
    [InlineData("99 + 11", 110)]
    [InlineData("12 - 3", 9)]
    [InlineData("4 * 2", 8)]
    [InlineData("9 / 3", 3)]
    [InlineData("(10)", 10)]
    [InlineData("12 == 3", false)]
    [InlineData("3 == 3", true)]
    [InlineData("3 != 3", false)]
    [InlineData("3 != 5", true)]
    [InlineData("3 > 5", false)]
    [InlineData("3 < 5", true)]
    [InlineData("1 < 4", true)]
    [InlineData("1 > 4", false)]
    [InlineData("1 >= 1", true)]
    [InlineData("1 <= 2", true)]
    [InlineData("2 <= 1", false)]
    [InlineData("2 >= 3", false)]
    [InlineData("false == false", true)]
    [InlineData("true == false", false)]
    [InlineData("false != false", false)]
    [InlineData("true != false", true)]
    [InlineData("true", true)]
    [InlineData("false", false)]
    [InlineData("!true", false)]
    [InlineData("!false", true)]
    [InlineData("{ var a = 0 (a = 10) * a }", 100)]
    [InlineData("{ var a = 0 if a == 0 a = 10 a }", 10)]
    [InlineData("{ var a = 0 if a == 4 a = 10 a }", 0)]
    [InlineData("{ var a = 0 if a == 4 a = 10 else a = 5 }", 5)]
    [InlineData("{ var a = 0 if a == 0 a = 10 else a = 5 }", 10)]
    [InlineData("{ var i = 10 var result = 0 while i > 0 { result = result + i i = i - 1 } result }", 55)]
    [InlineData("{ var result = 0 for i = 1 to 10 { result = result + i } result }", 55)]
    public void Evaluator_Computes_CorrectValue(string text, object expectedValue)
    {
        AssertValue(text, expectedValue);
    }

    [Fact]
    public void Evaluator_VariableDeclaration_Reports_Redeclaration()
    {
        const string text = """
                            {
                                var x = 10
                                var y = 100
                                {
                                    var x = 10
                                }
                                var [x] = 5
                            }
                            """;

        const string diagnostics = """
                                   Variable 'x' is already declared.
                                   """;

        AssertDiagnostics(text, diagnostics);
    }
    
    [Fact]
    public void Evaluator_VariableDeclaration_Reports_IsReadOnly()
    {
        const string text = """
                            {
                                let x = 10
                                x [=] 20
                            }
                            """;
        
        const string diagnostic = $"Variable 'x' is read-only and cannot be assigned to.";
        
        AssertDiagnostics(text, diagnostic);
    }
    
    [Fact]
    public void Evaluator_UnaryExpression_Reports_UndefinedOperator()
    {
        const string text = "[+]true";

        const string diagnostics = "Unary operator '+' is not defined for type 'System.Boolean'.";

        AssertDiagnostics(text, diagnostics);
    }

    [Fact]
    public void Evaluator_BinaryExpression_Reports_UndefinedOperator()
    {
        const string text = "false [+] 1";

        const string diagnostic = "Binary operator '+' is not defined for types 'System.Boolean' and 'System.Int32'.";
        
        AssertDiagnostics(text, diagnostic);
    }

    [Fact]
    public void Evaluator_NameExpression_Reports_Undefined()
    {
        const string text = "[x] * 10";

        const string diagnostics = "Variable 'x' doesn't exist in the current context.";

        AssertDiagnostics(text, diagnostics);
    }

    [Fact]
    public void Evaluator_NameExpression_Reports_NoErrorForInsertedToken()
    {
        const string text = "[]";

        const string diagnostics = "Unexpected token: <EndOfFileToken>, expected <IdentifierToken>.";

        AssertDiagnostics(text, diagnostics);
    }
    
    [Fact]
    public void Evaluator_AssignmentExpression_Reports_CannotConvert()
    {
        const string text = """
                            {
                                var x = 10
                                x = [false]
                            }
                            """;

        const string diagnostic = "Cannot convert type 'System.Boolean' to 'System.Int32'";
        
        AssertDiagnostics(text, diagnostic);
    }

    [Fact]
    public void Evaluator_BlockStatement_NoInfiniteLoop()
    {
        const string text = """
                            {
                            [)]
                            []
                            """;
        
        const string diagnostic = """
                                  Unexpected token: <CloseParenthesesToken>, expected <IdentifierToken>.
                                  Unexpected token: <EndOfFileToken>, expected <CloseBraceToken>.
                                  """;
        
        AssertDiagnostics(text, diagnostic);
    }
    
    [Fact]
    public void Evaluator_IfStatement_Reports_CannotConvert()
    {
        const string text = """
                            {
                                var x = 0
                                if [10]
                                    x = 10
                            }
                            """;
        
        const string diagnostic = "Cannot convert type 'System.Int32' to 'System.Boolean'";
        
        AssertDiagnostics(text, diagnostic);
    }
    
    [Fact]
    public void Evaluator_WhileStatement_Reports_CannotConvert()
    {
        const string text = """
                            {
                                var x = 0
                                while [10]
                                    x = 10
                            }
                            """;
        
        const string diagnostic = "Cannot convert type 'System.Int32' to 'System.Boolean'";
        
        AssertDiagnostics(text, diagnostic);
    }
    
    [Fact]
    public void Evaluator_ForStatement_Reports_CannotConvert_LowerBound()
    {
        const string text = """
                            {
                                var result = 0
                                for i = [false] to 10
                                    result = result + i
                            }
                            """;
        
        const string diagnostic = "Cannot convert type 'System.Boolean' to 'System.Int32'";
        
        AssertDiagnostics(text, diagnostic);
    }
    
    [Fact]
    public void Evaluator_ForStatement_Reports_CannotConvert_UpperBound()
    {
        const string text = """
                            {
                                var result = 0
                                for i = 1 to [true]
                                    result = result + i
                            }
                            """;
        
        const string diagnostic = "Cannot convert type 'System.Boolean' to 'System.Int32'";
        
        AssertDiagnostics(text, diagnostic);
    }

    private static void AssertValue(string text, object expectedValue)
    {
        var expression = SyntaxTree.Parse(text);

        var compilation = new Compilation(expression);

        var variables = new Dictionary<VariableSymbol?, object>();
        var actualResult = compilation.Evaluate(variables);

        Assert.Empty(actualResult.Diagnostics);
        Assert.Equal(expectedValue, actualResult.Value);
    }
    
    private static void AssertDiagnostics(string text, string expectedDiagnostics)
    {
        var annotatedText = AnnotatedText.Parse(text);
        var syntaxTree = SyntaxTree.Parse(annotatedText.Text);
        var compilation = new Compilation(syntaxTree);

        var result = compilation.Evaluate(new Dictionary<VariableSymbol?, object>());

        for (var i = 0; i < result.Diagnostics.Length; i++)
        {
            var expectedSpan = annotatedText.Spans[i];
            var actualSpan = result.Diagnostics[i].Span;
            
            Assert.Equal(expectedSpan, actualSpan);
        }

        var actualMessage = string.Join("\r\n", result.Diagnostics.Select(x => x.Message));
        Assert.Equal(expectedDiagnostics, actualMessage);
    }
}