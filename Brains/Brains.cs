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

		private readonly Tokenizer _tokenizer;

		private readonly FullProgramState _state;
		private readonly Types _types;
		private readonly Variables _variables;

		public Brains(FullProgramState programState)
		{
			_tokenizer = new Tokenizer();
			_state = programState;
			_types = _state.Types;
			_variables = _state.Variables;
		}

		public void ReactAt(string input)
		{
			var tokens = _tokenizer.Tokenize(input);

			// Language has no functions yet, so we treat exit() as a special case
			if (tokens.Count >= 3 && tokens[0] == "exit" &&
				tokens[1] == "(" && tokens[2] == ")")
			{
				ExitFlag = true;
			}

			// Language has no functions yet, so we treat print() as a special case
			if (tokens.Count >= 4 && tokens[0] == "print" &&
				tokens[1] == "(" && tokens[3] == ")")
			{
				string printArgument = tokens[2];
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
											_types.GetType("String"));
			_variables.AddVariable(newVariable);
			newVariable.PrimitiveValue = variableAndValue[1];
		}

		private string FigureOutPrintResult(string printArgument)
		{
			if (_variables.VariableExists(printArgument, _types.GetType("String")))
			{
				return StringOperations.TrimQuotes(
					_variables.GetVariable(printArgument).PrimitiveValue as string);
			}

			string printResult;
			if (printArgument.StartsWith('"'))
			{
				if (printArgument == "\"" || ! printArgument.EndsWith('"'))
				{
					printResult = "Error! Expected string end (the closing \" is missing).";
				}
				else // it's a normal string literal
				{
					printResult = StringOperations.TrimQuotes(printArgument);
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
