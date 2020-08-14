using System;
using System.Collections.Generic;
using System.Text;

using Scalp.CoreClasses;
using Scalp.ProgramState;

namespace Scalp.Brains
{
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


		}
	}
}
