using System.Collections.Immutable;
using System.Text;
using VNScript.CodeAnalysis.Text;

namespace VNScript.Tests.CodeAnalysis;

internal sealed class AnnotatedText
{
    public string Text { get; }
    public ImmutableArray<TextSpan> Spans { get; }

    public AnnotatedText(string text, ImmutableArray<TextSpan> spans)
    {
        Text = text;
        Spans = spans;
    }

    public static AnnotatedText Parse(string text)
    {
        var textBuilder = new StringBuilder();
        var spanBuilder = ImmutableArray.CreateBuilder<TextSpan>();

        var startStack = new Stack<int>();

        var position = 0;

        foreach (var c in text)
        {
            if (c == '[')
            {
                startStack.Push(position);
            }
            else if (c == ']')
            {
                if (startStack.Count == 0)
                {
                    throw new ArgumentException("Too many ']' in text", nameof(text));
                }

                var start = startStack.Pop();
                var end = position;
                var span = TextSpan.FromBounds(start, end);
                spanBuilder.Add(span);
            }
            else
            {
                position++;
                textBuilder.Append(c);
            }
        }

        if (startStack.Count != 0)
        {
            throw new ArgumentException("Missing ']' in text", nameof(text));
        }

        return new AnnotatedText(textBuilder.ToString(), spanBuilder.ToImmutable());
    }
}