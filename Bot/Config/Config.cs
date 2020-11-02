using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Bot.Config
{
	sealed class Config : IConfig
	{
		[JsonProperty("Bot Token")]
		public string Token { get; set; } = string.Empty;

		[JsonProperty("Next Thread Text")]
		public string NextText { get; set; } = string.Empty;

		[JsonProperty("Prev Thread Text")]
		public string PrevText { get; set; } = string.Empty;
	}
}
