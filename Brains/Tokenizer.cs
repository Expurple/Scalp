using System;
using System.Collections.Generic;
using System.Text;

using Scalp.CoreClasses;

namespace Scalp.Brains
{
	class Tokenizer
	{
		public readonly List<char> SEPARATE_CHAR_TOKENS = new List<char>
			{ '=', '(', ')' };
		public readonly List<string> BOOL_LITERALS = new List<string>
			{ "true", "false" };
		public readonly List<string> KEYWORDS = new List<string>
			{ "if" };

		private string _statement;
		private List<ScalpToken> _tokens;
		private ScalpToken _token;

		public List<ScalpToken> Tokenize(string statement)
		{
			_statement = statement;
			_tokens = new List<ScalpToken>();
			_token = new ScalpToken("", ScalpToken.Kind.Identifier);

			for (int i = 0; i < _statement.Length; i++)
			{
				if (_statement[i] == '\'')
				{
					SaveActiveToken();
					_tokens.Add(TokenizeCharLiteral(ref i));
				}
				else if (_statement[i] == '\"')
				{
					SaveActiveToken();
					_tokens.Add(TokenizeStringLiteral(ref i));
				}
				else if (SEPARATE_CHAR_TOKENS.Contains(_statement[i]))
				{
					SaveActiveToken();
					_tokens.Add(new ScalpToken(_statement[i].ToString(),
												ScalpToken.Kind.Character));
				}
				else if (_statement[i] == ' ')
				{
					SaveActiveToken();
				}
				else
				{
					_token.value += _statement[i];

					if (KEYWORDS.Contains(_token.value))
					{
						_token.kind = ScalpToken.Kind.Keyword;
						SaveActiveToken();
					}
					else if (BOOL_LITERALS.Contains(_token.value))
					{
						_token.kind = ScalpToken.Kind.BoolLiteral;
						SaveActiveToken();
					}
				}
			}
			SaveActiveToken();
			return _tokens;
		}

		private void SaveActiveToken()
		{
			if (_token.value != "")
			{
				_tokens.Add(_token);
				_token = new ScalpToken("", ScalpToken.Kind.Identifier);
			}
		}

		private ScalpToken TokenizeCharLiteral(ref int i)
		{
			if (i < _statement.Length - 2 && _statement[i + 2] == '\'')
			{
				i += 2;
				return new ScalpToken(_statement[i - 1].ToString(), ScalpToken.Kind.CharLiteral);
			}
			else
			{
				throw new ArgumentException("Invalid char literal!");
			}
		}

		private ScalpToken TokenizeStringLiteral(ref int i)
		{
			var literal = new StringBuilder("\"");
			i++;
			for (/*     */; i < _statement.Length; i++)
			{
				literal.Append(_statement[i]);
				if (_statement[i] == '\"')
				{
					return new ScalpToken(literal.ToString(), ScalpToken.Kind.StringLiteral);
				}
			}
			throw new ArgumentException($"Expected \" at the end of line to close the opened string literal \'{literal.ToString()}\'.");
		}
	}
}
