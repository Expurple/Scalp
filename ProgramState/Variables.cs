using System;
using System.Collections.Generic;
using System.Text;

using Scalp.CoreClasses;

namespace Scalp.ProgramState
{
	class Variables
	{
		private Dictionary<string, ScalpVariable> _variables;

		public bool VariableExists(string variableName)
		{
			return _variables.ContainsKey(variableName);
		}

		public ScalpVariable GetVariable(string variableName)
		{
			return VariableExists(variableName) ? _variables[variableName] : null;
		}

		public void AddVariable(ScalpVariable variable)
		{
			if (VariableExists(variable.Name) && variable != GetVariable(variable.Name))
			{
				throw new Exception($"Error: redefinition of variable {variable.Name}!");
			}
			else
			{
				_variables.Add(variable.Name, variable);
			}
		}

		public Variables()
		{
			_variables = new Dictionary<string, ScalpVariable>();
		}
	}
}
