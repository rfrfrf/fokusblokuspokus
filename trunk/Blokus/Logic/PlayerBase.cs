using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blokus.Logic
{
    public abstract class PlayerBase
    {
        public abstract Move GetMove(GameState gameState);
        public abstract void OnGameStart();
        public abstract void OnGameEnd();
        public abstract void CancelMove();
    }
}
