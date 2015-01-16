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
                players[i].island = new Island(@"pack://application:,,,/Resources/Island1.png", i + 1);
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

        /// <summary>
        /// get the player who is the owner of a given island
        /// </summary>
        public Player getPlayer(Island island)
        {
            foreach (Player player in players)
            {
                if (player.island.Equals(island))
                {
                    return player;
                }
            }
            return null;
        }

    }
}
