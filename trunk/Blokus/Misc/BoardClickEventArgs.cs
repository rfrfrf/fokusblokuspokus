using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Blokus.Logic;

namespace Blokus.Misc
{
    public class BoardClickEventArgs : EventArgs
    {
        public PiecePosition PiecePosition {get;set;}

        public BoardClickEventArgs(PiecePosition position)
        {
            PiecePosition = new PiecePosition(position);
        }
    }
}
