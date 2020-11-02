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

		public static async Task CreateEmptyAsync(IMessageChannel channel, IUser author, string text = "")
		{
			Task<IUserMessage> placeholderTask = CreatePlaceholderAsync(channel);

			Tuple<IEnumerable<EmbedFieldBuilder>, int> textFields = FromText(author, text);

			EmbedBuilder builder = new EmbedBuilder()
				.WithAuthor(author)
				.WithFields(textFields.Item1)
				.AddField(EmptyChar, EmptyChar, true)
				.AddField(EmptyChar, EmptyChar, true)
				.WithFooter($"{textFields.Item2}/{MaxTextLength}")
				.WithColor(GetUserColor(author));

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

		private static Tuple<IEnumerable<EmbedFieldBuilder>, int> FromText(IUser author, string text)
		{
			const int FieldLength = 1000;
			List<EmbedFieldBuilder> fields = new();
			int length = 0;

			for (int i = 0; i < text.Length; i += FieldLength)
			{
				string value;
				if (i == 0) value = $"{author.Mention}: {text[i..Math.Min(text.Length, i + FieldLength)]}";
				else value = text[i..Math.Min(text.Length, i + FieldLength)];

				EmbedFieldBuilder embedFieldBuilder = new EmbedFieldBuilder()
					.WithIsInline(false)
					.WithName(EmptyChar)
					.WithValue(value);

				fields.Add(embedFieldBuilder);
				length += value.Length + EmptyChar.Length;
			}

			return new(fields, length);
		}
	}
}
