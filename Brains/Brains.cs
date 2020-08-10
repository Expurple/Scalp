using System;
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
		public bool PrintFlag { get; private set; }

		public string PrintContents { get; private set; }

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
			PrintFlag = false;
			var tokens = _tokenizer.Tokenize(input);

			if (tokens.Count == 0)
			{
				return; // Ignore empty lines
			}

			// Language has no functions yet, so we treat exit() as a special case
			if (tokens.Count >= 3 && tokens[0].value == "exit" &&
				tokens[1].value == "(" && tokens[2].value == ")")
			{
				ExitFlag = true;
				return;
			}

			// Language has no functions yet, so we treat print() as a special case
			if (tokens.Count >= 4 && tokens[0].value == "print" &&
				tokens[1].value == "(" && tokens[3].value == ")")
			{
				PrintContents = GetStringRvalue(tokens[2]);
				if (PrintContents == null)
				{
					PrintContents = "null";
				}
				PrintFlag = true;
				return;
			}

			// As for it is now, string definition is a special case
			if (tokens[0].value == "String" && tokens.Count > 1)
			{
				ReactAtStringDefinition(tokens);
				return;
			}

			// As for it is now, string assignment is a special case
			if (_variables.VariableExists(tokens[0].value, _types.GetType("String")) &&
				tokens.Count == 3 && tokens[1].value == "=")
			{
				_variables.GetVariable(tokens[0].value).PrimitiveValue =
									GetStringRvalue(tokens[2]);
				return;
			}

			throw new Exception("The grammar of this line is incorrect. What did you mean by that?");
		}

		private void ReactAtStringDefinition(List<ScalpToken> tokens)
		{
			if (!IsValidIdentifierName(tokens[1].value))
			{
				throw new Exception($"\"{tokens[1]}\" is an invalid identifier.\n" +
							"Identifiers must only contain letters, digits, underscores or dashes\n" +
							"and must not start with a digit.");
			}

			var newVariable = new ScalpVariable(tokens[1].value, _types.GetType("String"));
			_variables.AddVariable(newVariable); // throws redefinition exceptions
			if (tokens.Count == 4 && tokens[2].value == "=")
			{
				newVariable.PrimitiveValue = GetStringRvalue(tokens[3]);
			}
		}

		private string GetStringRvalue(ScalpToken token)
		{
			if (token.kind == ScalpToken.Kind.StringLiteral)
			{
				return StringOperations.TrimQuotes(token.value);
			}
			else if (_variables.VariableExists(token.value, _types.GetType("String")))
			{
				return _variables.GetVariable(token.value).PrimitiveValue as string;
			}
			else if (_variables.VariableExists(token.value))
			{
				throw new Exception($"Variable \"{token.value}\" is not of type String.");
			}
			else if (_types.TypeExists(token.value))
			{
				throw new Exception($"Expected a string instance instead of typename \"{token.value}\".");
			}
			else
			{
				throw new Exception($"Unknown identifier \"{token.value}\".");
			}
		}

		private bool IsValidIdentifierName(string name)
		{
			return name.All(ch => ('a' <= ch && ch <= 'z') ||
								('A' <= ch && ch <= 'Z') ||
								('0' <= ch && ch <= '9') ||
								ch == '_' || ch == '-')
				&&
				!('0' <= name[0] && name[0] <= '9');
		}
	}
}
