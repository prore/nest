using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Surface.Presentation.Controls;

/*
 * represents a player of the game
 */

namespace PhotoPaint
{
    class Player
    {

        //public readonly int playerId;

        private int points;

        //public int orientation; // specifies orientation of elements in degrees to show something to this user
        public Island island;

        //public ScatterViewItem currentImage; // image currently in the image slot
        //public ScatterViewItem currentText; // text currently in the text slot

        public List<Article> finishedArticles; // articles to show in the list of finished articles

        /// <summary>
        /// constructor
        /// </summary>
        public Player(int orientation)
        {
            //this.orientation = orientation;
            points = 0;
            finishedArticles = new List<Article>();
        }

        /// <summary>
        /// Getter for points
        /// </summary>
        public int getPoints()
        {
            return points;
        }

        /// <summary>
        /// Setter for points
        /// </summary>
        public void setPoints(int points)
        {
            this.points = points;
            island.pointDisplay.Content = points;
        }

        /// <summary>
        /// reset player data
        /// </summary>
        public void resetPlayer()
        {
            points = 0;
            finishedArticles = new List<Article>();
        }

    }
}
