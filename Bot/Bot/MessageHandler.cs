using Bot.Bot.MessageHandlers;
using Discord;
using Discord.WebSocket;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Bot.Bot
{
	sealed class MessageHandler : IMessageHandler
	{
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();
		private readonly SocketSelfUser _self;
		private readonly IEnumerable<IHandleableMessage> _handlers;

		public MessageHandler(SocketSelfUser self)
		{
			_self = self;
			_handlers = new List<IHandleableMessage>
			{
				new NewThreadFromPost(),
				new NewThreadFromPostAndText(),
				new NewThreadFromText(),
				new NewEmptyThread(),
				new ReplyToThread()
			};
		}

		public async Task HandleMessageAsync(SocketMessage message)
		{
			if (!IsPing(message)) return;

			_logger.Trace(message);
			await SortOutMessageTypeAsync(message);
		}

		private bool IsPing(SocketMessage message)
		{
			foreach (ITag tag in message.Tags)
			{
				if (tag.Key == _self.Id) return true;
			}

			return false;
		}

		private async Task SortOutMessageTypeAsync(SocketMessage message)
		{
			foreach (IHandleableMessage handler in _handlers)
			{
				if (await handler.HandleAsync(message) == true) return;
			}
		}
	}
}
