using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using Scalp.Brains;

namespace Tests.Functionality
{
	// Functionality is always tested upon Brains, so I've hidden the setup in here.
	public abstract class BrainsSetupBase
	{
		private protected Brains brains;

		[SetUp]
		public void SetupBrains()
		{
			brains = new Brains();
		}
	}
}
