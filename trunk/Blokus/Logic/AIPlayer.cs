using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blokus.Logic
{
    [Serializable]
    public abstract class AIPlayer : PlayerBase
    {
        #region PlayerBase Members

        public override Move GetMove(GameState gameState)
        {
            return null;
        }

        public override void OnGameStart(GameState gameState) { }

        public override void OnGameEnd(GameState gameState) { }

        public override void CancelMove(GameState gameState) { }        

        #endregion //PlayerBase Members
    }
}
