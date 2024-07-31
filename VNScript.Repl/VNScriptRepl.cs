using VNScript.CodeAnalysis;
using VNScript.CodeAnalysis.Syntax;
using VNScript.CodeAnalysis.Text;
using VNScript.Repl;

namespace VNScript.Repl;

internal sealed class VNScriptRepl : Repl
{
    private readonly Dictionary<VariableSymbol, object> _variables = new();
    
    private Compilation? _previous;
    private bool _showTree;
    private bool _showProgram;

    protected override void RenderLine(string line)
    {
        var tokens = SyntaxTree.ParseTokens(line);
        foreach (var token in tokens)
        {
            var isKeyword = token.Kind.ToString().EndsWith("Keyword");
            var isNumber = token.Kind == SyntaxKind.NumberToken;
                
            if (isKeyword)
                Console.ForegroundColor = ConsoleColor.Magenta;
            else if (isNumber)
                Console.ForegroundColor = ConsoleColor.Red;
            
            Console.Write(token.Text);
            
            Console.ResetColor();
        }
    }

    protected override void EvaluateMetaCommand(string text)
    {
        switch (text)
        {
            case "#showTree":
                _showTree = !_showTree;
                Console.WriteLine(_showTree ? "Showing parse trees." : "Not showing parse trees");
                break;
            case "#showProgram":
                _showProgram = !_showProgram;
                Console.WriteLine(_showProgram ? "Showing bound tree." : "Not showing bound tree");
                break;
            case "#cls":
                Console.Clear();
                break;
            case "#reset":
                _previous = null;
                break;
            default:
                base.EvaluateMetaCommand(text);
                break;
        }
    }

    protected override bool IsCompleteSubmission(string text)
    {
        if (string.IsNullOrEmpty(text))
            return true;
        
        
        var syntaxTree = SyntaxTree.Parse(text);

        if (GetLastToken(syntaxTree.Root.Statement).IsMissing)
            return false;

        return true;
    }

    private SyntaxToken GetLastToken(SyntaxNode node)
    {
        while (true)
        {
            if (node is SyntaxToken token) return token;

            node = node.GetChildren().Last();
        }
    }

    protected override void EvaluateSubmission(string text)
    {
        var syntaxTree = SyntaxTree.Parse(text);

        var compilation = _previous is null
            ? new Compilation(syntaxTree)
            : _previous.ContinueWith(syntaxTree);
            
        if (_showTree)
            syntaxTree.Root.WriteTo(Console.Out);

        if (_showProgram)
            compilation.EmitTree(Console.Out);

        var result = compilation.Evaluate(_variables);

        if (!result.Diagnostics.Any())
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(result.Value);
            Console.ResetColor();

            _previous = compilation;
        }
        else
        {
            foreach (var diagnostic in result.Diagnostics)
            {
                var lineIndex = syntaxTree.Text.GetLineIndex(diagnostic.Span.Start);
                var line = syntaxTree.Text.Lines[lineIndex];
                var lineNumber = lineIndex + 1;
                var character = diagnostic.Span.Start - line.Start + 1;
                Console.WriteLine();

                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.Write($"({lineNumber}, {character}): ");
                Console.WriteLine(diagnostic);
                Console.ResetColor();

                var prefixSpan = TextSpan.FromBounds(line.Start, diagnostic.Span.Start);
                var suffixSpan = TextSpan.FromBounds(diagnostic.Span.End, line.Span.End);

                var prefix = syntaxTree.Text.ToString(prefixSpan);
                var error = syntaxTree.Text.ToString(diagnostic.Span);
                var suffix = syntaxTree.Text.ToString(suffixSpan);

                Console.Write("    ");
                Console.Write(prefix);

                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.Write(error);
                Console.ResetColor();

                Console.Write(suffix);
            }

            Console.WriteLine();
        }
    }


}