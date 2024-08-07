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
            case SyntaxKind.TildeToken:
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
            case SyntaxKind.LessToken:
            case SyntaxKind.LessOrEqualsToken:
            case SyntaxKind.GreaterToken:
            case SyntaxKind.GreaterOrEqualsToken:
                return 3;

            case SyntaxKind.AmpersandAmpersandToken:
            case SyntaxKind.AmpersandToken:
                return 2;
            case SyntaxKind.PipePipeToken:
            case SyntaxKind.PipeToken:
            case SyntaxKind.HatToken:
                return 1;
            default:
                return 0;
        }
    }

    public static SyntaxKind GetKeywordKind(string text) =>
        text switch
        {
            "true" => SyntaxKind.TrueKeyword,
            "false" => SyntaxKind.FalseKeyword,
            "let" => SyntaxKind.LetKeyword,
            "var" => SyntaxKind.VarKeyword,
            "if" => SyntaxKind.IfKeyword,
            "else" => SyntaxKind.ElseKeyword,
            "while" => SyntaxKind.WhileKeyword,
            "for" => SyntaxKind.ForKeyword,
            "to" => SyntaxKind.ToKeyword,
            _ => SyntaxKind.IdentifierToken
        };

    public static string? GetText(SyntaxKind kind) =>
        kind switch
        {
            SyntaxKind.PlusToken => "+",
            SyntaxKind.MinusToken => "-",
            SyntaxKind.TildeToken => "~",
            SyntaxKind.AsteriskToken => "*",
            SyntaxKind.AsteriskAsteriskToken => "**",
            SyntaxKind.SlashToken => "/",
            SyntaxKind.OpenParenthesesToken => "(",
            SyntaxKind.CloseParenthesesToken => ")",
            SyntaxKind.OpenBraceToken => "{",
            SyntaxKind.CloseBraceToken => "}",
            SyntaxKind.BangToken => "!",
            SyntaxKind.AmpersandAmpersandToken => "&&",
            SyntaxKind.PipePipeToken => "||",
            SyntaxKind.EqualsEqualsToken => "==",
            SyntaxKind.BangEqualsToken => "!=",
            SyntaxKind.EqualsToken => "=",
            SyntaxKind.LessToken => "<",
            SyntaxKind.GreaterToken => ">",
            SyntaxKind.LessOrEqualsToken => "<=",
            SyntaxKind.GreaterOrEqualsToken => ">=",
            SyntaxKind.AmpersandToken => "&",
            SyntaxKind.PipeToken => "|",
            SyntaxKind.HatToken => "^",
            SyntaxKind.FalseKeyword => "false",
            SyntaxKind.TrueKeyword => "true",
            SyntaxKind.LetKeyword => "let",
            SyntaxKind.VarKeyword => "var",
            SyntaxKind.IfKeyword => "if",
            SyntaxKind.ElseKeyword => "else",
            SyntaxKind.WhileKeyword => "while",
            SyntaxKind.ForKeyword => "for",
            SyntaxKind.ToKeyword => "to",
            _ => null
        };

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

    public static bool IsEscapableStringChar(char c)
    {
        return GetEscapableStringChars().Contains(c);
    }
    
    public static IEnumerable<char> GetEscapableStringChars()
    {
        return new[]
        {
            '\\',
            '"',
        };
    }
}