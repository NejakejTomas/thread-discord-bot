using Discord.WebSocket;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.Bot
{
	class MessageHandler : IMessageHandler
	{
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();

		public Task HandleMessage(SocketMessage message)
		{
			_logger.Trace(message);

			return Task.CompletedTask;
		}
	}
}
