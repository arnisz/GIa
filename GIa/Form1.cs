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
            textBox1.Text = "192.168.17.99";
            listBox1.Items.Clear();
            // var mod = giGate.FindModules();


            GIGatherer g = GIGatherer.Instance;
            g.SetIP(textBox1.Text);



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
            // Action<int> ReportProgress = new Action<int>(x => MessageBox.Show(x.ToString()));
            var progressIndicator = new Progress<int>(ReportProgress);

            GIGate g = GIGate.Instance;
            GIGatherer gIGatherer = GIGatherer.Instance;
            gIGatherer.SetIP("192.168.17.99");

            await Task.Run(()=> g.Initialize(progressIndicator));
            
            List<GIModule> gIModulesList = g.ListModules();
            string info = $"Es sind {g.modulesCount} Messmodule vorhanden. \r";
            foreach (GIModule module in gIModulesList)
            {
                info += $" Adresse {module.Adress} UART: {module.Uart} hat S/N {module.SerialNumber} \r";
            }
            MessageBox.Show(info);

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private async void button3_Click(object sender, EventArgs e)
        {
            GIGate g = GIGate.Instance;
            var mods = g.ListModules();
            var mod = mods.Find(x => x.Adress == 1);
            await mod.Initialize();

        }
    }
}
