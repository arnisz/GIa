using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using GI.Formats;
using fmt=GI.Formats.PatternCategory;
using GantnerInstruments;

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

        private int myChannelNumber = 0;
        private string myIP = "";

        public int MyChannelNumber { get { return this.myChannelNumber;} private set {} }       


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



        public double Meas 
        {
            get
            {
                double value=0;

                //IniteGate();
                GIGate.Instance.IniteGate();
                Thread.Sleep(100);
                HSP._CD_eGateHighSpeedPort_ReadOnline_Single(GIGate.Instance.HCONNECTION, this.MyChannelNumber, ref value);
                double rv = value;
                return rv;
                
            } 
            private set{}
        }




        public bool DataDirectionOut { get 
			{
				return (dadi != 0); 
			} private set { } }


		public GIChannel(string Config, GIModule gIModule)
		{
			_config = Config;
            this.gIModule = gIModule;

            Regex regexName = new Regex($"{fmt.Starter("Na=")}{fmt.Any}{fmt.EndLine}", RegexOptions.IgnoreCase);
			variableName = regexName.Match(Config).Value.Trim();

            Regex regexType = new Regex($"{fmt.Starter("DaDi=")}{fmt.WholeNumber}{fmt.EndLine}", RegexOptions.IgnoreCase);
            string vv = regexType.Match(Config).Value.Trim();
            dadi = int.Parse(regexType.Match(Config).Value.Trim());

            Regex regxFactor = new Regex($"{fmt.Starter("UnTrFa=")}{fmt.Double}{fmt.EndLine}", RegexOptions.IgnoreCase);
			string s = regxFactor.Match(Config).Value.Trim();
            cFaktor = GI.Formats.Formats.GetDouble(s);

            Regex regxOffset = new Regex($"{fmt.Starter("UnTrOf=")}{fmt.Double}{fmt.EndLine}", RegexOptions.IgnoreCase);
			string t = regxOffset.Match(Config).Value.Trim();
            cOffset = GI.Formats.Formats.GetDouble(regxOffset.Match(Config).Value.Trim());

			// #Summary.sta
			Regex regxSection = new Regex("(?<=\\[).*(?=\\])");
			string secname = regxSection.Match(_config).Value;
			string summarySection = $"M{this.gIModule.ModuleNumberInConfig}_{secname}";
			var summarySta = GIGate.Instance.GIConfigFile("#summary.sta").Content;
            Regex regxChannelPositions = new Regex("(?<=\\W\\[M)\\d{1,2}_V\\d{1,2}");
            var hits = regxChannelPositions.Matches(summarySta);
            Console.WriteLine(hits.Count);
            int countMatch = 0;
            foreach (Match match in hits)
            {
                countMatch ++;
                if (summarySection == "M"+match.Value)
                {break;}
                
            }

            myChannelNumber = countMatch;
            Console.WriteLine(countMatch);

			Regex regexCF = new Regex(fmt.InitFormatSectionbuilder(summarySection), RegexOptions.IgnoreCase);
			var cf = regexCF.Match(summarySta).Value;

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

            Regex regexOutCombDataFieldOffs = new Regex($"{fmt.Starter("OutCombDataFieldOffs=")}{fmt.Hexadecimal}{fmt.EndLine}");
            OutCombDataFieldOffs = regexInpCombDataFieldOffs.Match(cf).Value;
        }

        public override string ToString()
		{
			return $"N:{this.myChannelNumber.ToString()} - C:{variableName}";
		}
	}
}