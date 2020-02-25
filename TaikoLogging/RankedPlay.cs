using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaikoLogging
{
    class RankedPlay : Play
    {
        public int MatchNumber { get; set; }
        public int Difference { get; set; }
        public string Result { get; set; }

        public int OppScore { get; set; }

        public float OppAcc { get; set; }

        public int OppGOOD { get; set; }
        public int OppOK { get; set; }
        public int OppBAD { get; set; }

        public int OppCombo { get; set; }
        public int OppDrumroll { get; set; }
        public RankedPlay()
        {

        }
    }
}
