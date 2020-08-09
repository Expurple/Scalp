using System;
using System.Collections.Generic;
using System.Text;

namespace Scalp.Brains
{
	// A top level entity, responsible for the whole process of
	// analyzing an input statement, executing it
	// and probably returning something to print.
	class Brains
	{
		public bool MessageFlag { get; private set; }
		public string Message { get; private set; }

		public void ReactAt(string input)
		{
			// Can't do enything right now
		}
	}
}
