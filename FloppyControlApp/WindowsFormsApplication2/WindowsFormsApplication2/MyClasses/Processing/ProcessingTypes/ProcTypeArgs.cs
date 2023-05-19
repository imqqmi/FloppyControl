using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloppyControlApp.MyClasses.Processing.ProcessingTypes
{
    public class ProcTypeArgs
    {
        public int MINUS { get; set; }
        public int FOURUS { get; set; }
        public int SIXUS { get; set; }
        public int EIGHTUS { get; set; }
        public byte[] Rxbuf { get; set; }
        public float[] entropy { get; set; }
        public int[] Mfmlengths { get; set; }
        public int[] Progresses { get; set; }
        public float RateOfChange { get; set; }

        public ProcSettings Procsettings { get; set; }

        public StringBuilder Tbreceived { get; set; }

    }
}
