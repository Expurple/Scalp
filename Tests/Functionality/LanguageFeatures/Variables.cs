using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

namespace Tests.Functionality.LanguageFeatures
{
	public class Variables : BrainsSetupBase
	{
		[Test]
		public void VariablesGetCreatedAndCanBeAssigned()
		{
			brains.ProcessLineOfCode("Bool foo = true");
			brains.ProcessLineOfCode("Bool bar = foo");
			brains.ProcessLineOfCode("print(bar)");

			Assert.AreEqual(true, brains.PrintFlag);
			Assert.AreEqual("true", brains.PrintContents);
		}

		[Test]
		public void CheckTypeChecking()
		{
			brains.ProcessLineOfCode("String someString");

			Assert.Throws<Exception>(
				() => { brains.ProcessLineOfCode("someString = true"); },
				"Should've been a type error");
			Assert.Throws<Exception>(
				() => { brains.ProcessLineOfCode("Bool someBool = someString"); },
				"Should've been a type error");
		}

		[Test]
		public void RedefinitionsAreBad()
		{
			brains.ProcessLineOfCode("String foo");

			Assert.Throws<Exception>(
				() => { brains.ProcessLineOfCode("Bool foo"); },
				"Should've been a redefinition error");
		}
	}
}
