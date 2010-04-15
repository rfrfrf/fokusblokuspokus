using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Blokus.Logic;

namespace Blokus.Misc
{
    public class HandClickEventArgs : EventArgs
    {
        public Piece Piece { get; set; }

        public HandClickEventArgs(Piece piece)
        {
            Piece = piece;
        }
    }
}
