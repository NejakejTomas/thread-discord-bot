using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.Bot
{
	interface IHandleableMessage
	{
		public Task<bool> HandleAsync(SocketMessage message);
	}
}
