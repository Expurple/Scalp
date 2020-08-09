using System;
using System.Collections.Generic;
using System.Text;

namespace Scalp.Utility
{
	static class StringOperations
	{
		public static string TrimQuotes(string orig)
		{
			return orig[1..^1];
		}

		// Clears out all spaces outside of string or char literals.
		public static string RemoveExtraSpacesFromExpression(string expression)
		{
			var result = new StringBuilder();
			bool stringQuotesOpen = false;
			bool charQuotesOpen = false;
			foreach (char ch in expression)
			{
				if (ch == '\"' && !charQuotesOpen)
				{
					stringQuotesOpen = !stringQuotesOpen;
				}
				else if (ch == '\'' && !stringQuotesOpen)
				{
					charQuotesOpen = !charQuotesOpen;
				}

				if (!(ch == ' ' && !(stringQuotesOpen || charQuotesOpen)))
				{
					result.Append(ch);
				}
			}
			return result.ToString();
		}
	}
}
