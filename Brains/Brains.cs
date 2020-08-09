using System;
using System.Collections.Generic;
using System.Text;

namespace Scalp.Brains
{
	// A top level entity, responsible for the whole process of
	// analyzing an input statement, executing it
	// and probably returning something to print.
	class Brains
	{
		public bool ExitFlag { get; private set; }
		public bool MessageFlag { get; private set; }

		public string Message { get; private set; }

		public void ReactAt(string input)
		{
			// Language has no functions yet, so we treat it as a special case
			if (input == "exit()")
			{
				ExitFlag = true;
			}
		}
	}
}
