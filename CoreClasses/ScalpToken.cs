using System;
using System.Collections.Generic;
using System.Text;

namespace Scalp.CoreClasses
{
	struct ScalpToken
	{
		public string value;
		public Kind kind;
		
		public ScalpToken(string value, Kind kind)
		{
			this.value = value;
			this.kind = kind;
		}

		public enum Kind
		{
			Identifier, Character, Keyword, StringLiteral, CharLiteral
		}
	}
}
