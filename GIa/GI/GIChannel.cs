using System.Text.RegularExpressions;
using fmt = GI.Formats.PatternCategory;


namespace GI
{

    public class GIChannel
    {
        private double _cFaktor;
        private double _cOffset;
        private string _variableName;
        private string _config;
        private GIModule _gIModule;
        private int _dadi;

        public string VariableName { get { return _variableName; } private set { _variableName = value; } }

        public double Factor { get { return _cFaktor; } private set { _cFaktor = value; } }
        public double Offset { get { return _cOffset; } private set { _cFaktor = value; } }

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

                return 0;
            }
            private set { _cFaktor = value; }
        }




        public bool DataDirectionOut => (_dadi != 0);


        public GIChannel(string Config, GIModule gIModule)
        {
            _config = Config;

            _gIModule = gIModule;
            Regex regexName = new Regex($"{fmt.Starter("Na=")}{fmt.Any}{fmt.EndLine}", RegexOptions.IgnoreCase);
            _variableName = regexName.Match(Config).Value.Trim();

            Regex regexType = new Regex($"{fmt.Starter("DaDi=")}{fmt.WholeNumber}{fmt.EndLine}", RegexOptions.IgnoreCase);
            _dadi = int.Parse(regexType.Match(Config).Value.Trim());

            Regex regxFactor = new Regex($"{fmt.Starter("UnTrFa=")}{fmt.Double}{fmt.EndLine}", RegexOptions.IgnoreCase);
            string s = regxFactor.Match(Config).Value.Trim();
            _cFaktor = Formats.Formats.GetDouble(s);

            Regex regxOffset = new Regex($"{fmt.Starter("UnTrOf=")}{fmt.Double}{fmt.EndLine}", RegexOptions.IgnoreCase);
            _cOffset = Formats.Formats.GetDouble(regxOffset.Match(Config).Value.Trim());

            // #Summary.sta
            Regex regxSection = new Regex("(?<=\\[).*(?=\\])");
            string secname = regxSection.Match(_config).Value;
            string summarySection = $"M{_gIModule.ModuleNumberInConfig}_{secname}";
            var summarySta = GIGate.Instance.GiConfigFile("#summary.sta").Content;
            Regex regxChannelPositions = new Regex("(?<=\\W\\[M)\\d{1,2}_V\\d{1,2}");
            regxChannelPositions.Matches(summarySta);

            Regex regexCf = new Regex(fmt.InitFormatSectionbuilder(summarySection), RegexOptions.IgnoreCase);
            var cf = regexCf.Match(summarySta).Value;

            Regex regexDataType = new Regex("(?<=\\WDataType=)(\\w{1,16})(?!\\w)");
            DataType = regexDataType.Match(cf).Value;

            Regex regexDataFormat = new Regex("(?<=\\WFormat=)(%?\\d*\\.?\\d*f?)(?!\\w)");
            DataFormat = regexDataFormat.Match(cf).Value;

            Regex regexDataDirectionA = new Regex("(?<=\\WDataDirection=)(.{1,4})(?!\\w)");
            DataDirectionA = regexDataDirectionA.Match(cf).Value.Trim();

            Regex regexInpSplitDataFieldOffs = new Regex($"{fmt.Starter("InpSplitDataFieldOffs=")}{fmt.Hexadecimal}{fmt.EndLine}");
            InpSplitDataFieldOffs = regexInpSplitDataFieldOffs.Match(cf).Value;

            Regex regexInpCombDataFieldOffs = new Regex($"{fmt.Starter("InpCombDataFieldOffs=")}{fmt.Hexadecimal}{fmt.EndLine}");
            InpCombDataFieldOffs = regexInpCombDataFieldOffs.Match(cf).Value;

            Regex regexAccessindex = new Regex($"{fmt.Starter("AccessIndex=")}{fmt.WholeNumber}{fmt.EndLine}");
            string ai = regexAccessindex.Match(cf).Value;
            AccessIndex = int.Parse(ai);

            Regex regexOutSplitDataFieldOffs = new Regex($"{fmt.Starter("OutSplitDataFieldOffs=")}{fmt.Hexadecimal}{fmt.EndLine}");
            OutSplitDataFieldOffs = regexOutSplitDataFieldOffs.Match(cf).Value;

            Regex regOutCombDataFieldOffs = new Regex($"{fmt.Starter("OutCombDataFieldOffs=")}{fmt.Hexadecimal}{fmt.EndLine}");
            OutCombDataFieldOffs = regOutCombDataFieldOffs.Match(cf).Value;
        }

        public override string ToString()
        {
            return $"C:{_variableName} Zugriffsnummer:{AccessIndex}";
        }
    }
}