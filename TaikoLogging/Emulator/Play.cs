using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaikoLogging.Emulator
{
    class Play
    {
        // I don't like this class
        // So many items are doubled just because there's a Latest play and a Best play in the result file, and I didn't notice the Latest play until way too late

        public SongData SongData { get; set; }
        public int Score { get; set; }
        public int LastScore { get; set; }
        public int GOOD { get; set; }
        public int LastGoods { get; set; }
        public int OK { get; set; }
        public int LastOKs { get; set; }
        public int BAD { get; set; }
        public int LastBads { get; set; }
        public int Combo { get; set; }
        public int LastCombo { get; set; }
        //public double RecentOKs { get; set; }
        //public double RecentBads { get; set; }

        public float Accuracy { get; set; }
        public float LastAcc { get; set; }
        public float RecentAcc { get; set; }
        public float RecentAccChange { get; set; }

        public DateTime ScoreDateTime { get; set; }
        public DateTime LatestDateTime { get; set; }

        public ImageAnalysis.Mode Mode { get; set; }

        public Play()
        {
            Mode = ImageAnalysis.Mode.Emulator;
        }
    }
}
