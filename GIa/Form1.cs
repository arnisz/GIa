using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
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
                string info = $"Es sind {g.modulesCount} Messmodule vorhanden. \r";
                foreach (GIModule module in gIModulesList)
                {
                    info += $" Adresse {module.Adress} UART: {module.Uart} hat S/N {module.SerialNumber} \r";
                }
                MessageBox.Show(info);
                button3.Enabled = true;
            } catch (Exception ex)
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

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var lbi = (GIModule) listBox1.SelectedItem;

            string channels = "";
            foreach (GIChannel c in lbi.GetGIChannels)
            {
                channels += $"{c.MyChannelNumber} Channel {c.VariableName} f:{c.Factor} o:{c.Offset} AI:{c.AccessIndex} Form:{c.DataFormat} {c.Meas}\r";
            }
            Console.WriteLine(channels);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            GIGate.Instance.Dispose();
        }
    }
}
