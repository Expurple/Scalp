using System;
using System.Linq;

namespace Scalp
{
	class Program
	{
		static void Main(string[] args)
		{
			var state = new ProgramState.FullProgramState();
			var brains = new Brains.Brains(state);

			Console.WriteLine($"Scalp {GlobalConstants.VERSION} ({GlobalConstants.VERSION_DATE})");
			Console.WriteLine($"Visit {GlobalConstants.GITHUB_REPO_LINK} for more info.");
			
			while (true)
			{
				Console.Write(">>> ");
				string input = Console.ReadLine();

				try
				{
					brains.ReactAt(input);
				}
				catch (Exception e)
				{
					DisplayErrorPos(brains.ErrorPos, input);
					Console.Write($"Scalp error: {e.Message}");
					Console.Write(e.Message.EndsWith('\n') ? "" : "\n");
					// Cause ">>> " must be on the new line every time
				}

				if (brains.PrintFlag)
				{
					Console.Write(brains.PrintContents);
					Console.Write(brains.PrintContents.EndsWith('\n') ? "" : "\n");
					// Cause ">>> " must be on the new line every time
				}

				if (brains.ExitFlag)
				{
					break;
				}
			}
		}

		private static void DisplayErrorPos(int pos, string input)
		{
			int nTabs = input[..pos].Count(ch => ch == '\t');
			var tabs = new String('\t', nTabs);
			var spaces = new String(' ', pos - nTabs + ">>> ".Length);
			Console.WriteLine(spaces + tabs + "^");
		}
	}
}