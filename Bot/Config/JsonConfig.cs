using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NLog;

namespace Bot.Config
{
	static class JsonConfig
	{
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

		public static IConfig GetConfig()
		{
			const string file = "config.json";

			try
			{
				return GetConfig(file);
			}
			catch (IOException e)
			{
				_logger.Error(e);

				throw;
			}
		}

		private static Config GetConfig(string file)
		{
			Config? config;

			using (StreamReader fileStream = File.OpenText(file))
			{
				JsonReader reader = new JsonTextReader(fileStream);
				JsonSerializer serializer = new JsonSerializer();
				config = serializer.Deserialize<Config>(reader);
			}

			if (config != null) return config;

			throw new IOException($"Error parsing config file {file}");
		}
	}
}
