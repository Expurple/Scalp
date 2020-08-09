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
			// Language has no functions yet, so we treat exit() as a special case
			if (input == "exit()")
			{
				ExitFlag = true;
			}

			// Language has no functions yet, so we treat print() as a special case
			if (input.StartsWith("print(") && input.EndsWith(')'))
			{
				MessageFlag = true;
				Message = input[6..^1];
				if (! Message.EndsWith('\n'))
				{
					Message += '\n';
				}
			}
			else
			{
				MessageFlag = false;
			}
		}
	}
}
