﻿using System.Reflection;
using VNScript.CodeAnalysis.Text;

namespace VNScript.CodeAnalysis.Syntax;

public abstract class SyntaxNode
{
    public abstract SyntaxKind Kind { get; }

    public virtual TextSpan Span
    {
        get
        {
            var first = GetChildren().First().Span;
            var last = GetChildren().Last().Span;

            return TextSpan.FromBounds(first.Start, last.End);
        }
    }

    public IEnumerable<SyntaxNode> GetChildren()
    {
        var properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var property in properties)
        {
            if (typeof(SyntaxNode).IsAssignableFrom(property.PropertyType))
            {
                var child = property.GetValue(this);

                if (child is SyntaxNode castedChild)
                {
                    yield return castedChild;    
                }
            }
            else if (typeof(IEnumerable<SyntaxNode>).IsAssignableFrom(property.PropertyType))
            {
                var children = property.GetValue(this)!;

                if (children is IEnumerable<SyntaxNode> castedChildren)
                {
                    foreach (var child in castedChildren)
                    {
                        yield return child;    
                    }
                }
            }
        }
    }
    
    public SyntaxToken GetLastToken()
    {
        if (this is SyntaxToken token) return token;

        return GetChildren().Last().GetLastToken();
    }

    public void WriteTo(TextWriter writer)
    {
        PrettyPrint(writer, this);
    }
    
    private static void PrettyPrint(TextWriter writer, SyntaxNode node, string indent = "", bool isLast = true)
    {
        var isToConsole = writer == Console.Out;
        
        var marker = isLast
            ? @"└──"
            : @"├──";

        writer.Write(indent);

        if (isToConsole)
            Console.ForegroundColor = ConsoleColor.DarkGray;
        
        writer.Write(marker);

        if (isToConsole)
            Console.ForegroundColor = node is SyntaxToken ? ConsoleColor.Blue : ConsoleColor.Cyan;
        
        writer.Write(node.Kind);

        if (node is SyntaxToken { Value: not null } t)
        {
            writer.Write(" ");
            writer.Write(t.Value);
        }

        if (isToConsole)
        {
            Console.ResetColor();
        }
        
        writer.WriteLine();

        indent += isLast ? "    " : @"│    ";

        var lastChild = node.GetChildren().LastOrDefault();

        foreach (var child in node.GetChildren())
        {
            PrettyPrint(writer, child, indent, child == lastChild);
        }
    }

    public override string ToString()
    {
        using var writer = new StringWriter();
        WriteTo(writer);
        return writer.ToString();
    }
}