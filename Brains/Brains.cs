using System;
using System.Collections.Generic;
using System.Text;

using Scalp.Utility;

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
				string printArgument = input[6..^1];
				Message = FigureOutPrintResult(printArgument);
				if (! Message.EndsWith('\n'))
				{
					Message += '\n';
				}
				MessageFlag = true;
			}
			else
			{
				MessageFlag = false;
			}
		}

		private string FigureOutPrintResult(string printArgument)
		{
			string printResult;
			if (printArgument.StartsWith('"'))
			{
				if (printArgument == "\"" || ! printArgument.EndsWith('"'))
				{
					printResult = "Error! Expected string end (the closing \" is missing).";
				}
				else // it's a normal string literal
				{
					printResult = StringOperations.RemoveStringQuotes(printArgument);
				}
			}
			else
			{
				printResult = "Error! " + printArgument + " is not a string literal!";
			}
			return printResult;
		}
	}
}
