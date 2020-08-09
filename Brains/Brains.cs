using System;
using System.Collections.Generic;
using System.Text;

using Scalp.Utility;
using Scalp.ProgramState;
using Scalp.CoreClasses;

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

		private FullProgramState _programState;

		public Brains(FullProgramState programState)
		{
			_programState = programState;
		}

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

			if (input.StartsWith("String "))
			{
				ReactAtStringDefinition(input[7..]);
			}
		}

		private void ReactAtStringDefinition(string assignment)
		{
			assignment = StringOperations.RemoveExtraSpacesFromExpression(assignment);
			var variableAndValue = assignment.Split('=');
			var newVariable = new ScalpVariable(variableAndValue[0],
											_programState.Types.GetType("String"));
			_programState.Variables.AddVariable(newVariable);
			newVariable.PrimitiveValue = variableAndValue[1];
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
