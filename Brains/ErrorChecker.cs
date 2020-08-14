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
		private List<ScalpToken> _tokens;

		public delegate void Setter(int pos);
		Setter SetErrorPos;

		public ErrorChecker(FullProgramState state, Setter meansToSetErrorPos)
		{
			_state = state;
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
	}
}
