using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PcapDotNet.Core;
using PcapDotNet.Packets;
using System.Windows.Forms;
using System.Threading;

namespace xcicman_zadanie1_PSIP
{
    public class Receive
    {
        private static RichTextBox richTextBox1 = null;
        private PacketDevice device1, device2;
        private static string packet_string;

        public Receive(RichTextBox richTextBox1_receive, PacketDevice device1_receive, PacketDevice device2_receive)
        {
            richTextBox1 = richTextBox1_receive;
            device1 = device1_receive;
            device2 = device2_receive;
        }

        public void a1_receive()
        {
            try{
            using (PacketCommunicator communicator = device1.Open(65536, PacketDeviceOpenAttributes.Promiscuous, 1000))
                {
                    String text = "Listening on " + device1.Description + "... \r\n";
                    richTextBox1.BeginInvoke(new Action(() => richTextBox1.AppendText(text)));
                    // start the capture
                    communicator.ReceivePackets(0, PacketHandler);
                    communicator.Break();
                
            }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void a2_receive()
        {
            try
            {
                using (PacketCommunicator communicator = device2.Open(65536, PacketDeviceOpenAttributes.Promiscuous, 1000))
                {
                    String text = "Listening on " + device2.Description + "... \r\n";
                    richTextBox1.BeginInvoke(new Action(() => richTextBox1.AppendText(text)));
                    // start the capture
                    communicator.ReceivePackets(0, PacketHandler);
                    communicator.Break();

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void PacketHandler(Packet packet)
        {
            byte[] packet_bytes = new byte[packet.Length];
            packet_bytes = packet.Buffer;
            packet_string = BitConverter.ToString(packet_bytes);
            string[] asdasd = packet_string.Split('-');
            packet_string = string.Join("", asdasd);
            String text = "\r\n" + packet.Timestamp.ToString("yyyy-MM-dd hh:mm:ss.fff") + " length:" + packet.Length + "\r\n" + "\r\n" + packet_string + "\r\n";
            richTextBox1.BeginInvoke(new Action(() => richTextBox1.AppendText(text)));
        }
    }
}
