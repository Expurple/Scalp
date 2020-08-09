using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Scalp
{
	static class GlobalConstants
	{
		public static readonly string DIGITS = "1234567890";
		public static readonly Regex VALID_IDENTIFIER_CHARS_REGEX =
									new Regex("/[a-zA-Z0-9_-]/");
	}
}
