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
	// and probably returning something to print or an error message.
	class Brains
	{
		public int ErrorPos { get; private set; }
		public bool ExitFlag { get; private set; }
		public bool PrintFlag { get; private set; }

		public string PrintContents { get; private set; }

		private readonly Tokenizer _tokenizer;
		private List<ScalpToken> _tokens;

		private readonly FullProgramState _state;
		private readonly Types _types;
		private readonly Variables _variables;

		public Brains(FullProgramState programState)
		{
			_tokenizer = new Tokenizer(pos => this.ErrorPos = pos);
			_state = programState;
			_types = _state.Types;
			_variables = _state.Variables;
		}

		public void ReactAt(string input)
		{
			PrintFlag = false;
			_tokens = _tokenizer.Tokenize(input);

			if (_state.IfStack.Count != 0)
			{
				if (_tokens.Count > 0 && _tokens[0].value == "endif")
				{
					if (_tokens.Count > 1)
					{
						ErrorPos = _tokens[0].posInSourceLine + 4;
						throw new Exception("Expected a new line after \"endif\".");
					}
					_state.IfStack.Pop();
					return;
				}

				if (_state.IfStack.Any(condition => condition == false)
					&& !(_tokens.Count > 0 && _tokens[0].value == "if")) // need to register ifs
				{
					return;
				}
			}

			ReactAtTokens();
		}

		private void ReactAtTokens()
		{
			if (_tokens.Count == 0)
			{
				return; // Ignore empty lines
			}

			// Language has no functions yet, so we treat exit() as a special case
			if (_tokens.Count == 3 && _tokens[0].value == "exit" &&
				_tokens[1].value == "(" && _tokens[2].value == ")")
			{
				ExitFlag = true;
				return;
			}

			// Language has no functions yet, so we treat print() as a special case
			if (_tokens.Count == 4 && _tokens[0].value == "print" &&
				_tokens[1].value == "(" && _tokens[3].value == ")")
			{
				PrintContents = TryGetPrintContents();
				PrintFlag = true;
				return;
			}

			// Variable declaration
			if (_types.TypeExists(_tokens[0].value) && (_tokens.Count == 2 || _tokens.Count == 4))
			{
				ReactAtVariableDeclaration();
				return;
			}

			// Assign value to an existing variable
			if (_tokens.Count == 3 && _tokens[1].value == "=")
			{
				ReactAtVariableAssign();
				return;
			}

			// If statement
			if (_tokens[0].value == "if")
			{
				ReactAtIfStatement();
				return;
			}

			ErrorPos = _tokens[0].posInSourceLine;
			throw new Exception("The grammar of this line is incorrect. Interpreter can't figure it out.\n" +
				$"You can learn more about Scalp syntax at {GlobalConstants.GITHUB_WIKI_LINK}");
		}

		private string TryGetPrintContents()
		{
			// An "overload" for Bool
			try
			{
				var boolToPrint = GetRvalue("Bool", _tokens[2]);
				return boolToPrint.PrimitiveValue == null ? "null" :
					((bool)boolToPrint.PrimitiveValue == true ? "true" : "false");
			}
			catch { }

			// An "overload" for String
			var stringToPrint = GetRvalue("String", _tokens[2]);
			PrintContents = stringToPrint.PrimitiveValue as string;
			if (PrintContents == null)
			{
				PrintContents = "null";
			}
			return PrintContents;
		}

		private void ReactAtVariableDeclaration()
		{
			if (Tokenizer.KEYWORDS.Contains(_tokens[1].value))
			{
				ErrorPos = _tokens[1].posInSourceLine;
				throw new Exception($"Keyword \"{_tokens[1].value}\" can't be used as a variable name.");
			}

			var newVariable = new ScalpVariable(_tokens[1].value,
												_types.GetType(_tokens[0].value));

			if (_tokens.Count == 4 && _tokens[2].value == "=")
			{
				newVariable.CopyValueFrom(
						GetRvalue(newVariable.Type.TypeName, _tokens[3]));
			}

			_variables.AddVariable(newVariable);
		}

		private void ReactAtVariableAssign()
		{
			if (_variables.VariableExists(_tokens[0].value))
			{
				var modifiableVariable = _variables.GetVariable(_tokens[0].value);
				modifiableVariable.CopyValueFrom(
						GetRvalue(modifiableVariable.Type.TypeName, _tokens[2]));
			}
			else
			{
				ErrorPos = _tokens[0].posInSourceLine;
				throw new Exception($"Unknown identifier \"{_tokens[0].value}\".");
			}
		}

		private void ReactAtIfStatement()
		{
			if (_tokens.Count == 1)
			{
				ErrorPos = _tokens[0].posInSourceLine + 1;
				throw new Exception("Expected a condition after an \"if\" statement.");
			}

			ScalpVariable conditionIsTrue = GetRvalue("Bool", _tokens[1]);

			if (_tokens.Count > 2)
			{
				ErrorPos = _tokens[1].posInSourceLine + _tokens[1].value.Length - 1;
				throw new Exception($"Expected new line after the condition (\"{_tokens[1].value}\").");
			}

			_state.IfStack.Push((bool)conditionIsTrue.PrimitiveValue);
		}

		private ScalpVariable GetRvalue(string expectedType, ScalpToken token)
		{
			// Creating a tempopary string from a literal is a type-specific case
			if (expectedType == "String" && token.kind == ScalpToken.Kind.StringLiteral)
			{
				var stringFromLiteral = new ScalpVariable("", _types.GetType("String"));
				stringFromLiteral.PrimitiveValue = StringOperations.TrimQuotes(token.value);
				return stringFromLiteral;
			}
			// As is creating a bool from a literal
			if (expectedType == "Bool" && token.kind == ScalpToken.Kind.BoolLiteral)
			{
				var boolFromLiteral = new ScalpVariable("", _types.GetType("Bool"));
				boolFromLiteral.PrimitiveValue = (token.value == "true" ? true : false);
				return boolFromLiteral;
			}

			else if (_variables.VariableExists(token.value, _types.GetType(expectedType)))
			{
				return _variables.GetVariable(token.value);
			}
			else if (_variables.VariableExists(token.value))
			{
				ErrorPos = token.posInSourceLine;
				throw new Exception($" Wrong type of variable \"{token.value}\".\n" +
					$"Expected: \"{expectedType}\", got: \"{_variables.GetVariable(token.value).Type.TypeName}\".");
			}
			else if (_types.TypeExists(token.value))
			{
				ErrorPos = token.posInSourceLine;
				throw new Exception($"Expected a {expectedType} instance instead of typename \"{token.value}\".");
			}
			else if (token.kind == ScalpToken.Kind.StringLiteral)
			{
				ErrorPos = token.posInSourceLine;
				throw new Exception($"Expected a {expectedType} instance instead of a String literal.");
			}
			else if (token.kind == ScalpToken.Kind.CharLiteral)
			{
				ErrorPos = token.posInSourceLine;
				throw new Exception($"Expected a {expectedType} instance instead of a Char literal.");
			}
			else if (token.kind == ScalpToken.Kind.BoolLiteral)
			{
				ErrorPos = token.posInSourceLine;
				throw new Exception($"Expected a {expectedType} instance instead of a Bool literal.");
			}
			else if (token.kind == ScalpToken.Kind.Keyword)
			{
				ErrorPos = token.posInSourceLine;
				throw new Exception($"Expected a {expectedType} instance instead of a keyword \"{token.value}\".");
			}
			else
			{
				ErrorPos = token.posInSourceLine;
				throw new Exception($"Unknown identifier \"{token.value}\".");
			}
		}
	}
}
