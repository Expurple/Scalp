using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using Scalp.CoreClasses;

namespace Scalp.Brains
{
	class Tokenizer
	{
		public static readonly List<char> SEPARATE_CHAR_TOKENS = new List<char>
			{ '=', '(', ')', '}' };
		public static readonly List<string> BOOL_LITERALS = new List<string>
			{ "true", "false" };
		public static readonly List<string> KEYWORDS = new List<string>
			{ "if" };

		private int i;
		private string _statement;
		private List<ScalpToken> _tokens;
		private ScalpToken _token;

		public delegate void Setter(int pos);
		Setter SetErrorPos;

		public Tokenizer(Setter meansToSetErrorPos)
		{
			SetErrorPos = meansToSetErrorPos;
		}

		public List<ScalpToken> Tokenize(string statement)
		{
			_statement = statement;
			_tokens = new List<ScalpToken>();
			_token = new ScalpToken("", ScalpToken.Kind.Identifier, 0);

			for (i = 0; i < _statement.Length; i++)
			{
				if (_statement[i] == '\'')
				{
					SaveActiveToken();
					_tokens.Add(TokenizeCharLiteral());
					_token.posInSourceLine += 3;
				}
				else if (_statement[i] == '\"')
				{
					SaveActiveToken();
					_tokens.Add(TokenizeStringLiteral());
					_token.posInSourceLine += _tokens.Last().value.Length;
				}
				else if (SEPARATE_CHAR_TOKENS.Contains(_statement[i]))
				{
					SaveActiveToken();
					_tokens.Add(new ScalpToken(_statement[i].ToString(),
												ScalpToken.Kind.Character, i));
					_token.posInSourceLine++;
				}
				else if (_statement[i] == ' ' || _statement[i] == '\t')
				{
					SaveActiveToken();
					_token.posInSourceLine++;
				}
				else
				{
					_token.value += _statement[i];

					if (KEYWORDS.Contains(_token.value))
					{
						_token.kind = ScalpToken.Kind.Keyword;
						SaveActiveToken();
						_token.posInSourceLine++;
					}
					else if (BOOL_LITERALS.Contains(_token.value))
					{
						_token.kind = ScalpToken.Kind.BoolLiteral;
						SaveActiveToken();
						_token.posInSourceLine++;
					}
				}
			}
			SaveActiveToken();
			return _tokens;
		}

		private void SaveActiveToken()
		{
			if (_token.value != "") // otherwise no need to save
			{
				CheckForInvalidIdentifier(_token);
				_tokens.Add(_token);
				_token = new ScalpToken("", ScalpToken.Kind.Identifier, i);
			}
		}

		private void CheckForInvalidIdentifier(ScalpToken token)
		{
			if (token.kind == ScalpToken.Kind.Identifier
					&& !IsValidIdentifierName(token.value))
			{
				SetErrorPos(token.posInSourceLine);
				throw new Exception($"\"{token.value}\" is an invalid identifier.\n" +
					"Identifiers must only contain ASCII letters, digits, underscores or dashes,\n" +
					"and must not start with a digit.");
			}
		}

		private bool IsValidIdentifierName(string name)
		{
			return name.All(ch => ('a' <= ch && ch <= 'z') ||
								('A' <= ch && ch <= 'Z') ||
								('0' <= ch && ch <= '9') ||
								ch == '_' || ch == '-')
				&&
				!('0' <= name[0] && name[0] <= '9');
		}

		private ScalpToken TokenizeCharLiteral()
		{
			if (i < _statement.Length - 2 && _statement[i + 2] == '\'')
			{
				i += 2;
				return new ScalpToken(_statement[(i - 2)..(i + 1)],
									ScalpToken.Kind.CharLiteral, i - 2);
			}
			else
			{
				SetErrorPos(i);
				throw new ArgumentException("Invalid char literal!");
			}
		}

		private ScalpToken TokenizeStringLiteral()
		{
			var newToken = new ScalpToken("\"", ScalpToken.Kind.StringLiteral, i);
			i++;
			for (/*     */; i < _statement.Length; i++)
			{
				newToken.value += _statement[i];
				if (_statement[i] == '\"')
				{
					return newToken;
				}
			}
			SetErrorPos(_statement.Length - 1);
			throw new ArgumentException("Expected \" at the end of line " + 
				$"to close the opened string literal {newToken.value}");
		}
	}
}
