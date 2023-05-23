using FloppyControlApp.MyClasses.Capture.Models;

namespace FloppyControlApp
{
	public partial class FloppyControl
	{
		#region model classes

		class SectorMapContextMenu
        {
            public int Track { get; set; }
            public int Sector { get; set; }
            public int Duration { get; set; }
            public MFMData Sectordata { get; set; }
            public int Cmd { get; set; }
        }
    }
}
