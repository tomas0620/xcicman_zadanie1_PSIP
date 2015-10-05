using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using PcapDotNet.Core;
using System.Threading;

namespace xcicman_zadanie1_PSIP
{
    
    public partial class Form1 : Form
    {
        private Receive r = null;
        Help_Form help;
        public Form1()
        {
            InitializeComponent();
            Find_Adapter find_adapter = new Find_Adapter(comboBox1, comboBox2);

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            PacketDevice device1 = ((KeyValuePair<string, LivePacketDevice>)comboBox1.SelectedItem).Value;
            PacketDevice device2 = ((KeyValuePair<string, LivePacketDevice>)comboBox2.SelectedItem).Value;

            if ((device1 != null) && (device2 != null))
            {
                r = new Receive(richTextBox1, device1, device2);
                Thread adapter1_thread = new Thread(new ThreadStart(r.a1_receive));
                adapter1_thread.IsBackground = true;
                adapter1_thread.Start();
            }
            else
            {
                MessageBox.Show("Nezvolili ste adaptér pre oba porty");
            }
           /* Thread adapter2_thread = new Thread(new ThreadStart(r.a2_receive));
            adapter2_thread.IsBackground = true;
            adapter2_thread.Start();*/
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Exit_Form exit = new Exit_Form();
            if (exit.ShowDialog() == DialogResult.Yes)
            {
                Close();
            }
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (help == null)
            {
                help = new Help_Form();
                help.Disposed += new EventHandler(help_set_null);
                help.Show();
                help.MaximizeBox = false;
                help.MinimizeBox = false;

            }
            else
            {
                help.Activate();
            }
        }
        private void help_set_null(object sender, EventArgs e)
        {
            help = null;
        }

        private void infoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Autor: Tomas Cicman" + Environment.NewLine + "AIS cislo: 60327" + Environment.NewLine + "Odbor: PKSS 4" + Environment.NewLine + "Rocnik: 2015/2014" + Environment.NewLine + "Predmet: PSIP");
        }
    }
}
