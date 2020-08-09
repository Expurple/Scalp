using System;
using System.Collections.Generic;
using System.Text;

namespace Scalp.Utility
{
	static class StringOperations
	{
		public static string RemoveStringQuotes(string orig)
		{
			return orig[1..^1];
		}
	}
}
