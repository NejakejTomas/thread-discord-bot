using Bot.Config;
using Bot.Encoding;
using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemporaryWorkaround.Common;

namespace Bot.Bot
{
	static class Thread
	{
		private const string EmptyChar = "\u200B";
		private const int MaxTextLength = 5500;
		private const int MaxEmbedsCount = 20;
		private const int UtilityFieldsCount = 2;
		private static readonly WeakReferenceContainer<Tuple<ulong, ulong>, IUserMessage> cache = new(100_000);

		public static async Task<bool> CreateEmptyAsync(IMessageChannel channel, IUser author, string text = "", ulong replyTo = 0)
		{
			Task<IUserMessage> placeholderTask;
			Tuple<IEnumerable<EmbedFieldBuilder>, int> replyingToMessageFields;

			if (replyTo != 0)
			{
				IMessage? replyingTo = await channel.GetMessageAsync(replyTo);
				if (replyingTo == null) return false;

				placeholderTask = CreatePlaceholderAsync(channel);
				replyingToMessageFields = FieldsFromText(replyingTo.Author, replyingTo.Content);
			}
			else
			{
				placeholderTask = CreatePlaceholderAsync(channel);
				replyingToMessageFields = new Tuple<IEnumerable<EmbedFieldBuilder>, int>(new List<EmbedFieldBuilder>(), 0);
			}

			Tuple<IEnumerable<EmbedFieldBuilder>, int> replyMessageFields = FieldsFromText(author, text);

			EmbedBuilder builder = new EmbedBuilder()
				.WithAuthor(author)
				.WithFields(replyingToMessageFields.Item1)
				.WithFields(replyMessageFields.Item1)
				.AddField(EmptyChar, EmptyChar, true)
				.AddField(EmptyChar, EmptyChar, true)
				.WithFooter($"{replyMessageFields.Item2 + replyingToMessageFields.Item2}/{MaxTextLength}")
				.WithColor(GetUserColor(author));

			IUserMessage placeholder = await placeholderTask;

			await placeholder.ModifyAsync(message =>
			{
				message.Content = $"*{BaseConverter.ToMyBase(placeholder.Id)}*";
				message.Embed = builder.Build();
			});

			return true;
		}

		public static async Task<bool> Reply(ulong threadMessage, IMessageChannel channel, IUser author, string text)
		{
			IUserMessage? thread = await GetThreadMessageAsync(threadMessage, channel);
			if (thread == null) return false;

			IUserMessage lastThreadMessage = await GetLastThreadAsync(thread);
			Tuple<IEnumerable<EmbedFieldBuilder>, int> replyMessageFields = FieldsFromText(author, text);
			int currentLength = GetLength(lastThreadMessage);
			EmbedBuilder builder = lastThreadMessage.Embeds.First().ToEmbedBuilder();

			foreach (EmbedFieldBuilder embedFieldBuilder in replyMessageFields.Item1)
			{
				if (currentLength + FieldLength(embedFieldBuilder) > MaxTextLength || lastThreadMessage.Embeds.Count - UtilityFieldsCount + 1 > MaxTextLength)
				{
					Task<IUserMessage> createTask = CreateNextAsync(channel, builder.Author, builder.Color ?? new(0), lastThreadMessage);
					builder.WithFooter($"{currentLength}/{MaxTextLength}");

					currentLength = 0;
					await Task.WhenAll(lastThreadMessage.ModifyAsync(message => message.Embed = builder.Build()), createTask);

					lastThreadMessage = await createTask;
					builder = lastThreadMessage.Embeds.First().ToEmbedBuilder();
				}

				builder.Fields.Insert(builder.Fields.Count - UtilityFieldsCount, embedFieldBuilder);
				currentLength += FieldLength(embedFieldBuilder);
			}

			builder.WithFooter($"{currentLength}/{MaxTextLength}");
			await lastThreadMessage.ModifyAsync(message => message.Embed = builder.Build());

			return true;
		}

		public static async Task<IUserMessage> CreateNextAsync(IMessageChannel channel, EmbedAuthorBuilder author, Color color, IUserMessage prev)
		{
			Task<IUserMessage> placeholderTask = CreatePlaceholderAsync(channel);

			EmbedBuilder builder = new EmbedBuilder()
				.WithAuthor(author)
				.AddField(EmptyChar, $"[{JsonConfig.GetConfig().PrevText}]({prev.GetJumpUrl()})", true)
				.AddField(EmptyChar, EmptyChar, true)
				.WithFooter($"0/{MaxTextLength}")
				.WithColor(color);

			IUserMessage placeholder = await placeholderTask;

			await Task.WhenAll(
				placeholder.ModifyAsync(message =>
				{
					message.Content = $"*{BaseConverter.ToMyBase(placeholder.Id)}*";
					message.Embed = builder.Build();
				}),
				SetNext(prev, placeholder.GetJumpUrl()));

			return placeholder;
		}

		private static async Task<IUserMessage?> GetThreadMessageAsync(ulong id, IMessageChannel channel)
		{
			IUserMessage? thread;

			lock (cache) thread = cache.Get(new Tuple<ulong, ulong>(channel.Id, id));

			if (thread != null) return thread;

			thread = await channel.GetMessageAsync(id) as IUserMessage;
			if (thread != null) lock (cache) cache.Add(new Tuple<ulong, ulong>(channel.Id, id), thread);

			return thread;
		}

		private static async Task<IUserMessage> GetLastThreadAsync(IUserMessage threadMessage)
		{
			ulong next = GetNext(threadMessage);
			if (next == 0) return threadMessage;

			return await GetLastThreadAsync((await GetThreadMessageAsync(next, threadMessage.Channel))!);
		}

		private static ulong GetNext(IUserMessage threadMessage)
		{
			IEmbed embed = threadMessage.Embeds.First();
			if (embed.Fields[^1].Value == EmptyChar) return 0;

			return ulong.Parse(embed.Fields[^1].Value.Split(new char[] { '/', ')' })[6]);
		}

		private static int GetLength(IUserMessage threadMessage)
		{
			IEmbed embed = threadMessage.Embeds.First();
			string[] footerText = embed.Footer!.Value.Text.Split('/');

			return int.Parse(footerText[0]);
		}

		private static async Task SetPrev(IUserMessage threadMessage, string jumpUrl)
		{
			EmbedBuilder builder = threadMessage.Embeds.First().ToEmbedBuilder();
			builder.Fields[^2].Value = $"[{JsonConfig.GetConfig().PrevText}]({jumpUrl})";

			await threadMessage.ModifyAsync(message => message.Embed = builder.Build());
		}

		private static async Task SetNext(IUserMessage threadMessage, string jumpUrl)
		{
			EmbedBuilder builder = threadMessage.Embeds.First().ToEmbedBuilder();
			builder.Fields[^1].Value = $"[{JsonConfig.GetConfig().NextText}]({jumpUrl})";

			await threadMessage.ModifyAsync(message => message.Embed = builder.Build());
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

		private static int FieldLength(EmbedFieldBuilder embedFieldBuilder)
		{
			return ((string)embedFieldBuilder.Value).Length + ((string)embedFieldBuilder.Name).Length;
		}

		private static Tuple<IEnumerable<EmbedFieldBuilder>, int> FieldsFromText(IUser author, string text)
		{
			const int MaxTextLength = 1000;
			List<EmbedFieldBuilder> fields = new();
			int length = 0;

			for (int i = 0; i < text.Length; i += MaxTextLength)
			{
				string value;
				if (i == 0) value = $"{author.Mention}: {text[i..Math.Min(text.Length, i + MaxTextLength)]}";
				else value = text[i..Math.Min(text.Length, i + MaxTextLength)];

				EmbedFieldBuilder embedFieldBuilder = new EmbedFieldBuilder()
					.WithIsInline(false)
					.WithName(EmptyChar)
					.WithValue(value);

				fields.Add(embedFieldBuilder);
				length += FieldLength(embedFieldBuilder);
			}

			return new(fields, length);
		}
	}
}
