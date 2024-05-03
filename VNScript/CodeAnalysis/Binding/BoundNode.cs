using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using VNScript.CodeAnalysis.Binding.Enums;
using VNScript.CodeAnalysis.Syntax;

namespace VNScript.CodeAnalysis.Binding;

public abstract class BoundNode
{
    public abstract BoundNodeKind Kind { get; }
    public IEnumerable<BoundNode> GetChildren()
    {
        var properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var property in properties)
        {
            if (typeof(BoundNode).IsAssignableFrom(property.PropertyType))
            {
                var child = property.GetValue(this);

                if (child is BoundNode castedChild)
                {
                    yield return castedChild;    
                }
            }
            else if (typeof(IEnumerable<BoundNode>).IsAssignableFrom(property.PropertyType))
            {
                var children = property.GetValue(this)!;

                if (children is IEnumerable<BoundNode> castedChildren)
                {
                    foreach (var child in castedChildren)
                    {
                        yield return child;    
                    }
                }
            }
        }
    }

    public void WriteTo(TextWriter writer)
    {
        PrettyPrint(writer, this);
    }
    
    private static void PrettyPrint(TextWriter writer, BoundNode node, string indent = "", bool isLast = true)
    {
        var isToConsole = writer == Console.Out;
        
        var marker = isLast
            ? @"└──"
            : @"├──";

        if (isToConsole)
            Console.ForegroundColor = ConsoleColor.DarkGray;
        
        writer.Write(indent);
        writer.Write(marker);

        if (isToConsole)
            Console.ForegroundColor = GetColor(node);
        
        var text = GetText(node);
        writer.Write(text);

        var isFirstProperty = true;
        
        foreach (var p in node.GetProperties())
        {
            if (isFirstProperty)
            {
                isFirstProperty = false;
            }
            else
            {
                if (isToConsole)
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                
                writer.Write(",");
            }
            
            
            writer.Write(" ");

            if (isToConsole)
                Console.ForegroundColor = ConsoleColor.Yellow;
            
            writer.Write(p.name);
            
            if (isToConsole)
                Console.ForegroundColor = ConsoleColor.DarkGray;
            
            writer.Write(" = ");
            
            if (isToConsole)
                Console.ForegroundColor = ConsoleColor.DarkYellow;
            
            writer.Write(p.value);
        }
        
        if (isToConsole)
            Console.ResetColor();
        
        writer.WriteLine();

        indent += isLast ? "    " : @"│    ";

        var lastChild = node.GetChildren().LastOrDefault();

        foreach (var child in node.GetChildren())
        {
            PrettyPrint(writer, child, indent, child == lastChild);
        }
    }
    

    private static string GetText(BoundNode node) =>
        node switch
        {
            BoundBinaryExpression b => b.Op.Kind + "Expression",
            BoundUnaryExpression u => u.Op.Kind + "Expression",
            _ => node.Kind.ToString()
        };

    private static ConsoleColor GetColor(BoundNode node) =>
        node switch
        {
            BoundExpression => ConsoleColor.Blue,
            BoundStatement => ConsoleColor.Cyan,
            _ => ConsoleColor.Yellow
        };

    public override string ToString()
    {
        using var writer = new StringWriter();
        WriteTo(writer);
        return writer.ToString();
    }
    
    private IEnumerable<(string name, object value)> GetProperties()
    {
        var properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var property in properties)
        {
            if(property.Name is nameof(Kind) or nameof(BoundBinaryExpression.Op))
                continue;
            
            if (typeof(BoundNode).IsAssignableFrom(property.PropertyType) ||
                typeof(IEnumerable<BoundNode>).IsAssignableFrom(property.PropertyType))
            {
                continue;
            }

            var value = property.GetValue(this);
            if (value is not null)
            {
                yield return (property.Name, value);
            }
        }
    }
}