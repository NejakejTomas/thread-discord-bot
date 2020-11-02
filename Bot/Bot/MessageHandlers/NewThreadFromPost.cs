using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Bot.Bot.MessageHandlers
{
	class NewThreadFromPost : IHandleableMessage
	{
		private static readonly Regex Pattern = new(@"\A<@!?\d{1,20}>\s(\d{1,20})\z", RegexOptions.Singleline);
		public async Task<bool> HandleAsync(SocketMessage message)
		{
			Match match = Pattern.Match(message.Content);
			if (!match.Success) return false;

			if (!ulong.TryParse(match.Groups[1].Captures[0].Value, out ulong id)) return false;

			return await Thread.CreateEmptyAsync(message.Channel, message.Author, string.Empty, id);
		}
	}
}
