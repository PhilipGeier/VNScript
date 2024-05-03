using VNScript.CodeAnalysis.Binding.Enums;

namespace VNScript.CodeAnalysis.Binding;

internal sealed class BoundLabelStatement : BoundStatement
{
    public LabelSymbol Symbol { get; }

    public BoundLabelStatement(LabelSymbol symbol)
    {
        Symbol = symbol;
    }

    public override BoundNodeKind Kind => BoundNodeKind.LabelStatement;
}