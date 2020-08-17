using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using Scalp.Brains;

namespace Tests.Functionality.StandardFunctions
{
	public class Print : BrainsSetupBase
	{
		[Test]
		public void HelloWorld()
		{
			brains.ProcessLineOfCode("print(\"Hello World!\")");
			Assert.IsTrue(brains.PrintFlag);
			Assert.AreEqual("Hello World!", brains.PrintContents);
		}
	}
}
