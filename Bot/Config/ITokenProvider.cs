﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.Config
{
	interface ITokenProvider
	{
		string Token { get; }
	}
}
