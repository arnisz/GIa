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
		private string _modulType;
		private int _channelCount;
		private GIChannel[] _gIChannels;
		private bool _isInitialized = false;
        private int _hconnection = -1;

        //Module sollte wissen an welcher stelle es in Konfiguration vorkommt
		//#summary.sta weist dem ersten Modul 0 zu. Reihenfolge Aufsteigend
        private int _moduleNumberinConfig;

		private GIGate _gIGate;

		

		public GIModule(int ModuleinConfig)
		{
			this._moduleNumberinConfig = ModuleinConfig;
			_gIGate = GIGate.Instance;
		}

		public int ModuleNumberInConfig { get { return _moduleNumberinConfig; } private set { } }	

		public int Adress { get; set; }
		public string ModulType { get; set; }

		public long SerialNumber { get; set; }

		public List<GIChannel> GetGiChannels {
			get 
			{
				if (!_isInitialized)
				{
					Initialize();
				}
				return _gIChannels.ToList();
			}
			set
			{}
		}

		public int Uart { get; set; }

		public int ChannelsNumber { get { return this._channelCount; } private set { } }
		public string ConfigFile { get; set; }

		// INIT Channels
		// Kanaldaten werden in die Kanalobjekte Übertragen
		public async Task Initialize()
		{
			if (!_isInitialized)
			{
				Regex regxDevice = new Regex(GI.Formats.PatternCategory.InitFormatSectionbuilder("Device"), RegexOptions.IgnoreCase);
				var device = regxDevice.Match(this.ConfigFile).Value;

				Regex regxChannelsCount = new Regex("VCnt=\\d*");
				string scc = regxChannelsCount.Match(device).Value.Substring(5);

				this._channelCount = int.Parse(scc);

				_gIChannels = new GIChannel[_channelCount];
				for (int i = 0; i < _channelCount; i++)
				{
                    Regex regxV = new Regex(GI.Formats.PatternCategory.InitFormatSectionbuilder($"V{i}"), RegexOptions.IgnoreCase);
					_gIChannels[i] = new GIChannel(regxV.Match(this.ConfigFile).Value,this);
					
                    //
                }

			}

			_isInitialized = true;
		}

        public override string ToString()
        {
            return $"{this.Adress}-{this.ModulType}-{this.SerialNumber}";
        }
    }
}