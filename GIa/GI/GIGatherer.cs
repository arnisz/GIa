using FluentFTP;
using FluentFTP.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

using static System.Net.WebRequestMethods;

namespace GI
{
	/// <summary>
	/// Myftp.cs contains an inner static class of GIGatherer
	/// </summary>
	public partial class GIGatherer
	{
        #region instanzvariablen
        private DateTime myTime;
        private string gateIPaddress;
		private FtpClient myClient;
        #endregion

        #region Sigleton

        private static GIGatherer g = null;
		private static readonly object padlock = new object();

		GIGatherer()
		{

		}
		

		public static GIGatherer Instance
		{
			get
			{
				lock (padlock)
				{
					if (g == null)
					{
						g = new GIGatherer();
						g.myTime = DateTime.Now;
					}
					return g;
				}
			}
		}

        #endregion

		public DateTime CreateTime { get { return myTime; } }


		public void SetIP(string IPAddress)
		{
			if (string.IsNullOrEmpty(gateIPaddress))
			{
				gateIPaddress = IPAddress;
			} 
			
		}

        public string IPAddress { get { return gateIPaddress; } }

        #region FTP-Related
        private async Task< List<GIFile>> FTPDirectory(string path)
		{
			try
			{
				if (IPAddress == null)
				{
					throw new System.IO.IOException("Hostname oder IP Adresse unbekannt. Wurde IP-Adresse angegeben?");
					return null;
				}
				else
				{
					MyClientConnect();
					Thread.Sleep(100);
					List<GIFile> ftp = new List<GIFile>();
					FtpListItem[] items = myClient.GetListing(path);
					foreach (FtpListItem item in items)
					{
						GIFile fileinfo = new GIFile(item.Name);
						fileinfo.Size = item.Size;
						ftp.Add(fileinfo);
					}
					return ftp;
				}
			} catch (Exception ex)
			{
				throw ex;
				return null;
			}

		}

		private async Task<List<GIFile>> GetAllFilesandContent(List<GIFile> gIFiles)
		{
            if (IPAddress ==null) throw new System.IO.IOException("Hostname oder IP Adresse unbekannt. Wurde IP-Adresse angegeben?");

            var m = gIFiles.Find(x => x.Filename == "#actual.sta");
			
            m.Content = await GetFile(m.Filename);
            foreach (GIFile file in gIFiles)
			{
				file.Content =await GetFile(file.Filename);

			}
			return gIFiles;
		}

		private void MyClientConnect()
		{
			if (myClient == null) myClient = Myftp.GetClient(IPAddress);
			Myftp.Connect();
			Thread.Sleep(500);
		}

		private void MyClientDisconnect()
		{
			Myftp.Disconnect();
		}

        private async Task<string> GetFile(string FileName)
        {
            MyClientConnect();
            Byte[] fileContent = new byte[4098];
            myClient.DownloadBytes(out fileContent, FileName);
            return Encoding.UTF8.GetString(fileContent);
        }
        #endregion


		public async Task<List<GIFile>> GetFileInformations(IProgress<int> progress)
		{
			try
			{
				if (IPAddress == null)
				{
					throw new System.IO.IOException("Hostname oder IP Adresse unbekannt. Wurde IP-Adresse angegeben?");
					return null;
				}
				else
				{
					int _progressreport = 0;
					int stepWidth = 3;
					int unstablecounter = 40;  
					string file = "";
					progressReport();
					List<GIFile> list = await FTPDirectory("/");
					_progressreport += 3;
					progressReport();
					var item = list.Find(x => x.Filename == "#actual.sta");

					file = await GetFile(item.Filename);
					while (unstablecounter > 0 && !file.ContainsCI("CONFIGURATION STABLE"))
					{
						unstablecounter--;
						file = await GetFile(item.Filename);
						Thread.Sleep(500);
						_progressreport += 3;
						progressReport();
						Thread.Sleep(500);
						_progressreport += 3;
						progressReport();
					}
					if (unstablecounter < 1)
					{
						throw new Exception("Es konnte keine stabile Konfiguration geladen werden. Module defekt?");
					}

					item.Content = file;

					stepWidth = 72 / (list.Count);

					if (_progressreport < 28) _progressreport = 28;


					//erst wenn Configuration stabil andere dateien holen!

					foreach (GIFile it in list)
					{
						_progressreport += stepWidth;
						progressReport();
						if (it.Filename != "#actual.sta")
						{
							file = await GetFile(it.Filename);
							it.Content = file;
						}
					}

					MyClientDisconnect();
					_progressreport = 100;
					progressReport();
					return list;
					void progressReport()
					{
						if (progress != null)
						{
							progress.Report(_progressreport);
						}
					}
				}
			} catch (Exception ex)
			{
				throw ex;
				return null;
			}
		}
	}
}