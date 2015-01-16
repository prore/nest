using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Surface.Presentation.Controls;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows;
using Microsoft.Surface.Presentation;
using System.Windows.Media;

/*
 * a players island
 */

namespace PhotoPaint
{
    class Island 
    {
        public ScatterViewItem island; //the island itself (background)
        public ScatterViewItem imageSlot; //slot for an image
        public ScatterViewItem textSlot; // slot for a text item
        public ScatterViewItem pointDisplay; // element to show current points

        private String path;
        public int playerNumber;
        private int posX, imageX, textX, y;

        public int orientation;

        //public Player player; //owner of this island

        /// <summary>
        /// constructor
        /// </summary>
        public Island(String path, int playerNumber)
        {
            this.path = path;
            this.playerNumber = playerNumber;

            island = new ScatterViewItem();
            imageSlot = new ScatterViewItem();
            textSlot = new ScatterViewItem();
            pointDisplay = new ScatterViewItem();

            island.ZIndex = 0;
            imageSlot.ZIndex = 1;

            setPosValues();

            createIsland();
            createSlots();
            createPointDisplay();
            
            Control.Instance.mainScatterView.Items.Add(island);
            Control.Instance.mainScatterView.Items.Add(imageSlot);
            Control.Instance.mainScatterView.Items.Add(textSlot);
            Control.Instance.mainScatterView.Items.Add(pointDisplay);

        }

        /// <summary>
        /// set position values
        /// </summary>
        private void setPosValues()
        {
            y = 540;
            if (playerNumber == 1)
            {
                orientation = 90;
                posX = 145;
                imageX = posX-50;
                textX = posX+70; 
            }
            else if (playerNumber == 2)
            {
                orientation = 270;
                posX = 1815;
                imageX = posX+50;
                textX = posX-70;
            }

        }

        /// <summary>
        /// create island element
        /// </summary>
        private void createIsland()
        {
            island.Height = 250;
            island.Width = 300;
            island.MinWidth = island.Width;
            island.MaxWidth = island.Width;
            island.Center = new System.Windows.Point(posX, y);
            island.CanMove = false;
            island.CanRotate = false;
            island.CanScale = false;
            island.IsEnabled = false;
            island.Orientation = orientation;

            Image img1 = new Image();
            img1.Source = new BitmapImage(new Uri(path));

            island.Content = img1;

        }

        /// <summary>
        /// create slot elements for image and headline
        /// </summary>
        private void createSlots()
        {
            imageSlot.Width = 427 / 2.5;
            imageSlot.Height = 320 / 2.5;
            imageSlot.CanMove = false;
            imageSlot.CanRotate = false;
            imageSlot.CanScale = false;
            imageSlot.IsEnabled = false;
            imageSlot.Center = new System.Windows.Point(imageX, y);
            imageSlot.Orientation = orientation;

            textSlot.Width = 427 / 2.5;
            textSlot.Height = 80;
            textSlot.CanMove = false;
            textSlot.CanRotate = false;
            textSlot.CanScale = false;
            textSlot.IsEnabled = false;
            textSlot.Center = new System.Windows.Point(textX, y);
            textSlot.Orientation = orientation;
       }

        /// <summary>
        /// create element to show current points
        /// </summary>
        private void createPointDisplay()
        {
            pointDisplay.Width = 427 / 2.5;
            pointDisplay.Height = 320 / 2.5;
            pointDisplay.CanMove = false;
            pointDisplay.CanRotate = false;
            pointDisplay.CanScale = false;
            pointDisplay.IsEnabled = false;
            pointDisplay.Center = new System.Windows.Point(imageX, y);
            pointDisplay.Orientation = orientation;
            pointDisplay.Content = 0;
            pointDisplay.Background = Brushes.Transparent;
            pointDisplay.FontSize = 30;
            pointDisplay.HorizontalContentAlignment = HorizontalAlignment.Center;
            pointDisplay.VerticalContentAlignment = VerticalAlignment.Center;
        }

    }
}
