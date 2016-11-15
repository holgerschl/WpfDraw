using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfDraw.Model
{
    class ModusChangedEventArgs:EventArgs
    {
        public Modus Modus { get; private set; }

        public ModusChangedEventArgs(Modus modus)
        {
            Modus = modus;
        }
    }
}
