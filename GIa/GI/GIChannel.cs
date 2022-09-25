using System;
using System.Data.Common;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using GantnerInstruments;
using GI.Formats;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using fmt=GI.Formats.PatternCategory;


namespace GI
{

	public class GIChannel
	{
        public delegate void MeasResult(double[] values);

		private double cFaktor;
		private double cOffset;
		private string variableName;
		private string _config;
        private GIModule gIModule;
		private int dadi;

        private int[] _HC = { -1, -1 };
        private double _dtempInfo;

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

        private void OpenChannel()
        {
            unsafe
            {
                double dTempInfo = 0;
                fixed (int* pointerto = &_HC[0])
                {

                    ref int HCONNECTION = ref _HC[0];
                    ref int HCLIENT = ref _HC[1];
                    int ret;

                    ret = HSP._CD_eGateHighSpeedPort_Init(GIGate.Instance.IPAddress, 22, (int)HSP.CONNECTIONTYPE.Online,
                        100, ref HCLIENT,
                        ref HCONNECTION);

                }
            }
        }

        private void CloseChannel()
        {
            if (_HC[0] == 0 && _HC[1] == 0)
            {
                HSP._CD_eGateHighSpeedPort_Close(_HC[0], _HC[1]);
            }
        }

        public Task<Double[]> Meas(int times)
        {
            Func<double[]> fetchAction = delegate ()
            {
                double[] ret = new double[times];
                OpenChannel();
                _dtempInfo = 0;
                double dSum = 0;

                if (_HC[0] == 0 && _HC[1] == 0)
                {
                    for (int i = 0; i < times; i++)
                    {
                        HSP._CD_eGateHighSpeedPort_ReadOnline_Single(this._HC[0], this.AccessIndex + 1, ref _dtempInfo);
                        ret[i] = _dtempInfo;
                        System.Threading.Thread.Sleep(100);
                    }
                }

                CloseChannel();
                return ret;
            };

            return Task.Run(fetchAction);

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

			Regex regexCF = new Regex(fmt.InitFormatSectionbuilder(summarySection), RegexOptions.IgnoreCase);
			var cf = regexCF.Match(summarySta).Value;

			Regex regexDataType = new Regex("(?<=\\WDataType=)(\\w{1,16})(?!\\w)");
			DataType = regexDataType.Match(cf).Value;

            //{fmt.Starter("Format=")}{fmt.Format}{fmt.EndLine}
            Regex regexDataFormat = new Regex($"{fmt.Starter("Format=")}{fmt.Format}{fmt.EndLine}");
            DataFormat = regexDataFormat.Match(cf).Value;

            //{fmt.Starter("DataDirection=")}{fmt.Any}{fmt.EndLine}
            Regex regexDataDirectionA = new Regex($"{fmt.Starter("DataDirection=")}{fmt.Any}{fmt.EndLine}");
            DataDirectionA = regexDataDirectionA.Match(cf).Value.Trim();

            // {fmt.Starter("InpSplitDataFieldOffs=")}{fmt.Hexadecimal}{fmt.EndLine}
            Regex regexInpSplitDataFieldOffs = new Regex($"{fmt.Starter("InpSplitDataFieldOffs=")}{fmt.Hexadecimal}{fmt.EndLine}");
            InpSplitDataFieldOffs = regexInpSplitDataFieldOffs.Match(cf).Value;

            // {fmt.Starter("InpCombDataFieldOffs=")}{fmt.Hexadecimal}{fmt.EndLine}
            Regex regexInpCombDataFieldOffs = new Regex($"{fmt.Starter("InpCombDataFieldOffs=")}{fmt.Hexadecimal}{fmt.EndLine}");
            InpCombDataFieldOffs = regexInpCombDataFieldOffs.Match(cf).Value;

            //{fmt.Starter("AccessIndex=")}{fmt}{fmt.EndLine}
            Regex regexAccessindex = new Regex($"{fmt.Starter("AccessIndex=")}{fmt.WholeNumber}{fmt.EndLine}");
			string ai = regexAccessindex.Match(cf).Value;
            AccessIndex = int.Parse(ai);

            // {fmt.Starter("OutSplitDataFieldOffs=")}{fmt.Hexadecimal}{fmt.EndLine}
            Regex regexOutSplitDataFieldOffs = new Regex($"{fmt.Starter("OutSplitDataFieldOffs=")}{fmt.Hexadecimal}{fmt.EndLine}");
            OutSplitDataFieldOffs = regexOutSplitDataFieldOffs.Match(cf).Value;

            Regex regexOutCombDataFieldOffs = new Regex($"{fmt.Starter("OutCombDataFieldOffs=")}{fmt.Hexadecimal}{fmt.EndLine}");
            OutCombDataFieldOffs = regexInpCombDataFieldOffs.Match(cf).Value;
        }

        public override string ToString()
		{
			return $"C:{variableName} Zugriffsnummer:{AccessIndex}";
		}
    }
}