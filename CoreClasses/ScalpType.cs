using System;
using System.Collections.Generic;
using System.Text;

namespace Scalp.CoreClasses
{
	class ScalpType
	{
		public readonly string TypeName;
		public Object PrimitiveValue { get; set; }

		public ScalpType(string TypeName, Object PrimitiveValue)
		{
			this.TypeName = TypeName;
			this.PrimitiveValue = PrimitiveValue;
		}
	}
}
