using System;
using System.Collections.Generic;
using System.Text;

using Scalp.CoreClasses;
using Scalp.ProgramState;

namespace Scalp.Brains
{
	// It takes a tokenized statement and throws errors if it has any.
	// Statement is not executed.
	class ErrorChecker
	{
		FullProgramState _state;
		Variables _variables;
		Types _types;

		private List<ScalpToken> _tokens;

		public delegate void Setter(int pos);
		Setter SetErrorPos;

		public ErrorChecker(FullProgramState state, Setter meansToSetErrorPos)
		{
			_state = state;
			_variables = _state.Variables;
			_types = _state.Types;
			SetErrorPos = meansToSetErrorPos;
		}

		public void CheckLineOfCode(List<ScalpToken> tokens)
		{
			_tokens = tokens;
			
			if (_tokens.Count == 0)
			{
				return; // Ignore empty lines
			}

			if (_tokens[0].value == "if")
			{
				CheckIfStatement();
				return;
			}

			if (_tokens[0].value == "}")
			{
				CheckClosingScope();
				return;
			}
		}

		private void CheckIfStatement()
		{
			if (_tokens.Count == 1)
			{
				SetErrorPos(_tokens[0].posInSourceLine + 2);
				throw new Exception("Expected a condition after an \"if\" statement.");
			}

			if (_tokens.Count > 2)
			{
				SetErrorPos(_tokens[1].posInSourceLine + _tokens[1].value.Length);
				throw new Exception($"Expected new line after the condition \"{_tokens[1].value}\".");
			}
		}

		private void CheckClosingScope()
		{
			if (_state.IfStack.Count == 0)
			{
				SetErrorPos(_tokens[0].posInSourceLine);
				throw new Exception("There's no scope to be closed.");
			}
			else if (_tokens.Count > 1)
			{
				SetErrorPos(_tokens[0].posInSourceLine + 1);
				throw new Exception("Expected new line after \"}\".");
			}
		}

		public void CheckTokenForType(ScalpToken token, string expectedType)
		{
			// Cases where everyting is fine:
			if ((expectedType == "String" && token.kind == ScalpToken.Kind.StringLiteral)
				||
				(expectedType == "Bool" && token.kind == ScalpToken.Kind.BoolLiteral)
				||
				_state.Variables.VariableExists(token.value, _state.Types.GetType(expectedType)))
			{
				return;
			}

			// Something's not fine:
			SetErrorPos(token.posInSourceLine);

			if (_variables.VariableExists(token.value))
			{
				throw new Exception($" Wrong type of variable \"{token.value}\".\n" +
					$"Expected: \"{expectedType}\", got: \"{_variables.GetVariable(token.value).Type.TypeName}\".");
			}
			else if (_types.TypeExists(token.value))
			{
				throw new Exception($"Expected a {expectedType} instance instead of typename \"{token.value}\".");
			}
			else if (token.kind == ScalpToken.Kind.StringLiteral)
			{
				throw new Exception($"Expected a {expectedType} instance instead of a String literal.");
			}
			else if (token.kind == ScalpToken.Kind.CharLiteral)
			{
				throw new Exception($"Expected a {expectedType} instance instead of a Char literal.");
			}
			else if (token.kind == ScalpToken.Kind.BoolLiteral)
			{
				throw new Exception($"Expected a {expectedType} instance instead of a Bool literal.");
			}
			else if (token.kind == ScalpToken.Kind.Keyword)
			{
				throw new Exception($"Expected a {expectedType} instance instead of a keyword \"{token.value}\".");
			}
			else
			{
				throw new Exception($"Unknown identifier \"{token.value}\".");
			}
		}
	}
}
