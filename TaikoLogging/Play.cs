using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaikoLogging
{
    class Play
    {
        public Bitmap Bmp { get; set; }

        public string Title { get; set; }
        public string Genre { get; set; }
        public ImageAnalysis.Difficulty Difficulty { get; set; }
        public int Level { get; set; }

        public int Score { get; set; }
        public int GOOD { get; set; }
        public int OK { get; set; }
        public int BAD { get; set; }

        public int Combo { get; set; }
        public int Drumroll { get; set; }

        public List<string> Mods { get; set; }
        public string Account { get; set; }
        public string Mode { get; set; }

        public DateTime Time { get; set; }

        public Play()
        {
            
        }


    }
}
