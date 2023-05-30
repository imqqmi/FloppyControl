using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace FloppyControlApp.MyClasses
{
    [Serializable]
    public class ProcSettings
    {
        private float prateofchange;
        bool pSkipPeriodData, pfinddupes, pUseErrorCorrection, pAddnoise;
        ProcessingType pprocessingtype;

        public long offset { get;  set;  }
        public int min { get; set; }
		public int four { get; set; }
		public int six { get; set; }
		public int max { get; set; }
        public long start { get; set; }
        public long end { get; set; }
        public Platform platform { get; set; }
        public int pattern { get; set; }
        public int addnoiselimitstart { get; set; }
        public int addnoiselimitend { get; set; }
        public int addnoiserndamount { get; set; }
        public int NumberOfDups { get; set; }
        public int limittotrack { get; set; }
        public int limittosector { get; set; }
        public int hd { get; set; }
        public int AdaptOffset { get; set; }
        public float AdaptOffset2 { get; set; }

        public float rateofchange { get; set; }
        public float rateofchange2 { get; set; }
        public bool SkipPeriodData { get; set; }
        public bool finddupes { get; set; }
        public bool UseErrorCorrection { get; set; }
        public bool OnlyBadSectors { get; set; }
        public bool AddNoise { get; set; }
        public bool LimitTSOn { get; set; }
        public bool IgnoreHeaderError { get; set; }
        public bool AutoRefreshSectormap { get; set; }

        public string outputfilename { get; set; }

        public ProcessingType processingtype { get; set; }

        public ProcSettings()
        {
        }

    }
}
