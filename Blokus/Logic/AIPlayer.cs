using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blokus.Logic
{
    public abstract class AIPlayer : PlayerBase
    {
        #region PlayerBase Members

        public override Move GetMove(GameState gameState)
        {
            return null;
        }

        public override void OnGameStart() {}

        public override void OnGameEnd() { }

        public override void CancelMove() { }        

        #endregion //PlayerBase Members
    }
}
