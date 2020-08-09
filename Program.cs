using System;

namespace Scalp
{
	class Program
	{
		static void Main(string[] args)
		{
			var brains = new Brains.Brains();

			Console.WriteLine("Scalp");
			Console.WriteLine("Visit https://github.com/Expurple/Scalp for more info.");
			while (true)
			{
				Console.Write(">>> ");
				string input = Console.ReadLine();
				brains.ReactAt(input);

				if (brains.MessageFlag)
				{
					Console.Write(brains.Message);
				}
				if (brains.ExitFlag)
				{
					break;
				}
			}
		}
	}
}