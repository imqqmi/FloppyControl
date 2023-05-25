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
        public int periodSelectionStart { get; set; }
        public int periodSelectionEnd { get; set; }
        public int indexS1 { get; set; }
        public int threadid { get; set; }
        public int combinations { get; set; }
        public int C6Start { get; set; }
        public int C8Start { get; set; }
        public int MFMByteStart { get; set; }
        public int MFMByteLength { get; set; }
        public TextBox sectortextbox { get; set; }
		public int BitShift { get; internal set; }
	}
}
