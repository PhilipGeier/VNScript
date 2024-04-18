﻿namespace VNScript.CodeAnalysis.Syntax;

public static class SyntaxFacts
{
    public static int GetUnaryOperatorPrecedence(this SyntaxKind kind)
    {
        switch (kind)
        {
            case SyntaxKind.PlusToken:
            case SyntaxKind.MinusToken:
            case SyntaxKind.BangToken:
                return 7;
            default:
                return 0;
        }
    }

    public static int GetBinaryOperatorPrecedence(this SyntaxKind kind)
    {
        switch (kind)
        {
            case SyntaxKind.AsteriskAsteriskToken:
                return 6;
            case SyntaxKind.AsteriskToken:
            case SyntaxKind.SlashToken:
                return 5;
            case SyntaxKind.PlusToken:
            case SyntaxKind.MinusToken:
                return 4;

            case SyntaxKind.EqualsEqualsToken:
            case SyntaxKind.BangEqualsToken:
                return 3;

            case SyntaxKind.AmpersandAmpersandToken:
                return 2;
            case SyntaxKind.PipePipeToken:
                return 1;
            default:
                return 0;
        }
    }

    public static SyntaxKind GetKeywordKind(string text)
    {
        switch (text)
        {
            case "true":
                return SyntaxKind.TrueKeyword;
            case "false":
                return SyntaxKind.FalseKeyword;
            default:
                return SyntaxKind.IdentifierToken;
        }
    }

    public static string? GetText(SyntaxKind kind)
    {
        switch (kind)
        {
            case SyntaxKind.PlusToken: return "+";
            case SyntaxKind.MinusToken: return "-";
            case SyntaxKind.AsteriskToken: return "*";
            case SyntaxKind.AsteriskAsteriskToken: return "**";
            case SyntaxKind.SlashToken: return "/";
            case SyntaxKind.OpenParenthesesToken: return "(";
            case SyntaxKind.CloseParenthesesToken: return ")";
            case SyntaxKind.OpenBraceToken: return "{";
            case SyntaxKind.CloseBraceToken: return "}";
            case SyntaxKind.BangToken: return "!";
            case SyntaxKind.AmpersandAmpersandToken: return "&&";
            case SyntaxKind.PipePipeToken: return "||";
            case SyntaxKind.EqualsEqualsToken: return "==";
            case SyntaxKind.BangEqualsToken: return "!=";
            case SyntaxKind.EqualsToken: return "=";
            case SyntaxKind.FalseKeyword: return "false";
            case SyntaxKind.TrueKeyword: return "true";
            default: return null;
        }
    }
    
    public static IEnumerable<SyntaxKind> GetBinaryOperatorKinds()
    {
        var kinds = Enum.GetValues<SyntaxKind>();

        foreach (var kind in kinds)
        {
            if (GetBinaryOperatorPrecedence(kind) > 0)
            {
                yield return kind;
            }
        }
    }
    
    public static IEnumerable<SyntaxKind> GetUnaryOperatorKinds()
    {
        var kinds = Enum.GetValues<SyntaxKind>();

        foreach (var kind in kinds)
        {
            if (GetUnaryOperatorPrecedence(kind) > 0)
            {
                yield return kind;
            }
        }
    }
}