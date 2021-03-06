﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Scalp.ProgramState
{
	class FullProgramState
	{
		public readonly Types Types;
		public readonly Variables Variables;
		public Stack<bool> IfStack;

		public FullProgramState()
		{
			Types = new Types();
			Variables = new Variables(Types);
			IfStack = new Stack<bool>();
		}
	}
}
