using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Surface.Presentation.Controls;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

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

        private String path;
        public int playerNumber;
        private int posX, imageX, textX, y, orientation;

        //public Player player; //owner of this island

        public Island(String path, int playerNumber)
        {
            this.path = path;
            this.playerNumber = playerNumber;

            island = new ScatterViewItem();
            imageSlot = new ScatterViewItem();
            textSlot = new ScatterViewItem();

            island.ZIndex = 0;
            imageSlot.ZIndex = 1;

            setPosValues();

            createIsland();
            createSlots();
            

            Control.Instance.mainScatterView.Items.Add(island);
            Control.Instance.mainScatterView.Items.Add(imageSlot);
            Control.Instance.mainScatterView.Items.Add(textSlot);
            

        }
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
                orientation = 90;
                posX = 1815;
                imageX = posX+50;
                textX = posX-70;
            }

        }
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

    }
}
