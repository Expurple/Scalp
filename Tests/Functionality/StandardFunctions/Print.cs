using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using Scalp.Brains;

namespace Tests.Functionality.StandardFunctions
{
	public class Print
	{
		[Test]
		public void HelloWorld()
		{
			Brains brains = new Brains();
			brains.ProcessLineOfCode("print(\"Hello World!\")");
			Assert.IsTrue(brains.PrintFlag);
			Assert.AreEqual("Hello World!", brains.PrintContents);
		}
	}
}
