﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

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

			if (tokens.Count == 0)
			{
				return; // Ignore empty lines
			}

			// Language has no functions yet, so we treat exit() as a special case
			if (tokens.Count >= 3 && tokens[0] == "exit" &&
				tokens[1] == "(" && tokens[2] == ")")
			{
				ExitFlag = true;
				return;
			}

			// Language has no functions yet, so we treat print() as a special case
			if (tokens.Count >= 4 && tokens[0] == "print" &&
				tokens[1] == "(" && tokens[3] == ")")
			{
				string printArgument = tokens[2];
				Message = GetStringFromVariableOrLiteral(printArgument);
				if (Message == null)
				{
					Message = "null";
				}
				MessageFlag = true;
				return;
			}
			else
			{
				MessageFlag = false;
			}

			// As for it is now, string definition is a special case
			if (tokens[0] == "String" && tokens.Count > 1)
			{
				ReactAtStringDefinition(tokens);
				return;
			}

			// As for it is now, string assignment is a special case
			if (_variables.VariableExists(tokens[0], _types.GetType("String")) &&
				tokens.Count == 3 && tokens[1] == "=")
			{
				_variables.GetVariable(tokens[0]).PrimitiveValue =
									GetStringFromVariableOrLiteral(tokens[2]);
				return;
			}

			Message = "Error: the grammar of this line is incorrect. What did you mean by that?";
			MessageFlag = true;
		}

		private void ReactAtStringDefinition(List<string> tokens)
		{
			if (!IsValidIdentifierName(tokens[1]))
			{
				Message = $"Error! {tokens[1]} is an invalid identifier.\n" +
							"Identifiers can only contain letters, digits, underscores and dashes\n" +
							"and must not start with a digit.";
				MessageFlag = true;
				return;
			}

			try
			{
				var newVariable = new ScalpVariable(tokens[1], _types.GetType("String"));
				_variables.AddVariable(newVariable);
				if (tokens.Count == 4 && tokens[2] == "=")
				{
					newVariable.PrimitiveValue = GetStringFromVariableOrLiteral(tokens[3]);
				}
			}
			catch (Exception e) // Exceptions occure when redefining a cymbol
			{
				Message = e.Message;
				MessageFlag = true;
			}
		}

		private string GetStringFromVariableOrLiteral(string argument)
		{
			if (_variables.VariableExists(argument, _types.GetType("String")))
			{
				return _variables.GetVariable(argument).PrimitiveValue as string;
			}
			else if (_variables.VariableExists(argument))
			{
				return $"Error! Variable {argument} is not of type String.";
			}

			else if (argument.StartsWith('"'))
			{
				if (argument == "\"" || !argument.EndsWith('"'))
				{
					return "Error! Expected string end (the closing \" is missing).";
				}
				else // it's a normal string literal
				{
					return StringOperations.TrimQuotes(argument);
				}
			}
			else
			{
				return $"Error! {argument} is not a string literal!";
			}
		}

		private bool IsValidIdentifierName(string name)
		{
			return name.Any(ch => ('a' <= ch && ch <= 'z') ||
								('A' <= ch && ch <= 'Z') ||
								('0' <= ch && ch <= '9') ||
								ch == '_' || ch == '-')
				&&
				!('0' <= name[0] && name[0] <= '9');
		}
	}
}
