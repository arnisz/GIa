using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using GantnerInstruments;
using GI;

namespace GIa
{
    public partial class Form1 : Form
    {
        List<GIFile> dir;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GIGate giGate = GIGate.Instance;
            if (textBox1.Text=="") textBox1.Text = "192.168.17.99";
            giGate.IPAddress=textBox1.Text;

            MessageBox.Show(GIGate.Instance.IPAddress);
            if (Netzwerk.ip.IsValidIPAdress(textBox1.Text))
            {
                button2.Enabled = true;
            } else
            {
                button2.Enabled=false;
            }

            listBox1.Items.Clear();
          


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
            
                // List<GIModule> gIModulesList = g.ListModules();
                // string info = $"Es sind {g.modulesCount} Messmodule vorhanden. \r";
                // foreach (GIModule module in gIModulesList)
                // {
                //     info += $" Adresse {module.Adress} UART: {module.Uart} hat S/N {module.SerialNumber} \r";
                // }
                // MessageBox.Show(info);
                button3.Enabled = true;

            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }

        private async void button3_Click(object sender, EventArgs e)
        {
            GIGate g = GIGate.Instance;
            if (g.isInitialized)
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


        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var lbi = (GIModule) listBox1.SelectedItem;
            listBox2.Items.Clear();

            string channels = "";
            foreach (GIChannel c in lbi.GetGIChannels)
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
            string controllerIP = "192.168.17.99";
            int ret = 0;
            int HCONNECTION = -1;
            int HCLIENT = -1;
            string strTemp = "";
            double dTempInfo=0;

            int iTempInfo;
            ret = HSP._CD_eGateHighSpeedPort_Init(controllerIP, 7, (int)HSP.CONNECTIONTYPE.Online, 100, ref HCLIENT,
                ref HCONNECTION);
            HSP._CD_eGateHighSpeedPort_ReadOnline_Single(HCONNECTION, 1, ref dTempInfo);
            string d = dTempInfo.ToString();
            textBox3.Text += d + "\r\n";
            HSP._CD_eGateHighSpeedPort_Close(HCONNECTION,HCLIENT);

        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            //string controllerIP = "192.168.17.99";
            int ret = 0;

            GIChannel c = (GIChannel)listBox2.SelectedItem;
            var values = c.Meas(10).GetAwaiter().GetResult();

            string strTemp = "";
            double mittelwert = values.Average();

            strTemp = mittelwert.ToString();

            textBox3.Text = "Wert: " + strTemp + "\t";
            strTemp = StdDiv(values).ToString();

            textBox3.Text += "StdDiv: " + strTemp + "\t";

            strTemp = StdDivMean(values).ToString();
            textBox3.Text += "StdDivMean: " + strTemp + "\r\n";

            foreach (double measdDouble in values)
            {
                textBox3.Text += measdDouble + "\r\n";
            }
        }

        public static double StdDiv(double[] values)
        {
            double sum = 0;
            double mittelw = values.Average();
            foreach (double value in values)
            {
                sum += (value - mittelw) * (value - mittelw);
            }

            sum = sum / (values.Length-1);
            sum = Math.Sqrt(sum);
            return sum;
        }

        public static double StdDivMean(double[] values)
        {
            double stddiv = StdDiv(values);
            return stddiv / Math.Sqrt(values.Length);
        }
    }
}
