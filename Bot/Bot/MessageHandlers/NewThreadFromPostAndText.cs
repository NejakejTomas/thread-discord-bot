﻿using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Bot.Bot.MessageHandlers
{
	class NewThreadFromPostAndText : IHandleableMessage
	{
		private static readonly Regex Pattern = new(@"\A<@!?\d{1,20}>\s(\d{1,20})\s(.*?)\z", RegexOptions.Singleline);
		public async Task<bool> HandleAsync(SocketMessage message)
		{
			Match match = Pattern.Match(message.Content);
			if (!match.Success) return false;
			string? text = match.Groups[2].Captures[0].Value;

			if (!ulong.TryParse(match.Groups[1].Captures[0].Value, out ulong id)) return false;
			if (string.IsNullOrEmpty(text)) return false;

			return await Thread.CreateEmptyAsync(message.Channel, message.Author, text, id);
		}
	}
}
