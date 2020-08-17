using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using Scalp.Brains;

namespace Tests.Functionality.LanguageFeatures
{
	public class IfStatements : BrainsSetupBase
	{
		[Test]
		public void IfFalseShouldntExecute()
		{
			brains.ProcessLineOfCode("if false");
			brains.ProcessLineOfCode("	print(\"Test\")");
			Assert.IsFalse(brains.PrintFlag);
		}

		[Test]
		public void IfShouldСreateScope()
		{
			brains.ProcessLineOfCode("if true");
			brains.ProcessLineOfCode("	String foo");
			brains.ProcessLineOfCode("}");
			Assert.Throws<Exception>(
				() => { brains.ProcessLineOfCode("print(foo)"); },
				"Should've been an exception about foo not defined");
		}

		[Test]
		public void FalseBodyShouldBeCheckedForErrors()
		{
			brains.ProcessLineOfCode("if false");
			Assert.Throws<Exception>(
				() => { brains.ProcessLineOfCode("	some gibberish here"); },
				"Should've been a \"bad grammar\" exception");
		}

		[Test]
		public void ClosingNotExistingScopeShouldFail()
		{
			brains.ProcessLineOfCode("if true");
			brains.ProcessLineOfCode("}");
			Assert.Throws<Exception>(
				() => { brains.ProcessLineOfCode("}"); },
				"Should've been a \"no scope to close\" exception");
		}

		[Test]
		public void BraceShouldCloseOneScopeAtATime()
		{
			brains.ProcessLineOfCode("if false");
			brains.ProcessLineOfCode("	if true");
			brains.ProcessLineOfCode("	}");
			brains.ProcessLineOfCode("	print(\"This slouldn't be printed\")");
			Assert.IsFalse(brains.PrintFlag, "The first \"if\" is somehow closed");
		}
	}
}
