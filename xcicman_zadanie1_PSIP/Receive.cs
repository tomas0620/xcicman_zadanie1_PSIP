using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PcapDotNet.Core;
using PcapDotNet.Packets;
using System.Windows.Forms;
using System.Threading;
using System.Drawing;

namespace xcicman_zadanie1_PSIP
{
    public class Receive
    {
        private static RichTextBox richTextBox1 = null, richTextBox2 = null;
        private static PacketDevice device1, device2;
        private static int d1_index = 0, d2_index = 0;
        private static string packet_string;
        private static PacketCommunicator communicator1, communicator2;
        private static MAC_table mac_table;
        private static Form1 form1;
        private static DataGridView dataGridView1;
        private static DataGridView dataGridView2;
        private static Get_Info get_Info = new Get_Info();
        private static Filters filters_class;
        private static Statistics_table statistics_table = new Statistics_table();
        private static Dictionary<string, int> statistics_d1_I = new Dictionary<string, int>();
        private static Dictionary<string, int> statistics_d1_O = new Dictionary<string, int>();
        private static Dictionary<string, int> statistics_d2_I = new Dictionary<string, int>();
        private static Dictionary<string, int> statistics_d2_O = new Dictionary<string, int>();

        public Receive(RichTextBox richTextBox1_receive, RichTextBox richTextBox2_receive, PacketDevice device1_receive, int d1_index_receive, PacketDevice device2_receive, int d2_index_receive, DataGridView dataGridView1_receive, DataGridView dataGridView2_receive, Filters filters_class_receive)
        {
            richTextBox1 = richTextBox1_receive;
            richTextBox2 = richTextBox2_receive;
            device1 = device1_receive;
            device2 = device2_receive;
            d1_index = d1_index_receive;
            d2_index = d2_index_receive;
            communicator1 = device1.Open(65536, PacketDeviceOpenAttributes.NoCaptureLocal | PacketDeviceOpenAttributes.Promiscuous, 1000);
            communicator2 = device2.Open(65536, PacketDeviceOpenAttributes.NoCaptureLocal | PacketDeviceOpenAttributes.Promiscuous, 1000);
            mac_table = new MAC_table();
            dataGridView1 = dataGridView1_receive;
            dataGridView2 = dataGridView2_receive;
            filters_class = filters_class_receive;
            get_Info.prefill_statistics(statistics_d1_I);
            get_Info.prefill_statistics(statistics_d1_O);
            get_Info.prefill_statistics(statistics_d2_I);
            get_Info.prefill_statistics(statistics_d2_O);
        }

        public void a1_receive()
        {
           // try{
            using (communicator1)
                {
                    String text = "Listening on " + device1.Description + "... \r\n";
                    richTextBox1.BeginInvoke(new Action(() => richTextBox1.AppendText(text)));
                    // start the capture
                    communicator1.ReceivePackets(0, PacketHandler1);
                    communicator1.Break();                
            }
            /*}
            catch (Exception ex)
            {
                throw ex;
            }*/
        }

        public void a2_receive()
        {
           // try
            //{
                using (communicator2)
                {
                    String text = "Listening on " + device2.Description + "... \r\n";
                    richTextBox2.BeginInvoke(new Action(() => richTextBox2.AppendText(text)));
                    // start the capture
                    communicator2.ReceivePackets(0, PacketHandler2);
                    communicator2.Break();
                    
                }
            /*}
            catch (Exception ex)
            {
                throw ex;
            }*/
        }

        public static void PacketHandler1(Packet packet)
        {
            byte[] packet_bytes = new byte[packet.Length];
            packet_bytes = packet.Buffer;
            packet_string = BitConverter.ToString(packet_bytes);
            string[] asdasd = packet_string.Split('-');
            packet_string = string.Join("", asdasd);
            String text = "\r\n" + packet.Timestamp.ToString("yyyy-MM-dd hh:mm:ss.fff") + " length:" + packet.Length + "\r\n" + "\r\n" + packet_string + "\r\n";


            get_Info.find_data(packet_string, d1_index, mac_table, dataGridView1);
         
            if (!get_Info.Filtration.filtration(get_Info, filters_class, d1_index, "in", get_Info.Find_value))
            {
                string smac2 = get_Info.SourceMAC2;
                string dmac2 = get_Info.DestinationMAC2;
                string smac = get_Info.SourceMAC;
                bool print = true;
                foreach (var item in get_Info.SwitchMAC) // kontrola ci ramec nepatri switch-u
                {
                    if (smac2 == item || dmac2 == item)
                    {
                        print = false;
                        break;
                    }
                }
                if (print)
                {
                    richTextBox1.BeginInvoke(new Action(() => richTextBox1.SelectionColor = Color.Red));
                    richTextBox1.BeginInvoke(new Action(() => richTextBox1.AppendText("Zablokované z: " + smac + "\n")));
                }
                    return;
            }
            get_Info.fill_mac_table_dict();
            get_Info.get_statistics(packet_string, statistics_d1_I);
            dataGridView2.BeginInvoke(new Action(() => dataGridView2.DataSource = statistics_table.fill_statistics_table(statistics_d1_I, statistics_d1_O, statistics_d2_I, statistics_d2_O)));

            int PnumberSend;
            if (get_Info.Mac_table.ContainsKey(get_Info.DestinationMAC)){
                if(get_Info.Mac_table[get_Info.DestinationMAC].Item2 == d1_index)
                {
                    return;
                }
                PnumberSend = get_Info.Mac_table[get_Info.DestinationMAC].Item2; //cislo portu na ktorz sa ma paket odoslat
            }
            else
            {
                PnumberSend = d2_index;
            }

            
            if (!get_Info.Filtration.filtration(get_Info, filters_class, PnumberSend, "out", get_Info.Find_value)) // kontrola filtrov
            {
                bool print = true;
                foreach (var item in get_Info.SwitchMAC) // kontrola ci ramec nepatri switch-u
                {
                    if (get_Info.SourceMAC2 == item || get_Info.DestinationMAC2 == item)
                    {
                        print = false;
                        break;
                    }
                }
                if (print)
                {
                    richTextBox1.BeginInvoke(new Action(() => richTextBox1.SelectionColor = Color.Red));
                    richTextBox1.BeginInvoke(new Action(() => richTextBox1.AppendText("Zablokované na: " + get_Info.SourceMAC + "\n")));
                }
                return;
            }
            Sending s = new Sending(packet, device2);
            get_Info.get_statistics(packet_string, statistics_d2_O);
            s.send(communicator2);

            richTextBox1.BeginInvoke(new Action(() => richTextBox1.AppendText(text)));
            dataGridView2.BeginInvoke(new Action(() => dataGridView2.DataSource = statistics_table.fill_statistics_table(statistics_d1_I, statistics_d1_O, statistics_d2_I, statistics_d2_O)));
        }
        public static void PacketHandler2(Packet packet)
        {   
            
            byte[] packet_bytes = new byte[packet.Length];
            packet_bytes = packet.Buffer;
            packet_string = BitConverter.ToString(packet_bytes);
            string[] asdasd = packet_string.Split('-');
            packet_string = string.Join("", asdasd);
            String text = "\r\n" + packet.Timestamp.ToString("yyyy-MM-dd hh:mm:ss.fff") + " length:" + packet.Length + "\r\n" + "\r\n" + packet_string + "\r\n";
                        
            get_Info.find_data(packet_string, d2_index, mac_table, dataGridView1);
  
            if (!get_Info.Filtration.filtration(get_Info, filters_class,d2_index,"in", get_Info.Find_value))
            {
                bool print = true;
                foreach (var item in get_Info.SwitchMAC) // kontrola ci ramec nepatri switch-u
                {
                    if (get_Info.SourceMAC2 == item || get_Info.DestinationMAC2 == item)
                    {
                        print = false;
                        break;
                    }
                }
                if (print)
                {
                    richTextBox2.BeginInvoke(new Action(() => richTextBox2.SelectionColor = Color.Red));
                    richTextBox2.BeginInvoke(new Action(() => richTextBox2.AppendText("Zablokované z: " + get_Info.SourceMAC + "\n")));
                }
                return;
            }
            get_Info.fill_mac_table_dict();
            get_Info.get_statistics(packet_string, statistics_d2_I);
            dataGridView2.BeginInvoke(new Action(() => dataGridView2.DataSource = statistics_table.fill_statistics_table(statistics_d1_I, statistics_d1_O, statistics_d2_I, statistics_d2_O)));

            int PnumberSend;
            if (get_Info.Mac_table.ContainsKey(get_Info.DestinationMAC))
            {
                if (get_Info.Mac_table[get_Info.DestinationMAC].Item2 == d2_index)
                {
                    return;
                }
                PnumberSend = get_Info.Mac_table[get_Info.DestinationMAC].Item2; //cislo portu na ktory sa ma paket odoslat
            }
            else
            {
                PnumberSend = d1_index;
            }

            Sending s = new Sending(packet, device1);
            if (!get_Info.Filtration.filtration(get_Info, filters_class, PnumberSend, "out", get_Info.Find_value)) // kontrola filtrov
            {
                bool print = true;
                foreach (var item in get_Info.SwitchMAC) // kontrola ci ramec nepatri switch-u
                {
                    if (get_Info.SourceMAC2 == item || get_Info.DestinationMAC2 == item)
                    {
                        print = false;
                        break;
                    }
                }
                if (print)
                {
                    richTextBox2.BeginInvoke(new Action(() => richTextBox2.SelectionColor = Color.Red));
                    richTextBox2.BeginInvoke(new Action(() => richTextBox2.AppendText("Zablokované na: " + get_Info.SourceMAC + "\n")));
                }
                return;
            }
            get_Info.get_statistics(packet_string, statistics_d1_O);
            s.send(communicator1);

            richTextBox2.BeginInvoke(new Action(() => richTextBox2.AppendText(text)));
            dataGridView2.BeginInvoke(new Action(() => dataGridView2.DataSource = statistics_table.fill_statistics_table(statistics_d1_I, statistics_d1_O, statistics_d2_I, statistics_d2_O)));
        }

        public Get_Info Get_Info
        {
            get { return get_Info; }
            set { get_Info = value; }
        }
    }
}
