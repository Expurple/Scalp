using System;
using System.Collections.Generic;
using System.Text;

using Scalp.CoreClasses;

namespace Scalp.ProgramState
{
	class Variables
	{
		private Dictionary<string, ScalpVariable> _globals;
		private readonly Types _types;

		public bool VariableExists(string variableName)
		{
			return _globals.ContainsKey(variableName);
		}

		public bool VariableExists(string variableName, ScalpType variableType)
		{
			var variableInMemory = GetVariable(variableName);
			
			return variableInMemory != null &&
					variableInMemory.Type == variableType;
		}

		public ScalpVariable GetVariable(string variableName)
		{
			return VariableExists(variableName) ? _globals[variableName] : null;
		}

		public void AddVariable(ScalpVariable variable)
		{
			if (_types.TypeExists(variable.Name))
			{
				throw new Exception($"Name \"{variable.Name}\" is already registered as a type!");
			}
			else if (VariableExists(variable.Name) && variable != GetVariable(variable.Name))
			{
				throw new Exception($"Redefinition of variable \"{variable.Name}\"!");
			}
			else
			{
				_globals.Add(variable.Name, variable);
			}
		}

		public Variables(Types types)
		{
			_globals = new Dictionary<string, ScalpVariable>();
			_types = types;
		}
	}
}
