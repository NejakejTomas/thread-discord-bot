using Bot.Encoding;
using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.Bot
{
	static class Thread
	{
		private const string EmptyChar = "\u200B";
		private const int MaxTextLength = 5500;

		public static async Task CreateEmptyAsync(IMessage message)
		{
			Task<IUserMessage> placeholderTask = CreatePlaceholderAsync(message.Channel);

			EmbedBuilder builder = new EmbedBuilder()
				.WithAuthor(message.Author)
				.AddField(EmptyChar, EmptyChar, true)
				.AddField(EmptyChar, EmptyChar, true)
				.WithFooter($"0/{MaxTextLength}")
				.WithColor(GetUserColor(message.Author));

			IUserMessage placeholder = await placeholderTask;

			await placeholder.ModifyAsync(message =>
			{
				message.Content = $"*{BaseConverter.ToMyBase(placeholder.Id)}*";
				message.Embed = builder.Build();
			});
		}

		private static Task<IUserMessage> CreatePlaceholderAsync(IMessageChannel channel)
		{
			return channel.SendMessageAsync(EmptyChar);
		}

		private static Color GetUserColor(IUser user)
		{
			if (user.Id == 155951502387707904) return new(0xFF5000);

			return new((uint)(user.Id >> 22) & 0xFFFFFF);
		}
	}
}
