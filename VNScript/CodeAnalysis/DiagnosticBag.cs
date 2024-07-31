using System.Collections;
using VNScript.CodeAnalysis.Syntax;
using VNScript.CodeAnalysis.Text;

namespace VNScript.CodeAnalysis;

internal sealed class DiagnosticBag : IEnumerable<Diagnostic>
{
    private readonly List<Diagnostic> _diagnostics = [];
    public IEnumerator<Diagnostic> GetEnumerator() => _diagnostics.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

        
    public void AddRange(DiagnosticBag diagnosticBag)
    {
        _diagnostics.AddRange(diagnosticBag._diagnostics);
    }
    
    public void ReportInvalidNumber(TextSpan span, string text, Type type)
    {
        var message = $"The number {text} isn't a valid {type}.";
        Report(span, message);
    }
    
    public void ReportBadCharacter(int position, char character)
    {
        var message = $"Bad character in input: '{character}'.";
        Report(new TextSpan(position, 1), message);
    }
    
    public void ReportUnterminatedString(TextSpan span)
    {
        const string message = "Unterminated string literal.";
       
        Report(span, message);
    }
    public void ReportUnrecognizedEscapeSequence(TextSpan span)
    {
        const string message = "Unrecognized escape sequence";
        Report(span, message);
    }
    
    public void ReportUnexpectedToken(TextSpan span, SyntaxKind actualKind, SyntaxKind expectedKind)
    {
        var message = $"Unexpected token: <{actualKind}>, expected <{expectedKind}>.";
        Report(span, message);
    }

    public void ReportUndefinedUnaryOperator(TextSpan span, string? operatorText, Type operandType)
    {
        var message =
            $"Unary operator '{operatorText}' is not defined for type '{operandType}'.";
        Report(span, message);
    }

    public void ReportUndefinedBinaryOperator(TextSpan span, string? operatorText, Type leftType, Type rightType)
    {
        var message = $"Binary operator '{operatorText}' is not defined for types '{leftType}' and '{rightType}'.";
        Report(span, message);
    }
    
    
    public void ReportUndefinedName(TextSpan span, string name)
    {
        var message = $"Variable '{name}' doesn't exist in the current context.";
        Report(span, message);
    }
    
    public void ReportVariableAlreadyDeclared(TextSpan span, string name)
    {
        var message = $"Variable '{name}' is already declared.";
        Report(span, message);
    }
    
    public void ReportCannotConvert(TextSpan span, Type fromType, Type toType)
    {
        var message = $"Cannot convert type '{fromType}' to '{toType}'";
        Report(span, message);
    }

    public void ReportCannotAssign(TextSpan span, string name)
    {
        var message = $"Variable '{name}' is read-only and cannot be assigned to.";
        Report(span, message);
    }
    
    private void Report(TextSpan span, string message)
    {
        var diagnostic = new Diagnostic(span, message);
        _diagnostics.Add(diagnostic);
    }



}