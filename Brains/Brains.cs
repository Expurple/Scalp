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
	//
	// Essentially, it's a Facade of the whole interpreter.
	class Brains
	{
		public int ErrorPos { get; private set; }
		public bool ExitFlag { get; private set; }
		public bool PrintFlag { get; private set; }

		public string PrintContents { get; private set; }

		private readonly ErrorChecker _errorChecker;
		private readonly Tokenizer _tokenizer;
		private List<ScalpToken> _tokens;

		private readonly FullProgramState _state;
		private readonly Types _types;
		private readonly Variables _variables;

		public Brains()
		{
			_state = new FullProgramState();
			_types = _state.Types;
			_variables = _state.Variables;

			_tokenizer = new Tokenizer(pos => this.ErrorPos = pos);
			_errorChecker = new ErrorChecker(_state, pos => this.ErrorPos = pos);
		}

		public void ReactAt(string input)
		{
			PrintFlag = false;
			_tokens = _tokenizer.Tokenize(input);
			_errorChecker.CheckLineOfCode(_tokens);

			if (_state.IfStack.Count != 0)
			{
				if (_tokens.Count > 0 && _tokens[0].value == "}")
				{
					_state.IfStack.Pop();
					_variables.LeaveScope();
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
				PrintContents = GetPrintContents();
				PrintFlag = true;
				return;
			}

			// Variable declaration
			if (_types.TypeExists(_tokens[0].value))
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
		}

		private string GetPrintContents()
		{
			// An "overload" for Bool
			try
			{
				_errorChecker.CheckTokenForType(_tokens[2], "Bool");
				var boolToPrint = GetRvalue("Bool", _tokens[2]);
				return boolToPrint.PrimitiveValue == null ? "null" :
					((bool)boolToPrint.PrimitiveValue == true ? "true" : "false");
			}
			catch { } // fine, it's not a Bool

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
			var newVariable = new ScalpVariable(_tokens[1].value,
												_types.GetType(_tokens[0].value));

			if (_tokens.Count == 4)
			{
				newVariable.CopyValueFrom(
						GetRvalue(newVariable.Type.TypeName, _tokens[3]));
			}

			_variables.AddVariable(newVariable);
		}

		private void ReactAtVariableAssign()
		{
			var modifiableVariable = _variables.GetVariable(_tokens[0].value);
			modifiableVariable.CopyValueFrom(
					GetRvalue(modifiableVariable.Type.TypeName, _tokens[2]));
		}

		private void ReactAtIfStatement()
		{
			ScalpVariable conditionIsTrue = GetRvalue("Bool", _tokens[1]);
			_state.IfStack.Push((bool)conditionIsTrue.PrimitiveValue);
			_variables.EnterNewScope();
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

			// Normal case:
			return _variables.GetVariable(token.value);
		}
	}
}
