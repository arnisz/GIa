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
            private static FtpClient ftpClient;


            public static FtpClient GetClient(string Host)
            {
                try
                {
                    ftpClient = new FtpClient(Host, FtpCredentials.FtpUsername, FtpCredentials.FtpPassword, 21);
                    return ftpClient;
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
                if (ftpClient != null)
                {
                    if (!ftpClient.IsConnected)
                    {
                        ftpClient.Connect();
                        var s = ftpClient.Status;

                    }
                }

            }

            public static void Disconnect()
            {
                if (ftpClient.IsConnected)
                {
                    ftpClient.Disconnect();
                }
            }

            public static List<GIFile> GetFTPDirectory(string path)
            {
                List<GIFile> ftpDirectory = new List<GIFile>();
                Connect();
                FtpListItem[] v = ftpClient.GetListing("/");
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