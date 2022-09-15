using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GI
{
	public class GIModule
	{
		private string modulType;
		private long channelCount;
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

		public int Uart { get; set; }

		public string ConfigFile { get; set; }

		// INIT Channels
		// Kanaldaten werden in die Kanalobjekte übertragen
		public async Task Initialize()
		{
			Regex regxDevice = new Regex(buildReg("Device"), RegexOptions.IgnoreCase);
			var device = regxDevice.Match(this.ConfigFile).Value;
			Regex regxChannelsCount = new Regex("VCnt=\\d*");
			string scc = regxChannelsCount.Match(device).Value.Substring(5);

            this.channelCount = int.Parse(scc);

            Regex regxV0 = new Regex(buildReg("V0"), RegexOptions.IgnoreCase);
            var V0 = regxV0.Match(this.ConfigFile).Value;

        }

		private string buildReg(string Section)
		{
			return $"\\[{Section}\\].*(?:\\r?\\n\\s*[^\\]\\[\\s].*)+";
		}

	}
}