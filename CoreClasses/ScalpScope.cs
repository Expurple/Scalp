using System;
using System.Collections.Generic;
using System.Text;

namespace Scalp.CoreClasses
{
	class ScalpScope
	{
		public Dictionary<string, ScalpVariable> Variables;
		public readonly bool HidesPrev;

		public ScalpScope(bool hidesPrev)
		{
			HidesPrev = hidesPrev;
			Variables = new Dictionary<string, ScalpVariable>();
		}
	}
}
