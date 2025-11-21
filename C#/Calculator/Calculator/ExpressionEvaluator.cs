using System;
using System.Globalization;

namespace Calculator
{
    /// <summary>
    /// Evaluates arithmetic expressions with +, -, *, /, parentheses and unary +/- using decimal.
    /// Examples:
    ///   "7*2"
    ///   "10 + 5 * 3"
    ///   "-(2 + 3) / 4"
    ///   "1,5 + 2.25"
    /// </summary>
    public static class ExpressionEvaluator
    {
        /// <summary>
        /// Evaluates the given expression string and returns the result as decimal.
        /// Throws FormatException / DivideByZeroException / OverflowException for invalid input.
        /// </summary>
        public static decimal Evaluate(string expression)
        {
            if (string.IsNullOrWhiteSpace(expression))
                return 0;

            Parser parser = new Parser(expression);
            decimal result = parser.ParseExpression();

            if (!parser.IsAtEnd())
                throw new FormatException("Unexpected characters at end of expression.");

            return result;
        }

        /// <summary>
        /// Internal recursive-descent parser.
        /// Grammar:
        ///   Expression = Term   { ('+' | '-') Term }
        ///   Term       = Factor { ('*' | '/') Factor }
        ///   Factor     = Number
        ///              | '(' Expression ')'
        ///              | ('+' | '-') Factor    (unary)
        /// </summary>
        private sealed class Parser
        {
            private readonly string _text;
            private int _position;

            public Parser(string text)
            {
                _text = text;
                _position = 0;
            }

            /// <summary>
            /// Public check used by the outer class. Does NOT skip whitespace.
            /// </summary>
            public bool IsAtEnd()
            {
                SkipWhiteSpace();
                return _position >= _text.Length;
            }

            private bool IsEndRaw()
            {
                return _position >= _text.Length;
            }

            private void SkipWhiteSpace()
            {
                while (!IsEndRaw() && char.IsWhiteSpace(_text[_position]))
                    _position++;
            }

            /// <summary>
            /// Expression = Term { ('+' | '-') Term }
            /// </summary>
            /// <returns></returns>
            public decimal ParseExpression()
            {
                decimal value = ParseTerm();

                while (true)
                {
                    SkipWhiteSpace();
                    if (IsEndRaw())
                    {
                        break;
                    }

                    char op = _text[_position];

                    if (op == '+' || op == '-')
                    {
                        _position++;
                        decimal right = ParseTerm();
                        if (op == '+')
                            value += right;
                        else
                            value -= right;
                    }
                    else
                    {
                        break;
                    }
                }

                SkipWhiteSpace();
                return value;
            }

            /// <summary>
            /// Term = Factor { ('*' | '/') Factor }
            /// </summary>
            /// <returns></returns>
            /// <exception cref="DivideByZeroException"></exception>
            private decimal ParseTerm()
            {
                decimal value = ParseFactor();

                while (true)
                {
                    SkipWhiteSpace();
                    if (IsEndRaw())
                        break;

                    char op = _text[_position];

                    if (op == '*' || op == '/')
                    {
                        _position++;
                        decimal right = ParseFactor();
                        if (op == '*')
                            value *= right;
                        else
                        {
                            if (right == decimal.Zero)
                                throw new DivideByZeroException("Division by zero in expression.");

                            value /= right;
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                return value;
            }

            /// <summary>
            /// Factor = Number | '(' Expression ')' | unary +/- Factor
            /// </summary>
            /// <returns></returns>
            /// <exception cref="FormatException"></exception>
            private decimal ParseFactor()
            {
                SkipWhiteSpace();

                if (IsEndRaw())
                    throw new FormatException("Unexpected end of expression.");

                char c = _text[_position];

                if (c == '+' || c == '-')
                {
                    _position++;
                    decimal inner = ParseFactor();
                    return c == '-' ? -inner : inner;
                }

                if (c == '(')
                {
                    _position++;
                    decimal value = ParseExpression();
                    SkipWhiteSpace();

                    if (IsEndRaw() || _text[_position] != ')')
                        throw new FormatException("Missing closing parenthesis.");

                    _position++;
                    return value;
                }

                return ParseNumber();
            }

            private decimal ParseNumber()
            {
                SkipWhiteSpace();

                int start = _position;
                bool hasDecimalSeparator = false;

                while (!IsEndRaw())
                {
                    char c = _text[_position];

                    if (char.IsDigit(c))
                        _position++;
                    else if (c == '.' || c == ',')
                    {
                        if (hasDecimalSeparator)
                            throw new FormatException("Invalid number format (multiple decimal separators).");

                        hasDecimalSeparator = true;
                        _position++;
                    }
                    else
                    {
                        break;
                    }
                }

                if (start == _position)
                {
                    char bad = _position < _text.Length ? _text[_position] : '?';
                    throw new FormatException($"Number expected at position {_position}, found '{bad}'.");
                }

                string numberText = _text.Substring(start, _position - start);

                numberText = numberText.Replace(',', '.');

                return decimal.Parse(numberText, NumberStyles.Number, CultureInfo.InvariantCulture);
            }
        }
    }
}
