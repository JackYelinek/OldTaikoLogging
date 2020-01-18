using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaikoLogging.Emulator
{
    class EmulatorPlay
    {
        public string Title { get; set; }
        public int Score { get; set; }
        public int Goods { get; set; }
        public int OKs { get; set; }
        public int Bads { get; set; }
        public int Combo { get; set; }
        public DateTime DateTime { get; set; }

        public EmulatorPlay()
        {

        }
    }
}
