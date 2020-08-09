﻿using System;
using System.Collections.Generic;
using System.Text;

using Scalp.CoreClasses;

namespace Scalp.ProgramState
{
	class Types
	{
		private Dictionary<string, ScalpType> _types;

		public bool TypeExists(string typeName)
		{
			return _types.ContainsKey(typeName);
		}

		public ScalpType GetType(string typeName)
		{
			return TypeExists(typeName) ? _types[typeName] : null;
		}

		public void AddType(ScalpType type)
		{
			if (TypeExists(type.TypeName) && type != GetType(type.TypeName))
			{
				throw new TypeLoadException($"Error: refinition of type {type.TypeName}!");
			}
			else
			{
				_types.Add(type.TypeName, type);
			}
		}

		public Types()
		{
			_types = new Dictionary<string, ScalpType>();

			// Filling in primitive system types:
			AddType(new ScalpType("String"));
		}
	}
}
