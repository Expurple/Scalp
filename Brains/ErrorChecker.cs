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

			if (_types.TypeExists(_tokens[0].value))
			{
				CheckVariableDeclaration();
				return;
			}

			if (_tokens.Count >= 2 && _tokens[1].value == "=")
			{
				CheckVariableAssign();
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

			CheckTokenForType(_tokens[1], "Bool");

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

		private void CheckVariableDeclaration()
		{
			if (_tokens.Count == 1)
			{
				SetErrorPos(_tokens[0].posInSourceLine + _tokens[0].value.Length);
				throw new Exception($"Expected an identifier after typename \"{_tokens[0].value}\".");
			}
			if (_tokens[1].kind != ScalpToken.Kind.Identifier)
			{
				SetErrorPos(_tokens[1].posInSourceLine);
				throw new Exception($"Expected an identifier after typename \"{_tokens[0].value}\".");
			}
			if (_types.TypeExists(_tokens[1].value))
			{
				SetErrorPos(_tokens[1].posInSourceLine);
				throw new Exception($"\"{_tokens[1].value}\" is already registered as a type.");
			}
			if (_variables.VariableExists(_tokens[1].value))
			{
				SetErrorPos(_tokens[1].posInSourceLine);
				throw new Exception($"Redefinition of variable \"{_tokens[1].value}\".");
			}
			if (_tokens.Count >= 3 && _tokens[2].value != "=")
			{
				SetErrorPos(_tokens[2].posInSourceLine);
				throw new Exception("Expected \"=\" after the identifier.");
			}
			if (_tokens.Count == 3)
			{
				SetErrorPos(_tokens[2].posInSourceLine + 1);
				throw new Exception($"Expected a {_tokens[0].value} value after \"=\".");
			}
			if (_tokens.Count >= 4)
			{
				CheckTokenForType(_tokens[3], _tokens[0].value);
			}
			if (_tokens.Count > 4)
			{
				SetErrorPos(_tokens[3].posInSourceLine + _tokens[3].value.Length);
				throw new Exception($"Expected a new line after \"{_tokens[3].value}\".");
			}
		}

		private void CheckVariableAssign()
		{
			if (! _variables.VariableExists(_tokens[0].value))
			{
				SetErrorPos(_tokens[0].posInSourceLine);
				throw new Exception($"\"{_tokens[0].value}\" is not an existing variable.");
			}

			string variableType = _variables.GetVariable(_tokens[0].value).Type.TypeName;
			
			if (_tokens.Count == 2)
			{
				SetErrorPos(_tokens[1].posInSourceLine + 1);
				throw new Exception($"Expected a {variableType} value after \"=\".");
			}

			CheckTokenForType(_tokens[2], variableType);

			if (_tokens.Count > 3)
			{
				SetErrorPos(_tokens[2].posInSourceLine + _tokens[2].value.Length);
				throw new Exception($"Expected new line after the value \"{_tokens[2].value}\".");
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
