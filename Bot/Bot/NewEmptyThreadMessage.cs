using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Bot.Bot
{
	sealed class NewEmptyThreadMessage : IHandleableMessage
	{
		private static readonly Regex Pattern = new(@"\A<@!?\d{1,20}>\z");
		public async Task<bool> HandleAsync(SocketMessage message)
		{
			if (!Pattern.IsMatch(message.Content)) return false;

			await Thread.CreateEmptyAsync(message);

			return true;
		}
	}
}
