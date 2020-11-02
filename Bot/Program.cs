using System;
using System.Threading.Tasks;
using Bot;

namespace Bot
{
	class Program
	{
		static async Task Main(string[] args)
		{
			Bot.Bot bot = new();
			await bot.Start();
		}
	}
}
