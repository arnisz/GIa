using FluentFTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace GI
{
    public partial class GIGatherer
    {

        protected static class Myftp
        {
            private static FtpClient _ftpClient;


            public static FtpClient GetClient(string Host)
            {
                try
                {
                    _ftpClient = new FtpClient(Host, FtpCredentials.FtpUsername, FtpCredentials.FtpPassword, 21);
                    return _ftpClient;
                }
                catch (Exception ex)
                {
                    if (Host == null) MessageBox.Show("IP Adresse unbekannt");
                    else MessageBox.Show(ex.Message);
                    return null;
                }


            }

            public static void Connect()
            {
                if (_ftpClient != null)
                {
                    if (!_ftpClient.IsConnected)
                    {
                        try
                        {
                            _ftpClient.Connect();
                            var s = _ftpClient.Status;
                        } catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }

                    }
                }

            }

            public static void Disconnect()
            {
                if (_ftpClient.IsConnected)
                {
                    _ftpClient.Disconnect();
                }
            }

            public static List<GIFile> GetFtpDirectory(string path)
            {
                List<GIFile> ftpDirectory = new List<GIFile>();
                Connect();
                FtpListItem[] v = _ftpClient.GetListing("/");
                foreach (var f in v)
                {
                    GIFile file = new GIFile(f.Name);
                    file.Size = f.Size;

                    ftpDirectory.Add(file);
                }
                return ftpDirectory;

            }

        }
    }
}