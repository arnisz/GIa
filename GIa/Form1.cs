using GantnerInstruments;
using GI;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace GIa
{
    public partial class Form1 : Form
    {
        List<GIFile> _dir;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GIGate giGate = GIGate.Instance;
            if (textBox1.Text == "") textBox1.Text = "192.168.17.99";
            giGate.IpAddress = textBox1.Text;

            MessageBox.Show(GIGate.Instance.IpAddress);
            if (Netzwerk.Ip.IsValidIpAdress(textBox1.Text))
            {
                button2.Enabled = true;
            }
            else
            {
                button2.Enabled = false;
            }

            listBox1.Items.Clear();
            // var mod = giGate.FindModules();

            /*

            dir = g.FTPDirectory("/");
            
            foreach (var entry in dir)
            {
                listBox1.Items.Add(entry);
            }
            */



        }

        void ReportProgress(int value)
        {
            //Update the UI to reflect the progress value that is passed back.
            textBox2.Text = value.ToString();
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            try
            {
                // Action<int> ReportProgress = new Action<int>(x => MessageBox.Show(x.ToString()));
                var progressIndicator = new Progress<int>(ReportProgress);

                GIGate g = GIGate.Instance;
                GIGatherer gIGatherer = GIGatherer.Instance;
            
                await Task.Run(()=> g.Initialize(progressIndicator));
            
                List<GIModule> gIModulesList = g.ListModules();
                string info = $"Es sind {g.ModulesCount} Messmodule vorhanden. \r";
                foreach (GIModule module in gIModulesList)
                {
                    info += $" Adresse {module.Adress} UART: {module.Uart} hat S/N {module.SerialNumber} \r";
                }
                MessageBox.Show(info);
                button3.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private async void button3_Click(object sender, EventArgs e)
        {
            GIGate g = GIGate.Instance;
            if (g.IsInitialized)
            {
                listBox1.Items.Clear();
                var mods = g.ListModules();
                var mod = mods.Find(x => x.Adress == 1);
                await mod.Initialize();

                foreach (var m in mods)
                {
                    listBox1.Items.Add(m);
                }
            }
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var lbi = (GIModule) listBox1.SelectedItem;
            listBox2.Items.Clear();

            string channels = "";
            foreach (GIChannel c in lbi.GetGiChannels)
            {
                listBox2.Items.Add(c);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            GIGate.Instance.Dispose();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string controllerIp = "192.168.17.99";
            int ret = 0;
            int hconnection = -1;
            int hclient = -1;
            byte[] baTempInfo = new byte[1024];
            string strTemp = "";
            double dTempInfo=0;

            int iTempInfo;
            ret = Hsp._CD_eGateHighSpeedPort_Init(controllerIp, 7, (int)Hsp.Connectiontype.Online, 100, ref hclient,
                ref hconnection);
            Hsp._CD_eGateHighSpeedPort_ReadOnline_Single(hconnection, 1, ref dTempInfo);
            string d = dTempInfo.ToString();
            textBox3.Text += d + "\r\n";
            Hsp._CD_eGateHighSpeedPort_Close(hconnection,hclient);

        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            //string controllerIP = "192.168.17.99";
            int ret = 0;
            int hconnection = -1;
            int hclient = -1;
            byte[] baTempInfo = new byte[1024];
            string strTemp = "";
            double dTempInfo = 0;

            GIChannel c = (GIChannel)listBox2.SelectedItem;
            

            int iTempInfo;
            ret = Hsp._CD_eGateHighSpeedPort_Init(GIGate.Instance.IpAddress, 7, (int)Hsp.Connectiontype.Online, 100, ref hclient,
                ref hconnection);
            Hsp._CD_eGateHighSpeedPort_ReadOnline_Single(hconnection, c.AccessIndex+1, ref dTempInfo);
            string d = dTempInfo.ToString();
            textBox3.Text += d + "\r\n";
            Hsp._CD_eGateHighSpeedPort_Close(hconnection, hclient);
        }
    }
}
