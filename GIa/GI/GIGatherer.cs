using FluentFTP;
using FluentFTP.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GI
{
	/// <summary>
	/// Myftp.cs contains an inner static class of GIGatherer
	/// </summary>
	public partial class GIGatherer
	{
		#region instanzvariablen
		private DateTime _myTime;
		private string _gateIPaddress;
		private FtpClient _myClient;
		#endregion

		#region Sigleton

		private static GIGatherer _g;
		private static readonly object Padlock = new object();

		GIGatherer()
		{

		}


		public static GIGatherer Instance
		{
			get
			{
				lock (Padlock)
				{
					if (_g == null)
					{
						_g = new GIGatherer
                        {
                            _myTime = DateTime.Now
                        };
                    }
					return _g;
				}
			}
		}

		#endregion

		public DateTime CreateTime { get { return _myTime; } }


		public void SetIp(string ipAddress)
		{
			if (string.IsNullOrEmpty(_gateIPaddress))
			{
				_gateIPaddress = ipAddress;
			}

		}

		public string IpAddress { get { return _gateIPaddress; } }

		#region FTP-Related
		private List<GIFile> FtpDirectory(string path)
        {
            if (IpAddress == null)
            {
                throw new System.IO.IOException("Hostname oder IP Adresse unbekannt. Wurde IP-Adresse angegeben?");
            }
            else
            {
                MyClientConnect();
                Thread.Sleep(100);
                List<GIFile> ftp = new List<GIFile>();
                FtpListItem[] items = _myClient.GetListing(path);
                foreach (FtpListItem item in items)
                {
                    GIFile fileinfo = new GIFile(item.Name)
                    {
                        Size = item.Size
                    };
                    ftp.Add(fileinfo);
                }
                return ftp;
            }
        }

		private void MyClientConnect()
		{
			if (_myClient == null) _myClient = Myftp.GetClient(IpAddress);
			Myftp.Connect();
			Thread.Sleep(500);
		}

		private void MyClientDisconnect()
		{
			Myftp.Disconnect();
		}

		private string GetFile(string FileName)
		{
			MyClientConnect();
			byte[] fileContent;
			_myClient.DownloadBytes(out fileContent, FileName);
			return Encoding.UTF8.GetString(fileContent);
		}
		#endregion


		public async Task<List<GIFile>> GetFileInformations(IProgress<int> progress)
        {
            if (IpAddress == null)
            {
                throw new System.IO.IOException("Hostname oder IP Adresse unbekannt. Wurde IP-Adresse angegeben?");
            }
            else
            {
                int progressreport = 0;
                int stepWidth;
                int unstablecounter = 40;
                string file;
                ProgressReport();
                List<GIFile> list = await Task.Run(() => FtpDirectory("/"));
                progressreport += 3;
                ProgressReport();
                var item = list.Find(x => x.Filename == "#actual.sta");

                file = await Task.Run(() => GetFile(item.Filename));
                while (unstablecounter > 0 && !file.ContainsCI("CONFIGURATION STABLE"))
                {
                    unstablecounter--;
                    file = await Task.Run(() => GetFile(item.Filename));
                    Thread.Sleep(500);
                    progressreport += 3;
                    ProgressReport();
                    Thread.Sleep(500);
                    progressreport += 3;
                    ProgressReport();
                }
                if (unstablecounter < 1)
                {
                    throw new Exception("Es konnte keine stabile Konfiguration geladen werden. Module defekt?");
                }

                item.Content = file;

                stepWidth = 72 / (list.Count);

                if (progressreport < 28) progressreport = 28;


                //erst wenn Configuration stabil andere dateien holen!

                foreach (GIFile it in list)
                {
                    progressreport += stepWidth;
                    ProgressReport();
                    if (it.Filename != "#actual.sta")
                    {
                        file = await Task.Run(() => GetFile(it.Filename));
                        it.Content = file;
                    }
                }

                MyClientDisconnect();
                progressreport = 100;
                ProgressReport();
                
                GC.Collect();
                return list;
                void ProgressReport()
                {
                    if (progress != null)
                    {
                        progress.Report(progressreport);
                    }
                }
            }
        }
	}
}