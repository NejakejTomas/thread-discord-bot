using Bot.Config;
using Discord.WebSocket;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.Bot
{
	sealed class Bot : IDisposable
	{
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();
		private readonly DiscordSocketClient _client = new();
		private readonly IMessageHandler _handler;

		public Bot()
		{
			_handler = new MessageHandler();
		}

		public async Task Start()
		{
			string token = JsonConfig.GetConfig().Token;

			_logger.Info("Connecting");
			await Task.WhenAll(
				_client.LoginAsync(Discord.TokenType.Bot, token),
				_client.StartAsync());

			_client.Connected += () =>
			{
				_logger.Info("Connected");

				return Task.CompletedTask;
			};

			_client.MessageReceived += _handler.HandleMessage;

			await Task.Delay(-1);
		}

		public void Dispose()
		{
			_client.Dispose();
		}
	}
}
