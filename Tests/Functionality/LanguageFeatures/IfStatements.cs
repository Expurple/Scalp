﻿using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using Scalp.Brains;

namespace Tests.Functionality.LanguageFeatures
{
	public class IfStatements
	{
		[Test]
		public void IfFalseShouldntExecute()
		{
			Brains brains = new Brains();
			brains.ProcessLineOfCode("if false");
			brains.ProcessLineOfCode("	print(\"Test\")");
			Assert.IsFalse(brains.PrintFlag);
		}

		[Test]
		public void IfShouldСreateScope()
		{
			Brains brains = new Brains();
			brains.ProcessLineOfCode("if true");
			brains.ProcessLineOfCode("	String foo");
			brains.ProcessLineOfCode("}");
			try
			{
				brains.ProcessLineOfCode("print(foo)");
				Assert.Fail("Should've been an exception about foo not defined");
			}
			catch (Exception e)
			{
				Assert.Pass("Scalp failed with an error: " + e.Message);
			}
		}

		[Test]
		public void FalseBodyShouldBeCheckedForErrors()
		{
			Brains brains = new Brains();
			brains.ProcessLineOfCode("if false");
			try
			{
				brains.ProcessLineOfCode("	some gibberish here");
				Assert.Fail("Should've been a \"bad grammar\" exception");
			}
			catch (Exception e)
			{
				Assert.Pass("Scalp failed with an error: " + e.Message);
			}
		}

		[Test]
		public void ClosingNotExistingScopeShouldFail()
		{
			Brains brains = new Brains();
			brains.ProcessLineOfCode("if true");
			brains.ProcessLineOfCode("}");
			try
			{
				brains.ProcessLineOfCode("}");
				Assert.Fail("Should've been a \"no scope to close\" exception");
			}
			catch (Exception e)
			{
				Assert.Pass("Scalp failed with an error: " + e.Message);
			}
		}

		[Test]
		public void BraceShouldCloseOneScopeAtATime()
		{
			Brains brains = new Brains();
			brains.ProcessLineOfCode("if false");
			brains.ProcessLineOfCode("	if true");
			brains.ProcessLineOfCode("	}");
			brains.ProcessLineOfCode("	print(\"This slouldn't be printed\")");
			Assert.IsFalse(brains.PrintFlag, "The first \"if\" is somehow closed");
		}
	}
}
