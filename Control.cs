using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Surface.Presentation.Controls;

/*
 * Class for basic game data that may be used in several other classes
 * Implemented as singelton
 */

namespace PhotoPaint
{
    class Control
    {

        private static Control instance; // singleton instance

        public Window1 window1;
        public ScatterView mainScatterView;

        public int player1points;
        public int player2points;

        public Island island1;
        public Island island2;

        public Random rnd; // seed for random numbers

        /// <summary>
        /// constructor
        /// </summary>
        private Control() 
        {
            player1points = 0;
            player2points = 0;
            rnd = new Random();
        }

        /// <summary>
        /// singleton function
        /// </summary>
        public static Control Instance
           {
              get 
              {
                 if (instance == null)
                 {
                     instance = new Control();
                 }
                 return instance;
              }
           }

    }
}
