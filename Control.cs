using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Surface.Presentation.Controls;
using System.Windows.Input;
using System.Windows;
using System.Windows.Media;

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
        public ScatterView mainScatterView = null;

        public PlayerList playerList;
        public ArticleList articleList;
        public FinishedArticles finishedArticles;

        //public int player1points;
        //public int player2points;

        //public Island island1;
        //public Island island2;

        public Random rnd; // seed for random numbers

        private ScatterViewItem menu = new ScatterViewItem();
        private string menuText = "";
        public bool menuShown = false;
        public bool pointMode = true;
        public bool players3and4 = true;

        /// <summary>
        /// constructor
        /// </summary>
        private Control() 
        {
            //player1points = 0;
            //player2points = 0;
            rnd = new Random();

            menu.Width = 600;
            menu.Height = 400;
            menu.Center = new Point(1920 / 2, 1080 / 2);

            menu.CanMove = false;
            menu.CanRotate = false;
            menu.CanScale = false;
            menu.ShowsActivationEffects = false;

            menu.Background = Brushes.White;
            menu.Padding = new Thickness(60);

            menu.FontFamily = new FontFamily("Poiret One");
            menu.FontSize = 20;

            updateMenu();

            menu.Visibility = Visibility.Hidden;

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

        /// <summary>
        /// update menu
        /// </summary>
        public void updateMenu()
        {
            menuText = "NEWS STREAM v0.1: Wartungsmodus";
            menuText += Environment.NewLine;
            menuText += Environment.NewLine;
            menuText += "[S] Spieler: ";
            if (players3and4)
            {
                menuText += "1-4";
            }
            else
            {
                menuText += "1-2";
            }
            menuText += Environment.NewLine;
            menuText += "[M] Modus: ";
            if (pointMode)
            {
                menuText += "Mit Punkten";
            }
            else
            {
                menuText += "Ohne Punkte";
            }
            menuText += Environment.NewLine;
            //menuText += "[A] Anzahl gleichzeitig angezeigter Artikel: 3";
            //menuText += Environment.NewLine;
            menuText += "[P] Punkte zurücksetzen";
            menuText += Environment.NewLine;
            menuText += Environment.NewLine;
            menuText += "[Esc] Menü verlassen";
            menu.Content = menuText;
        }

        /// <summary>
        /// handle keyboard input
        /// </summary>
        public void keyboardInput(KeyEventArgs e)
        {

            // show menu
            if (!menuShown)
            {
                if (e.Key == Key.Escape)
                {
                    if (!mainScatterView.Items.Contains(menu))
                    {
                        mainScatterView.Items.Add(menu);
                    }
                    menu.Visibility = Visibility.Visible;
                    menu.ZIndex = 100;
                    menuShown = true;
                }
            }
            else
            {
                // close menu
                if (e.Key == Key.Escape)
                 {
                    menu.Visibility = Visibility.Hidden;
                    menuShown = false;
                 }
                // reset points
                else if (e.Key == Key.P)
                {
                    foreach (Player player in playerList.players)
                    {
                        player.setPoints(0);
                    }
                }
                // switch mode
                else if (e.Key == Key.M)
                {
                    if (pointMode)
                    {
                        foreach (Player player in playerList.players)
                        {
                            player.island.pointDisplay.Visibility = Visibility.Hidden;

                            foreach (ScatterViewItem item in player.island.listImages)
                            {
                                if (item.Content != null)
                                {
                                    item.Background = Brushes.White;
                                    item.Foreground = Brushes.Black;
                                }
                                else
                                {
                                    item.Background = Brushes.Transparent;
                                    item.Foreground = Brushes.Transparent;
                                }
                            }
                            foreach (ScatterViewItem item in player.island.listTexts)
                            {
                                if (item.Content != null)
                                {
                                    item.Background = Brushes.White;
                                    item.Foreground = Brushes.Black;
                                }
                                else
                                {
                                    item.Background = Brushes.Transparent;
                                    item.Foreground = Brushes.Transparent;
                                }
                            }

                        }
                    }
                    else
                    {
                        foreach (Player player in playerList.players)
                        {
                            player.setPoints(0);
                            player.island.pointDisplay.Visibility = Visibility.Visible;
                            for (int i = 0; i < player.island.listImages.Count(); i++)
                            {
                                for (int j = 0; j < articleList.articles.Count(); j++)
                                {
                                    if (player.island.listTexts[i].Content != null && player.island.listTexts[i].Content.Equals(articleList.articles[j].textItem.Content))
                                    {
                                        player.island.listTexts[i].Background = articleList.articles[j].imageOwner.island.color;
                                        player.island.listImages[i].Background = articleList.articles[j].imageOwner.island.color;
                                        break;
                                    }
                                    else if (player.island.listTexts[i].Content == null)
                                    {
                                        player.island.listTexts[i].Background = Brushes.Transparent;
                                        player.island.listImages[i].Background = Brushes.Transparent;
                                    }
                                }
                                player.island.listTexts[i].Foreground = Brushes.White;
                                player.island.listImages[i].Foreground = Brushes.White;
                            }
                        }
                    }
                    pointMode = !pointMode;
                    updateMenu();
                    if (!players3and4)
                    {
                        playerList.players[2].island.pointDisplay.Visibility = Visibility.Hidden;
                        playerList.players[3].island.pointDisplay.Visibility = Visibility.Hidden;
                    }
                }
                // swicht between 1-2 and 1-4 players
                else if (e.Key == Key.S)
                {
                    if (players3and4)
                    {

                        playerList.players[2].island.island.Visibility = Visibility.Hidden;
                        playerList.players[2].island.imageSlot.Visibility = Visibility.Hidden;
                        playerList.players[2].island.textSlot.Visibility = Visibility.Hidden;
                        playerList.players[2].island.pointDisplay.Visibility = Visibility.Hidden;
                        playerList.players[2].island.finishedArticles.Visibility = Visibility.Hidden;

                        foreach ( ScatterViewItem item in playerList.players[2].island.listTexts )
                        {
                            item.Visibility = Visibility.Hidden;
                        }
                        foreach (ScatterViewItem item in playerList.players[2].island.listImages)
                        {
                            item.Visibility = Visibility.Hidden;
                        }

                        playerList.players[3].island.island.Visibility = Visibility.Hidden;
                        playerList.players[3].island.imageSlot.Visibility = Visibility.Hidden;
                        playerList.players[3].island.textSlot.Visibility = Visibility.Hidden;
                        playerList.players[3].island.pointDisplay.Visibility = Visibility.Hidden;
                        playerList.players[3].island.finishedArticles.Visibility = Visibility.Hidden;

                        foreach (ScatterViewItem item in playerList.players[3].island.listTexts)
                        {
                            item.Visibility = Visibility.Hidden;
                        }
                        foreach (ScatterViewItem item in playerList.players[3].island.listImages)
                        {
                            item.Visibility = Visibility.Hidden;
                        }

                        playerList.players[2].setPoints(0);
                        playerList.players[3].setPoints(0);

                    }
                    else
                    {

                        playerList.players[2].island.island.Visibility = Visibility.Visible;
                        playerList.players[2].island.imageSlot.Visibility = Visibility.Visible;
                        playerList.players[2].island.textSlot.Visibility = Visibility.Visible;
                        if (pointMode)
                        {
                            playerList.players[2].island.pointDisplay.Visibility = Visibility.Visible;
                        }
                        playerList.players[2].island.finishedArticles.Visibility = Visibility.Visible;

                        foreach (ScatterViewItem item in playerList.players[2].island.listTexts)
                        {
                            item.Visibility = Visibility.Visible;
                        }
                        foreach (ScatterViewItem item in playerList.players[2].island.listImages)
                        {
                            item.Visibility = Visibility.Visible;
                        }

                        playerList.players[3].island.island.Visibility = Visibility.Visible;
                        playerList.players[3].island.imageSlot.Visibility = Visibility.Visible;
                        playerList.players[3].island.textSlot.Visibility = Visibility.Visible;
                        if (pointMode)
                        {
                            playerList.players[3].island.pointDisplay.Visibility = Visibility.Visible;
                        }
                        playerList.players[3].island.finishedArticles.Visibility = Visibility.Visible;

                        foreach (ScatterViewItem item in playerList.players[3].island.listTexts)
                        {
                            item.Visibility = Visibility.Visible;
                        }
                        foreach (ScatterViewItem item in playerList.players[3].island.listImages)
                        {
                            item.Visibility = Visibility.Visible;
                        }

                    }
                    players3and4 = !players3and4;
                    updateMenu();
                }
            }

        }

    }
}
