﻿using System.Collections.Immutable;
using VNScript.CodeAnalysis.Binding;
using VNScript.CodeAnalysis.Lowering;
using VNScript.CodeAnalysis.Syntax;

namespace VNScript.CodeAnalysis;

public class Compilation
{
    public Compilation? Previous { get; }
    private BoundGlobalScope? _globalScope;
    private SyntaxTree SyntaxTree { get; }

    private BoundGlobalScope? GlobalScope
    {
        get
        {
            if (_globalScope == null)
            {
                var globalScope = Binder.BindGlobalScope(Previous?.GlobalScope, SyntaxTree.Root);
                Interlocked.CompareExchange(ref _globalScope, globalScope, null);
            }

            return _globalScope;
        }   
    }
    
    public Compilation(SyntaxTree syntaxTree)
        : this(null, syntaxTree)
    {
    }

    private Compilation(Compilation? previous, SyntaxTree syntaxTree)
    {
        Previous = previous;
        SyntaxTree = syntaxTree;
    }
    
    public Compilation ContinueWith(SyntaxTree syntaxTree)
    {
        return new Compilation(this, syntaxTree);
    }
    
    public EvaluationResult Evaluate(Dictionary<VariableSymbol, object> variables)
    {
        
        var diagnostics = SyntaxTree.Diagnostics.Concat(GlobalScope.Diagnostics).ToArray();

        if (diagnostics.Length != 0)
        {
            return new EvaluationResult(diagnostics.ToImmutableArray(), null);
        }

        var statement = GetStatement();
        var evaluator = new Evaluation(statement, variables);
        var value = evaluator.Evaluate();

        return new EvaluationResult(ImmutableArray<Diagnostic>.Empty, value);
    }

    public void EmitTree(TextWriter writer)
    {
        var statement = GetStatement();
        statement.WriteTo(writer);
    }
    
    private BoundBlockStatement GetStatement()
    {
        var result = GlobalScope?.Statement;
        return Lowerer.Lower(result!);
    }
}