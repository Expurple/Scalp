using System;
using System.Collections.Generic;
using System.Text;

namespace Scalp.CoreClasses
{
	class ScalpVariable
	{
		public readonly string Name;
		public readonly ScalpType Type;
		public Object PrimitiveValue { get; set; }

		public ScalpVariable(string name, ScalpType type)
		{
			Name = name;
			Type = type;
			PrimitiveValue = null;
		}

		public void CopyValueFrom(ScalpVariable source)
		{
			if (this.Type != source.Type)
			{
				throw new ArgumentException(
					$"Types \"{this.Type.TypeName}\" and \"{source.Type.TypeName}\" do not match!");
			}

			this.PrimitiveValue = source.PrimitiveValue;
		}
	}
}
