using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GantnerInstruments;

namespace GI
{
    public struct FtpCredentials
    {
        public const string FtpUsername = "instrumentation4";
        public const string FtpPassword = "gantner";
    }
    public class GiGate : IDisposable
    {
        public int ModulesCount;
        private bool _initialized = false;
        private String _ipAddress;
        private String _serialNumber;
        private List<GiFile> _liGiFiles;

        private GiModule[] _gIModules;
        private GiGatherer _gIGatherer;

        #region Sigleton

        private static GiGate _g = null;
        private static readonly object Padlock = new object();

        GiGate()
        {
            _ipAddress = GiGatherer.Instance.IpAddress;
        }
        private DateTime _myTime;

        public static GiGate Instance
        {
            get
            {
                lock (Padlock)
                {
                    if (_g == null)
                    {
                        _g = new GiGate();
                        _g._myTime = DateTime.Now;
                    }
                    return _g;
                }
            }
        }

        #endregion

        public GiChannel GetChannelByNumber(int Channelnumber)
        {
            foreach (var giModule in _gIModules)
            {
                foreach (var variable in giModule.GetGiChannels)
                {
                    if (variable.AccessIndex==Channelnumber)
                    {
                        return variable;
                    }
                }
            }

            return null;
        }

        public String IpAddress
        {
            get
            {
                return _ipAddress;
            }
            set
            {
                _ipAddress = value;
            }
        }

        public GiFile GiConfigFile(string Filename)
        {
            return _liGiFiles.Find(x => x.Filename == Filename);
        }

        public ref List<GiFile> GiConfigfilesList()
        {
            return ref _liGiFiles;
        }

        public List<GiModule> ListModules()
        {
            List<GiModule> modules = new List<GiModule>();
            try
            {
                foreach (GiModule module in _gIModules)
                    if (module == null) { }
                    else
                    {
                        modules.Add(module);
                    }
                return modules;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public bool IsInitialized { get { return _initialized; } private set { } }

        //initialize Modules
        //Moduldaten werden in die Modulobjekte übertragen
        //Daten aus #actual.sta
        public async Task Initialize([Optional] IProgress<int> Progress)
        {
            try
            {
                if (!_initialized)
                {
                    if (_ipAddress == null)
                    {
                        _ipAddress = GiGatherer.Instance.IpAddress;
                    }
                    GiGatherer.Instance.SetIp(_ipAddress);
                    _liGiFiles = await GiGatherer.Instance.GetFileInformations(Progress);
                    var actual = _liGiFiles.Find(x => x.Filename == "#actual.sta");
                    var summary = _liGiFiles.Find(x => x.Filename == "#summary.sta");

                    //Module Informationen
                    Regex regexLine = new Regex("TS:\\d.*STA-");
                    Regex regexAdress = new Regex("AD:\\d*");
                    Regex regexType = new Regex("TY:..\\d*");
                    Regex regexSerial = new Regex("SNR-\\d*");
                    Regex regexUart = new Regex("DEV:.UART\\d*");

                    var modulelines = regexLine.Matches(actual.Content);
                    ModulesCount = modulelines.Count;
                    _gIModules = new GiModule[ModulesCount];

                    int mcount = 0;
                    foreach (Match i in modulelines)
                    {
                        string item = i.Value;
                        _gIModules[mcount] = new GiModule(mcount);

                        int adr = int.Parse(regexAdress.Match(item).Value.Substring(3));
                        _gIModules[mcount].Adress = adr;

                        string m = regexType.Match(item).Value;
                        _gIModules[mcount].ModulType = m.Substring(3).TrimEnd(new char[] { '\t', '\r', '\n' });

                        string sr = regexSerial.Match(item).Value.Substring(4);
                        long n = long.Parse(sr);
                        _gIModules[mcount].SerialNumber = n;

                        string su = regexUart.Match(item).Value.Substring(9);
                        int v = int.Parse(su);
                        _gIModules[mcount].Uart = v;

                        _gIModules[mcount].ConfigFile = _liGiFiles.Find(x => x.Filename == $"@{v}_{adr}_c.gcf").Content;

                        mcount++;
                    }

                    _initialized = true;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }
    }
}