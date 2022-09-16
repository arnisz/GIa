using System;
using System.Drawing.Printing;
using System.Text.RegularExpressions;
using fmt=GI.Formats.Formats;

namespace GI
{

	public class GIChannel
	{
		private double cFaktor;
		private double cOffset;
		private string variableName;
		private string _config;
        private GIModule gIModule;
		private int dadi;

		public string VariableName { get { return variableName; } private set { }}

		public double Factor { get { return cFaktor; } private set { } }
        public double Offset { get { return cOffset; } private set { } }
        public bool DataDirectionOut { get 
			{
				return (dadi != 0); 
			} private set { } }


		public GIChannel(string Config, GIModule gIModule)
		{
			_config = Config;
			this.gIModule = gIModule;
			Regex regexName = new Regex("Na=.*",RegexOptions.IgnoreCase);
			variableName = regexName.Match(Config).Value.Substring(3).TrimEnd(new char[] {'\t','\r','\n'});

            Regex regexType = new Regex("(?<=\\WDaDi=)(-?\\d*(\\.|,)?\\d*(E|e)?-?\\d{1,3})", RegexOptions.IgnoreCase);
			dadi = int.Parse(regexType.Match(Config).Value);

			Regex regxFactor = new Regex("(?<=\\WUnTrFa=)(-?\\d*(\\.|,)?\\d*(E|e)?-?\\d{1,3})", RegexOptions.IgnoreCase);
			string s = regxFactor.Match(Config).Value;
            cFaktor = fmt.GetDouble(s);

            Regex regxOffset = new Regex("(?<=\\WUnTrOf=)(-?\\d*(\\.|,)?\\d*(E|e)?-?\\d{1,3})", RegexOptions.IgnoreCase);
            cOffset = fmt.GetDouble(regxOffset.Match(Config).Value);
			
        }
		


		public override string ToString()
		{
			return $"C:{variableName}";
		}
	}
}