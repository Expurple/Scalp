﻿using System;

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
				if (input == "exit()")
				{
					break;
				}
				Console.Write(brains.ReactionAt(input));
			}
		}
	}
}