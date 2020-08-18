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
		public void ShouldPrintNormalStuff()
		{
			brains.ProcessLineOfCode("print(\"Hello World!\")");
			Assert.IsTrue(brains.PrintFlag);
			Assert.AreEqual("Hello World!", brains.PrintContents);

			brains.ProcessLineOfCode("String s = \"Hello World!\"");
			brains.ProcessLineOfCode("print(s)");
			Assert.IsTrue(brains.PrintFlag);
			Assert.AreEqual("Hello World!", brains.PrintContents);

			brains.ProcessLineOfCode("print(false)");
			Assert.IsTrue(brains.PrintFlag);
			Assert.AreEqual("false", brains.PrintContents);
		}

		[Test]
		public void ShouldPrintNullVariables()
		{
			brains.ProcessLineOfCode("String nullString");
			brains.ProcessLineOfCode("print(nullString)");
			Assert.IsTrue(brains.PrintFlag);
			Assert.AreEqual("null", brains.PrintContents);
		}

		[Test]
		public void ShouldFailWithThese()
		{
			Assert.Throws<Exception>(
				() => { brains.ProcessLineOfCode("print(undeclaredVariable)"); });

			Assert.Throws<Exception>(
				() => { brains.ProcessLineOfCode("print('c')"); });

			Assert.Throws<Exception>(
				() => { brains.ProcessLineOfCode("print(arg1, arg2)"); });

			Assert.Throws<Exception>(
				() => { brains.ProcessLineOfCode("print(String)"); });

			Assert.Throws<Exception>(
				() => { brains.ProcessLineOfCode("print()"); });
		}
	}
}
