using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FloppyControlApp.MyClasses.Processing
{
    public class ECSettings
    {
        public ulong periodSelectionStart { get; set; }
        public ulong periodSelectionEnd { get; set; }
        public long indexS1 { get; set; }
        public long threadid { get; set; }
        public int combinations { get; set; }
        public ulong C6Start { get; set; }
        public ulong C8Start { get; set; }
        public int MFMByteStart { get; set; }
        public int MFMByteLength { get; set; }
        public TextBox sectortextbox { get; set; }
		public int BitShift { get; internal set; }
	}
}
