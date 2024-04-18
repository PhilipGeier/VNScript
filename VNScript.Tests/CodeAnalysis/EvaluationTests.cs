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
    public void Evaluator_Computes_CorrectValue(string text, object expectedValue)
    {
        AssertValue(text, expectedValue);
    }

    [Fact]
    public void Evaluator_VariableDeclaration_Reports_Redeclaration()
    {
        var text = """
                   {
                       var x = 10
                       var y = 100
                       {
                           var x = 10
                       }
                       var [x] = 5
                   }
                   """;

        var diagnostics = """
                          Variable 'x' is already declared.
                          """;

        AssertDiagnostics(text, diagnostics);
    }

    [Fact]
    public void Evaluator_Name_Reports_Undefined()
    {
        var text = "[x] * 10";

        var diagnostics = "Variable 'x' doesn't exist in the current context.";

        AssertDiagnostics(text, diagnostics);
    }

    [Fact]
    public void Evaluator_Variable_Reports_IsReadOnly()
    {
        var text = """
                   {
                       let x = 10
                       x [=] 20
                   }
                   """;
        
        var diagnostic = $"Variable 'x' is read-only and cannot be assigned to.";
        
        AssertDiagnostics(text, diagnostic);
    }

    [Fact]
    public void Evaluator_Assignment_Reports_CannotConvert()
    {
        var text = """
                   {
                       var x = 10
                       x = [false]
                   }
                   """;

        var diagnostic = "Cannot convert type 'System.Boolean' to 'System.Int32'";
        
        AssertDiagnostics(text, diagnostic);
    }

    [Fact]
    public void Evaluator_Unary_Reports_UndefinedOperator()
    {
        var text = "[+]true";

        var diagnostics = "Unary operator '+' is not defined for type 'System.Boolean'.";

        AssertDiagnostics(text, diagnostics);
    }

    [Fact]
    public void Evaluator_Binary_Reports_UndefinedOperator()
    {
        var text = "false [+] 1";

        var diagnostic = "Binary operator '+' is not defined for types 'System.Boolean' and 'System.Int32'.";
        
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

        var expectedSpan = annotatedText.Spans[0];
        var actualSpan = result.Diagnostics[0].Span;
        Assert.Equal(expectedSpan, actualSpan);

        var expectedMessage = expectedDiagnostics;
        var actualMessage = result.Diagnostics[0].Message;
        Assert.Equal(expectedMessage, actualMessage);
    }
}