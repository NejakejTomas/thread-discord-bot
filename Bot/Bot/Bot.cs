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
		private IMessageHandler? _handler;

		public async Task StartAsync()
		{
			string token = JsonConfig.GetConfig().Token;

			_logger.Info("Connecting");
			await Task.WhenAll(
				_client.LoginAsync(Discord.TokenType.Bot, token),
				_client.StartAsync());

			_client.Connected += () =>
			{
				_logger.Info("Connected");

				_handler = new MessageHandler(_client.CurrentUser);
				_client.MessageReceived += _handler.HandleMessageAsync;
				return Task.CompletedTask;
			};


			await Task.Delay(-1);
		}

		public void Dispose()
		{
			_client.Dispose();
		}
	}
}
