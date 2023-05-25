using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloppyControlApp.MyClasses.Processing.ProcessingTypes
{
    public class ProcTypeArgs
    {
        public long MINUS { get; set; }
        public long FOURUS { get; set; }
        public long SIXUS { get; set; }
        public long EIGHTUS { get; set; }
        public byte[] Rxbuf { get; set; }
        public float[] entropy { get; set; }
        public long[] Mfmlengths { get; set; }
        public long[] Progresses { get; set; }
        public float RateOfChange { get; set; }

        public ProcSettings Procsettings { get; set; }

        public StringBuilder Tbreceived { get; set; }

    }
}
