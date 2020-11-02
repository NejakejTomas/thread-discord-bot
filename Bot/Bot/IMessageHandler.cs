﻿using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.Bot
{
	interface IMessageHandler
	{
		Task HandleMessageAsync(SocketMessage message);
	}
}
