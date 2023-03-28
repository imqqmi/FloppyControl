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
    public class ProcSettings : ICloneable
    {
        private int poffset, pmin, pfour, psix, pmax, pstart, pend, ppattern;

        private float prateofchange;
        bool pSkipPeriodData, pfinddupes, pUseErrorCorrection, pAddnoise;
        ProcessingType pprocessingtype;

        public int offset { get { return poffset; } set { poffset = value; } }
        public int min { get { return pmin; } set { pmin = value; } }
        public int four { get { return pfour; } set { pfour = value; } }
        public int six { get { return psix; } set { psix = value; } }
        public int max { get { return pmax; } set { pmax = value; } }
        public int start { get { return pstart; } set { pstart = value; } }
        public int end { get { return pend; } set { pend = value; } }
        public Platform platform { get; set; }
        public int pattern { get { return ppattern; } set { ppattern = value; } }
        public int addnoiselimitstart { get; set; }
        public int addnoiselimitend { get; set; }
        public int addnoiserndamount { get; set; }
        public int NumberOfDups { get; set; }
        public int limittotrack { get; set; }
        public int limittosector { get; set; }
        public int hd { get; set; }
        public int AdaptOffset { get; set; }
        public float AdaptOffset2 { get; set; }

        public float rateofchange { get { return prateofchange; } set { prateofchange = value; } }
        public float rateofchange2 { get; set; }
        public bool SkipPeriodData { get { return pSkipPeriodData; } set { pSkipPeriodData = value; } }
        public bool finddupes { get { return pfinddupes; } set { pfinddupes = value; } }
        public bool UseErrorCorrection { get { return pUseErrorCorrection; } set { pUseErrorCorrection = value; } }
        public bool OnlyBadSectors { get; set; }
        public bool AddNoise { get { return pAddnoise; } set { pAddnoise = value; } }
        public bool LimitTSOn { get; set; }
        public bool IgnoreHeaderError { get; set; }
        public bool AutoRefreshSectormap { get; set; }

        public string outputfilename { get; set; }

        public ProcessingType processingtype { get { return pprocessingtype; } set { pprocessingtype = value; } }

        public ProcSettings()
        {
        }
        public object Clone()
        {
            MemoryStream ms = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(ms, this);
            ms.Position = 0;
            object obj = bf.Deserialize(ms);
            ms.Close();
            return obj;
        }
    }
}
