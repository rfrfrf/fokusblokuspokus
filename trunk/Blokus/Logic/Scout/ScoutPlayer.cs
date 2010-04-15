using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blokus.Logic.Scout
{
    class ScoutPlayer : AIPlayer
    {
        public override Move GetMove(GameState gameState)
        {
            return null; //TODO: wyszukiwanie ruchu scoutem
        }        

        public override string ToString()
        {
            return "Scout";
        }
    }
}
