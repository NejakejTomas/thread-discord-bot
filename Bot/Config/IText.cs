using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.Config
{
	interface IText
	{
		string NextText { get; }
		string PrevText { get; }
	}
}
