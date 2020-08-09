using System;
using System.Collections.Generic;
using System.Text;

namespace Scalp.Brains
{
	class Tokenizer
	{
		public readonly List<char> SEPARATE_CHAR_TOKENS = new List<char>
			{ '=', '(', ')' };

		private string _statement;
		private List<string> _tokens;
		private string _token; // Could've been a StringBuilder, but it's more readable like this.

		public List<string> Tokenize(string statement)
		{
			_statement = statement;
			_tokens = new List<string>();
			_token = "";

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
					_tokens.Add(_statement[i].ToString());
				}
				else if (_statement[i] == ' ')
				{
					SaveActiveToken();
				}
				else
				{
					_token += _statement[i];
				}
			}
			SaveActiveToken();
			return _tokens;
		}

		private void SaveActiveToken()
		{
			if (_token != "")
			{
				_tokens.Add(_token);
				_token = "";
			}
		}

		private string TokenizeCharLiteral(ref int i)
		{
			if (i < _statement.Length - 2 && _statement[i + 2] == '\'')
			{
				i += 2;
				return _statement[i - 1].ToString();
			}
			else
			{
				throw new ArgumentException("Error: invalid char literal!");
			}
		}

		private string TokenizeStringLiteral(ref int i)
		{
			var literal = new StringBuilder("\"");
			i++;
			for (/*     */; i < _statement.Length; i++)
			{
				literal.Append(_statement[i]);
				if (_statement[i] == '\"')
				{
					return literal.ToString();
				}
			}
			throw new ArgumentException("Error: Expected string literal end (the closing \" is missing).");
		}
	}
}
