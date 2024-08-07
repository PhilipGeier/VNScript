﻿using System.Text;
using VNScript.CodeAnalysis.Text;

namespace VNScript.CodeAnalysis.Syntax;

internal class Lexer
{
    private readonly SourceText _text;
    private int _position;

    public DiagnosticBag Diagnostics { get; } = new();

    private int _start;
    private SyntaxKind _kind;
    private object? _value;

    private char Current => Peek(0);
    private char Lookahead => Peek(1);

    public Lexer(SourceText text)
    {
        _text = text;
    }

    private char Peek(int offset)
    {
        var index = _position + offset;
        return index >= _text.Length
            ? '\0'
            : _text[index];
    }

    public SyntaxToken Lex()
    {
        _start = _position;
        _kind = SyntaxKind.BadToken;
        _value = null;

        switch (Current)
        {
            case '\0':
                _kind = SyntaxKind.EndOfFileToken;
                break;
            case '+':
                _kind = SyntaxKind.PlusToken;
                _position++;
                break;
            case '-':
                _kind = SyntaxKind.MinusToken;
                _position++;
                break;
            case '/':
                _kind = SyntaxKind.SlashToken;
                _position++;
                break;
            case '(':
                _kind = SyntaxKind.OpenParenthesesToken;
                _position++;
                break;
            case ')':
                _kind = SyntaxKind.CloseParenthesesToken;
                _position++;
                break;
            case '{':
                _kind = SyntaxKind.OpenBraceToken;
                _position++;
                break;
            case '}':
                _kind = SyntaxKind.CloseBraceToken;
                _position++;
                break;
            case '~':
                _kind = SyntaxKind.TildeToken;
                _position++;
                break;
            case '^':
                _kind = SyntaxKind.HatToken;
                _position++;
                break;
            case '*':
                _position++;
                if (Current != '*')
                {
                    _kind = SyntaxKind.AsteriskToken;
                    break;
                }

                _kind = SyntaxKind.AsteriskAsteriskToken;
                _position++;
                break;
            case '!':
                _position++;
                if (Current != '=')
                {
                    _kind = SyntaxKind.BangToken;
                    break;
                }

                _kind = SyntaxKind.BangEqualsToken;
                _position++;
                break;
            case '&':
                _position++;
                if (Current != '&')
                {
                    _kind = SyntaxKind.AmpersandToken;
                    break;
                }

                _kind = SyntaxKind.AmpersandAmpersandToken;
                _position++;
                break;
            case '|':
                _position++;
                if (Current != '|')
                {
                    _kind = SyntaxKind.PipeToken;
                    break;
                }

                _kind = SyntaxKind.PipePipeToken;
                _position++;
                break;
            case '=':
                _position++;
                if (Current != '=')
                {
                    _kind = SyntaxKind.EqualsToken;
                    break;
                }

                _kind = SyntaxKind.EqualsEqualsToken;
                _position++;
                break;
            case '<':
                _position++;
                if (Current != '=')
                {
                    _kind = SyntaxKind.LessToken;
                    break;
                }

                _kind = SyntaxKind.LessOrEqualsToken;
                _position++;
                break;
            case '>':
                _position++;
                if (Current != '=')
                {
                    _kind = SyntaxKind.GreaterToken;
                    break;
                }

                _kind = SyntaxKind.GreaterOrEqualsToken;
                _position++;
                break;
            case '"':
                ReadString();
                break;
            case '1':
            case '2':
            case '3':
            case '4':
            case '5':
            case '6':
            case '7':
            case '8':
            case '9':
            case '0':
                ReadNumber();
                break;
            case ' ':
            case '\t':
            case '\n':
            case '\r':
                ReadWhitespace();
                break;
            default:
                if (char.IsLetter(Current))
                {
                    ReadIdentifierOrKeyword();
                }
                else if (char.IsWhiteSpace(Current))
                {
                    ReadWhitespace();
                }
                else
                {
                    Diagnostics.ReportBadCharacter(_position, Current);
                    _position++;
                }

                break;
        }

        var length = _position - _start;
        var text = SyntaxFacts.GetText(_kind);

        if (text is null)
        {
            text = _text.ToString(_start, length);
        }

        return new SyntaxToken(_kind, _start, text, _value);
    }

    private void ReadIdentifierOrKeyword()
    {
        while (char.IsLetter(Current))
        {
            _position++;
        }

        var length = _position - _start;
        var text = _text.ToString(_start, length);

        _kind = SyntaxFacts.GetKeywordKind(text);
    }

    private void ReadString()
    {
        // Skip the current quote
        _position++;
        var sb = new StringBuilder();

        var done = false;
        while (!done)
        {
            switch (Current)
            {
                case '\0':
                case '\r':
                case '\n':
                {
                    var span = new TextSpan(_start, 1);
                    Diagnostics.ReportUnterminatedString(span);
                    done = true;
                    break;
                }
                case '"':
                    _position++;
                    done = true;
                    break;
                case '\\':
                    if (Lookahead == '"')
                    {
                        _position++;
                        sb.Append(Current);
                        _position++;
                    }
                    else
                    {
                        var span = new TextSpan(_position, 2);
                        Diagnostics.ReportUnrecognizedEscapeSequence(span);
                        _position++;
                    }
                    break;
                default:
                    sb.Append(Current);
                    _position++;
                    break;
            }
        }

        _kind = SyntaxKind.StringToken;
        _value = sb.ToString();
    }

    private void ReadWhitespace()
    {
        while (char.IsWhiteSpace(Current))
        {
            _position++;
        }

        _kind = SyntaxKind.WhiteSpaceToken;
    }

    private void ReadNumber()
    {
        while (char.IsDigit(Current))
        {
            _position++;
        }

        var length = _position - _start;
        var text = _text.ToString(_start, length);

        if (!int.TryParse(text, out var value))
        {
            Diagnostics.ReportInvalidNumber(new TextSpan(_start, length), text, typeof(int));
        }

        _value = value;
        _kind = SyntaxKind.NumberToken;
    }
}