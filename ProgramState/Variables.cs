using System;
using System.Collections.Generic;
using System.Text;

using Scalp.CoreClasses;

namespace Scalp.ProgramState
{
	class Variables
	{
		private Dictionary<string, ScalpVariable> _globals;
		private List<ScalpScope> _scopes;

		private readonly Types _types;

		public Variables(Types types)
		{
			_globals = new Dictionary<string, ScalpVariable>();
			_scopes = new List<ScalpScope>();
			_types = types;
		}

		public bool VariableExists(string variableName)
		{
			return GetVariable(variableName) != null;
		}

		public bool VariableExists(string variableName, ScalpType variableType)
		{
			var variableInMemory = GetVariable(variableName);
			
			return variableInMemory != null &&
					variableInMemory.Type == variableType;
		}

		public ScalpVariable GetVariable(string variableName)
		{
			for (int i = _scopes.Count - 1; i >= 0; i--)
			{
				if (_scopes[i].Variables.ContainsKey(variableName))
				{
					return _scopes[i].Variables[variableName];
				}

				if (_scopes[i].HidesPrev)
				{
					break;
				}
			}

			return _globals.ContainsKey(variableName) ? _globals[variableName] : null;
		}

		public void AddVariable(ScalpVariable variable)
		{
			if (_types.TypeExists(variable.Name))
			{
				throw new Exception($"Name \"{variable.Name}\" is already registered as a type!");
			}
			else if (VariableExists(variable.Name))
			{
				throw new Exception($"Redefinition of variable \"{variable.Name}\"!");
			}
			else
			{
				if (_scopes.Count > 0)
				{
					_scopes[_scopes.Count - 1].Variables.Add(variable.Name, variable);
				}
				else
				{
					_globals.Add(variable.Name, variable);
				}
			}
		}

		public void EnterNewScope()
		{
			_scopes.Add(new ScalpScope(false));
		}

		public void LeaveScope()
		{
			if (_scopes.Count > 0)
			{
				_scopes.RemoveAt(_scopes.Count - 1);
			}
			else
			{
				throw new Exception("There's no scope to close.");
			}
		}
	}
}
