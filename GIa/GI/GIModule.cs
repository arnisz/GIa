using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GI
{
	public class GIModule
	{
		private string modulType;
		private int channelCount;
		private GIChannel[] gIChannels;
		private bool isInitialized = false;


		private GIGate gIGate;

		public GIModule()
		{
			gIGate = GIGate.Instance;
		}

		public int Adress { get; set; }
		public string ModulType { get; set; }

		public long SerialNumber { get; set; }

		public List<GIChannel> GetGIChannels {
			get 
			{
				if (!isInitialized)
				{
					Initialize();
				}
				return gIChannels.ToList();
			}
			set
			{}
		}

		public int Uart { get; set; }

		public int ChannelsNumber { get { return this.channelCount; } private set { } }
		public string ConfigFile { get; set; }

		// INIT Channels
		// Kanaldaten werden in die Kanalobjekte �bertragen
		public async Task Initialize()
		{
			if (!isInitialized)
			{
				Regex regxDevice = new Regex(buildReg("Device"), RegexOptions.IgnoreCase);
				var device = regxDevice.Match(this.ConfigFile).Value;

				Regex regxChannelsCount = new Regex("VCnt=\\d*");
				string scc = regxChannelsCount.Match(device).Value.Substring(5);

				this.channelCount = int.Parse(scc);

				gIChannels = new GIChannel[channelCount];
				for (int i = 0; i < channelCount; i++)
				{
                    Regex regxV = new Regex(buildReg($"V{i}"), RegexOptions.IgnoreCase);
					gIChannels[i] = new GIChannel(regxV.Match(this.ConfigFile).Value,this);
					
                    //
                }

			}

			isInitialized = true;
		}

		private string buildReg(string Section)
		{
			return $"\\[{Section}\\].*(?:\\r?\\n\\s*[^\\]\\[\\s].*)+";
		}

	}
}