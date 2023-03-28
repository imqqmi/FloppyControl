using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloppyControlApp.MyClasses.Processing
{
    public class DiskGeometry
    {
        public int sectorsPerTrack { get; set; }
        public int tracksPerDisk { get; set; }
        public int numberOfHeads { get; set; }
        public int sectorSize { get; set; }
        public FluxEncoding fluxEncoding { get; set; }
    }
}
