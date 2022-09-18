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
        private int HCONNECTION = -1;

        //Module sollte wissen an welcher stelle es in Konfiguration vorkommt
		//#summary.sta weist dem ersten Modul 0 zu. Reihenfolge Aufsteigend
        private int moduleNumberinConfig;

		private GIGate gIGate;

		

		public GIModule(int ModuleinConfig)
		{
			this.moduleNumberinConfig = ModuleinConfig;
			gIGate = GIGate.Instance;
		}

		public int ModuleNumberInConfig { get { return moduleNumberinConfig; } private set { } }	

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
		// Kanaldaten werden in die Kanalobjekte Übertragen
		public async Task Initialize()
		{
			if (!isInitialized)
			{
				Regex regxDevice = new Regex(GI.Formats.PatternCategory.InitFormatSectionbuilder("Device"), RegexOptions.IgnoreCase);
				var device = regxDevice.Match(this.ConfigFile).Value;

				Regex regxChannelsCount = new Regex("VCnt=\\d*");
				string scc = regxChannelsCount.Match(device).Value.Substring(5);

				this.channelCount = int.Parse(scc);

				gIChannels = new GIChannel[channelCount];
				for (int i = 0; i < channelCount; i++)
				{
                    Regex regxV = new Regex(GI.Formats.PatternCategory.InitFormatSectionbuilder($"V{i}"), RegexOptions.IgnoreCase);
					gIChannels[i] = new GIChannel(regxV.Match(this.ConfigFile).Value,this);
					
                    //
                }

			}

			isInitialized = true;
		}

        public override string ToString()
        {
            return $"{this.Adress}-{this.ModulType}-{this.SerialNumber}";
        }
    }
}