using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloppyControlApp.MyClasses
{
    public enum SectorMapStatus
    {
        empty = 0,
        CrcOk = 1,
        HeadOkDataBad = 2,
        BadSectorUnique = 3,
        DuplicatesFound = 4,
        SectorOKButZeroed = 5,
        ErrorCorrected = 6,
        ErrorCorrected2 = 7,
        AmigaCrcOk = 11,
        AmigaHeadOkDataBad = 12,
        CrcBad = 13
    }

    public enum ProcessingType { adaptive1, normal, aufit, adaptive2, adaptive3, adaptivePredict, adaptiveEntropy,adaptivea }
    public enum FluxEncoding { MFM = 1, FM = 2, GRC = 3 }
    public enum ScanMode { AuScan, OffsetScan, OffsetScan2, ExtremeScan, AdaptiveRate, AdaptiveOffsetRate, AdaptiveDeep, AdaptiveShallow, AdaptiveNarrow, AdaptiveNarrowRate }

    public enum DiskFormat { unknown = 0, amigados = 1, diskspare = 2, pcdd = 3, pchd = 4, pc2m = 5, pcssdd = 6, diskspare984KB = 7, pc360kb525in = 8 }
    public enum MarkerType { unkown = 0, header = 1, data = 2, headerAndData = 3 }

    public enum Platform {  PC, Amiga }

    public class Helpers
    {
        
    }
}
