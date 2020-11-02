using Bot.Encoding;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Bot.Bot.MessageHandlers
{
	class ReplyToThread : IHandleableMessage
	{
		private static readonly Regex Pattern = new(@"\A>\s\*([" + new string(BaseConverter.Alphabet) + @"]{16})\*\s<@!?\d{1,20}>\s(.*?)\z", RegexOptions.Singleline);
		public async Task<bool> HandleAsync(SocketMessage message)
		{
			Match match = Pattern.Match(message.Content);
			if (!match.Success) return false;
			string? textId = match.Groups[1].Captures[0].Value;
			string? text = match.Groups[2].Captures[0].Value;
			if (string.IsNullOrEmpty(text)) return false;

			byte[] id = BaseConverter.FromMyBase(textId);
			if (BitConverter.IsLittleEndian) Array.Reverse(id);

			return await Thread.Reply(BitConverter.ToUInt64(id), message.Channel, message.Author, text);
		}
	}
}
