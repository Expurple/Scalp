using System;
using System.Collections.Generic;
using System.Text;

namespace Scalp.Brains
{
	class ErrorChecker
	{
		public delegate void Setter(int pos);
		Setter SetErrorPos;

		public ErrorChecker(Setter meansToSetErrorPos)
		{
			SetErrorPos = meansToSetErrorPos;
		}
	}
}
