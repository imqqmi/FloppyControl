namespace FloppyControlApp.MyClasses.Capture.Models
{
    public class MFMData
    {
        public int crc { get; set; }
        public long threadid { get; set; }
        public long MarkerPositions { get; set; }
        public long rxbufMarkerPositions { get; set; }
        public SectorMapStatus Status { get; set; }
        public int track { get; set; }
        public int trackhead { get; set; }
        public int sector { get; set; }
        public int head { get; set; }
        public int sectorlength { get; set; }
        public int DataIndex { get; set; }
        public int HeaderIndex { get; set; }
        public byte[] sectorbytes { get; set; }
        public MarkerType MarkerType { get; set; }
        public bool processed { get; set; }

    }
}
