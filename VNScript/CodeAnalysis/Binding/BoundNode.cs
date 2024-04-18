using VNScript.CodeAnalysis.Binding.Enums;

namespace VNScript.CodeAnalysis.Binding;

public abstract class BoundNode
{
    public abstract BoundNodeKind Kind { get; }
}