using System;
using System.Collections.Generic;
using System.Text;

namespace Scalp.CoreClasses
{
	struct ScalpToken
	{
		public string value;
		public Kind kind;
		public int posInSourceLine; // it's mostly useful for error messages
		
		public ScalpToken(string value, Kind kind, int posInSourceLine)
		{
			this.value = value;
			this.kind = kind;
			this.posInSourceLine = posInSourceLine;
		}

		public enum Kind
		{
			Identifier, Character, Keyword, BoolLiteral, StringLiteral, CharLiteral
		}
	}
}
