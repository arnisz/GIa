using FluentFTP;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                        try
                        {
                            ftpClient.Connect();
                            var s = ftpClient.Status;
                        } catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }

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
            
        }
    }
}