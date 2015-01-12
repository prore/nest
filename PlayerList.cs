using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhotoPaint
{
    class PlayerList
    {

        public List<Player> players;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="numberOfPlayers">number of players to be able to play the game</param>
        public PlayerList(int numberOfPlayers) 
        {
            players = new List<Player>();
            for (int i = 0; i < numberOfPlayers; i++)
            {
                players.Add(new Player(i * 90));
            }
        }

        /// <summary>
        /// reset all players
        /// </summary>
        public void resetAll()
        {
            foreach (Player player in players)
            {
                player.resetPlayer();
            }
        }

    }
}
