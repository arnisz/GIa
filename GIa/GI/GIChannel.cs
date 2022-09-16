using System;
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

		public string DataType { get; private set; }

		public string DataFormat { get; private set; }

        public string DataDirectionA { get; private set; }

        public string InpSplitDataFieldOffs { get; private set; }

        public string InpCombDataFieldOffs { get; private set; }

        public string OutSplitDataFieldOffs { get; private set; }

        public string OutCombDataFieldOffs { get; private set; }

        public int AccessIndex { get; private set; }


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

			// #Summary.sta
			Regex regxSection = new Regex("(?<=\\[).*(?=\\])");
			string secname = regxSection.Match(_config).Value;
			string summary_section = $"M{this.gIModule.ModuleNumberInConfig}_{secname}";
			var df = GIGate.Instance.GIConfigFile("#summary.sta").Content;

			Regex regexCF = new Regex(fmt.InitFormatSectionbuilder(summary_section), RegexOptions.IgnoreCase);
			var cf = regexCF.Match(df).Value;

			Regex regexDataType = new Regex("(?<=\\WDataType=)(\\w{1,16})(?!\\w)");
			DataType = regexDataType.Match(cf).Value;

            Regex regexDataFormat = new Regex("(?<=\\WFormat=)(%?\\d*\\.?\\d*f?)(?!\\w)");
            DataFormat = regexDataFormat.Match(cf).Value;

            Regex regexDataDirectionA = new Regex("(?<=\\WDataDirection=)(.{1,4})(?!\\w)");
            DataDirectionA = regexDataDirectionA.Match(cf).Value.Trim();

            Regex regexInpSplitDataFieldOffs = new Regex("(?<=\\WInpSplitDataFieldOffs=)((0(x|X)|#)[0-9a-fA-F]{1,4})(?!\\w)");
            InpSplitDataFieldOffs = regexInpSplitDataFieldOffs.Match(cf).Value;

            Regex regexInpCombDataFieldOffs = new Regex("(?<=\\WInpCombDataFieldOffs=)((0(x|X)|#)[0-9a-fA-F]{1,4})(?!\\w)");
            InpCombDataFieldOffs = regexInpCombDataFieldOffs.Match(cf).Value;

            Regex regexAccessindex = new Regex("(?<=\\WAccessIndex=)(-?\\d{1,4})(?!\\w)");
			string ai = regexAccessindex.Match(cf).Value;
            AccessIndex = int.Parse(ai);

            Regex regexOutSplitDataFieldOffs = new Regex("(?<=\\WOutSplitDataFieldOffs=)((0(x|X)|#)[0-9a-fA-F]{1,4})(?!\\w)");
            OutSplitDataFieldOffs = regexOutSplitDataFieldOffs.Match(cf).Value;

            Regex regexOutCombDataFieldOffs = new Regex("(?<=\\WOutCombDataFieldOffs=)((0(x|X)|#)[0-9a-fA-F]{1,4})(?!\\w)");
            OutCombDataFieldOffs = regexInpCombDataFieldOffs.Match(cf).Value;


        }



		public override string ToString()
		{
			return $"C:{variableName}";
		}
	}
}