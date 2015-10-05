using PcapDotNet.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace xcicman_zadanie1_PSIP
{
    class Find_Adapter
    { // vyhladanie adapterov a naplnenie comboboxu
        public Find_Adapter(ComboBox comb1, ComboBox comb2)
        {
            IList<LivePacketDevice> allDevices = LivePacketDevice.AllLocalMachine;
            Dictionary<string, LivePacketDevice> adapters = new Dictionary<string, LivePacketDevice>();
            adapters.Add("Chose adapter for port.", null);
            for (int i = 0; i != allDevices.Count; ++i)
            {
                LivePacketDevice device = allDevices[i];
                adapters.Add((i + 1) + ". " + device.Description, device);
            }
            comb1.DataSource = new BindingSource(adapters, null);
            comb1.DisplayMember = "Key";
            comb1.ValueMember = "Value";
            comb2.DataSource = new BindingSource(adapters, null);
            comb2.DisplayMember = "Key";
            comb2.ValueMember = "Value";
        }
    }
}
