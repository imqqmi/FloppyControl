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
        public long periodSelectionStart { get; set; }
        public long periodSelectionEnd { get; set; }
        public long indexS1 { get; set; }
        public long threadid { get; set; }
        public int combinations { get; set; }
        public long C6Start { get; set; }
        public long C8Start { get; set; }
        public int MFMByteStart { get; set; }
        public int MFMByteLength { get; set; }
        public TextBox sectortextbox { get; set; }
		public int BitShift { get; internal set; }
	}
}
