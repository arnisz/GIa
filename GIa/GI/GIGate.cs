using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GI.Formats;

namespace GI
{
    public struct FtpCredentials
    {
        public const string FtpUsername = "instrumentation4";
        public const string FtpPassword = "gantner";
    }
    public class GIGate:IDisposable
    {
        public int modulesCount;
        private bool _initialized = false;
        private String ipAddress;
        private String serialNumber;
        private List<GIFile> liGIFiles;

        private GIModule[] gIModules;
        private GIGatherer gIGatherer;

        #region Sigleton

        private static GIGate g = null;
        private static readonly object padlock = new object();

        GIGate()
        {
            ipAddress = GIGatherer.Instance.IPAddress;
        }
        private DateTime myTime;

        public static GIGate Instance
        {
            get
            {
                lock (padlock)
                {
                    if (g == null)
                    {
                        g = new GIGate();
                        g.myTime = DateTime.Now;
                    }
                    return g;
                }
            }
        }

        #endregion

        public String IPAddress
        {
            get
            {
                return ipAddress;
            }
            set
            {
                ipAddress = value;
            }
        }

        public GIFile GIConfigFile(string Filename)
        {
            return liGIFiles.Find(x => x.Filename == Filename);
        }

        public ref List<GIFile> GIConfigfilesList()
        {
            return ref liGIFiles;
        }

        public List<GIModule> ListModules()
        {
            List<GIModule> modules = new List<GIModule>();
            try
            {
                foreach (GIModule module in gIModules)
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

        public bool isInitialized { get { return _initialized; } private set { } }

        //initialize Modules
        //Moduldaten werden in die Modulobjekte übertragen
        //Daten aus #actual.sta
        public async Task Initialize([Optional] IProgress<int> Progress)
        {
            try
            {
                if (!_initialized)
                {
                    if (ipAddress == null)
                    {
                        ipAddress = GIGatherer.Instance.IPAddress;
                    }
                    GIGatherer.Instance.SetIP(ipAddress);
                    liGIFiles = await GIGatherer.Instance.GetFileInformations(Progress);
                    var actual = liGIFiles.Find(x => x.Filename == "#actual.sta");
                    var summary = liGIFiles.Find(x => x.Filename == "#summary.sta");

                    //Module Informationen
                    Regex regexLine = new Regex("TS:\\d.*STA-");
                    Regex regexAdress = new Regex("AD:\\d*");
                    Regex regexType = new Regex("TY:..\\d*");
                    Regex regexSerial = new Regex("SNR-\\d*");
                    Regex regexUart = new Regex("DEV:.UART\\d*");

                    var modulelines = regexLine.Matches(actual.Content);
                    modulesCount = modulelines.Count;
                    gIModules = new GIModule[modulesCount];

                    int _mcount = 0;
                    foreach (Match i in modulelines)
                    {
                        string item = i.Value;
                        gIModules[_mcount] = new GIModule(_mcount);

                        int adr = int.Parse(regexAdress.Match(item).Value.Substring(3));
                        gIModules[_mcount].Adress = adr;

                        string m = regexType.Match(item).Value;
                        gIModules[_mcount].ModulType = m.Substring(3).TrimEnd(new char[] { '\t', '\r', '\n' });

                        string sr = regexSerial.Match(item).Value.Substring(4);
                        long n = long.Parse(sr);
                        gIModules[_mcount].SerialNumber = n;

                        string su = regexUart.Match(item).Value.Substring(9);
                        int v = int.Parse(su);
                        gIModules[_mcount].Uart = v;

                        gIModules[_mcount].ConfigFile = liGIFiles.Find(x => x.Filename == $"@{v}_{adr}_c.gcf").Content;

                        _mcount++;
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
            throw new NotImplementedException();
        }
    }
}