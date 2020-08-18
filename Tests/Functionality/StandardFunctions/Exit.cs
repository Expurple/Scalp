using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

namespace Tests.Functionality.StandardFunctions
{
	class Exit : BrainsSetupBase
	{
		[Test]
		public void ShouldExit()
		{
			brains.ProcessLineOfCode("exit()");
			Assert.IsTrue(brains.ExitFlag);
		}

		[Test]
		public void ShouldTakeNoArguments()
		{
			Assert.Throws<Exception>(
				() => { brains.ProcessLineOfCode("exit(0)"); });

			Assert.Throws<Exception>(
				() => { brains.ProcessLineOfCode("exit(\"Bad stuff happened.\")"); });
		}
	}
}
