using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaikoLogging.Emulator
{
    class Play
    {
        public SongData SongData { get; set; }
        public int Score { get; set; }
        public int Goods { get; set; }
        public int LastGoods { get; set; }
        public int OKs { get; set; }
        public int LastOKs { get; set; }
        public int Bads { get; set; }
        public int LastBads { get; set; }
        public int Combo { get; set; }
        public double RecentOKs { get; set; }
        public double RecentBads { get; set; }

        public DateTime ScoreDateTime { get; set; }
        public DateTime LatestDateTime { get; set; }

        public Play()
        {

        }
    }
}
