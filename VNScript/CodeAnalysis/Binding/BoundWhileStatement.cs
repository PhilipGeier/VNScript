using VNScript.CodeAnalysis.Binding.Enums;

namespace VNScript.CodeAnalysis.Binding;

internal class BoundWhileStatement : BoundStatement
{
    public BoundExpression Condition { get; }
    public BoundStatement Body { get; }
    public override BoundNodeKind Kind => BoundNodeKind.WhileStatement;

    public BoundWhileStatement(BoundExpression condition, BoundStatement body)
    {
        Condition = condition;
        Body = body;
    }
}