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
using Microsoft.Surface.Presentation.Generic;
using System.Diagnostics;


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
        public ScatterViewItem finishedArticles; // area for finished articles
        public ScatterViewItem pointDisplay; // element to show current points

        private String path;
        public int playerNumber;

        private Brush color;

        //private int posX, imageX, textX, y;

        private int[] islandSize           = new int[] { 450, 250 };
        private int[] textSlotSize         = new int[] { 170,  80 };
        private int[] imageSlotSize        = new int[] { 170, 128 };
        private int[] finishedArticlesSize = new int[] { 234, 224 };
        private int[] pointDisplaySize     = new int[] {  50,  50 };

        private int[] textSlotOffset         = new int[] {-125,   70 };
        private int[] imageSlotOffset        = new int[] {-125,  -50 };
        private int[] finishedArticlesOffset = new int[] {  92,    2 };
        private int[] pointDisplayOffset     = new int[] {   0,  140 };

        private int[] tableResolution = new int[] { 1920, 1080 };

        private int[] positionBase;

        private int xValue; // parameter index of coordinates that defines horizontal values
        private int yValue; // parameter index of coordinates that defines vertical values

        public int orientation;

        //public Player player; //owner of this island

        /// <summary>
        /// constructor
        /// </summary>
        public Island(String path, int playerNumber)
        {
            this.path = path;
            this.playerNumber = playerNumber;

            int[] posCorrection = new int[] { 20, 20, -45, -45 }; // necessary offset of island centers, not sure where that necessity comes from

            switch (playerNumber)
            {
                case 0: orientation =  90; xValue = 1; yValue = 0;
                    positionBase = new int[] { tableResolution[xValue] / 2, islandSize[1] / 2 + posCorrection [0]};
                    color = Brushes.DarkBlue;
                    break;
                case 1: orientation = 270; xValue = 1; yValue = 0;
                    positionBase = new int[] { tableResolution[xValue] / 2, tableResolution[yValue] - islandSize[1] / 2 + posCorrection[1] };
                    color = Brushes.DarkGreen;
                    break;
                case 2: orientation =   0; xValue = 0; yValue = 1;
                    positionBase = new int[] { tableResolution[xValue] / 2, tableResolution[yValue] - islandSize[1] / 2 + posCorrection[2] };
                    color = Brushes.DarkRed;
                    break;
                case 3: orientation = 180; xValue = 0; yValue = 1;
                    positionBase = new int[] { tableResolution[xValue] / 2, islandSize[1] / 2 + posCorrection[3] };
                    color = Brushes.DarkMagenta;
                    break;
            }

            if (orientation == 0 || orientation == 270)
            {
                textSlotOffset[1] *= -1;
                imageSlotOffset[1] *= -1;
                pointDisplayOffset[0] *= -1;
                pointDisplayOffset[1] *= -1;
            }

            if (orientation == 180 || orientation == 270)
            {
                textSlotOffset[0] *= -1;
                imageSlotOffset[0] *= -1;
                finishedArticlesOffset[0] *= -1;
            }

            if (orientation == 90 || orientation == 180)
            {
                finishedArticlesOffset[1] *= -1;
            }

            island = new ScatterViewItem();
            imageSlot = new ScatterViewItem();
            textSlot = new ScatterViewItem();
            finishedArticles = new ScatterViewItem();
            pointDisplay = new ScatterViewItem();

            island.ZIndex = 0;
            imageSlot.ZIndex = 1;
            textSlot.ZIndex = 1;
            finishedArticles.ZIndex = 1;
            pointDisplay.ZIndex = 1;

            island.IsTopmostOnActivation = false;
            imageSlot.IsTopmostOnActivation = false;
            textSlot.IsTopmostOnActivation = false;
            finishedArticles.IsTopmostOnActivation = false;
            pointDisplay.IsTopmostOnActivation = false;

            //setPosValues();
           

            createIsland();
            createSlots();
            createFinishedArticles();
            createPointDisplay();
            
            Control.Instance.mainScatterView.Items.Add(island);
            Control.Instance.mainScatterView.Items.Add(imageSlot);
            Control.Instance.mainScatterView.Items.Add(textSlot);
            Control.Instance.mainScatterView.Items.Add(finishedArticles);
            Control.Instance.mainScatterView.Items.Add(pointDisplay);

        }

        /// <summary>
        /// set position values
        /// </summary>
        //private void setPosValues()
        //{
        //    y = 540;
        //    if (playerNumber == 1)
        //    {
        //        orientation = 90;
        //        posX = 145;
        //        imageX = posX-50;
        //        textX = posX+70; 
        //    }
        //    else if (playerNumber == 2)
        //    {
        //        orientation = 270;
        //        posX = 1815;
        //        imageX = posX+50;
        //        textX = posX-70;
        //    }

        //}

        /// <summary>
        /// create island element
        /// </summary>
        private void createIsland()
        {

            island.Width = islandSize[0];
            island.Height = islandSize[1];
            island.MinWidth = island.Width;
            island.MaxWidth = island.Width;
            island.Center = new Point(positionBase[xValue], positionBase[yValue]);

            island.Orientation = orientation;

            island.CanMove = false;
            island.CanRotate = false;
            island.CanScale = false;
            //island.IsEnabled = false;
            island.ShowsActivationEffects = false;
            
            //Image img1 = new Image();
            //img1.Source = new BitmapImage(new Uri(path));
            //island.Content = img1;
//            island.Background = Brushes.DarkOliveGreen;

            BitmapImage img = new BitmapImage();
            //load the image from a local resource
            img.BeginInit();
            img.UriSource = new Uri("pack://application:,,,/Resources/" + "island_yellow.png", UriKind.Absolute);
            img.EndInit();

            island.Background = new ImageBrush(img);

            island.ShowsActivationEffects = false;
            RoutedEventHandler loadedEventHandler = null;
            loadedEventHandler = new RoutedEventHandler(delegate
            {
                island.Loaded -= loadedEventHandler;
                Microsoft.Surface.Presentation.Generic.SurfaceShadowChrome ssc;
                ssc = island.Template.FindName("shadow", island) as Microsoft.Surface.Presentation.Generic.SurfaceShadowChrome;
                ssc.Visibility = Visibility.Hidden;
            });
            island.Loaded += loadedEventHandler;


        }

        /// <summary>
        /// create slot elements for image and headline
        /// </summary>
        private void createSlots()
        {

            imageSlot.Width = imageSlotSize[0];
            imageSlot.Height = imageSlotSize[1];
            imageSlot.Center = new Point(positionBase[xValue] + imageSlotOffset[xValue],
                                         positionBase[yValue] + imageSlotOffset[yValue]);

            imageSlot.Orientation = orientation;

            imageSlot.CanMove = false;
            imageSlot.CanRotate = false;
            imageSlot.CanScale = false;
            //imageSlot.IsEnabled = false;

            imageSlot.Background = new SolidColorBrush(Colors.Transparent);

            BitmapImage img = new BitmapImage();
            //load the image from a local resource
            img.BeginInit();
            img.UriSource = new Uri("pack://application:,,,/Resources/" + "example_transparent.png", UriKind.Absolute);
            img.EndInit();

            imageSlot.Background = new ImageBrush(img);

            imageSlot.ShowsActivationEffects = false;
            RoutedEventHandler loadedEventHandler = null;
            loadedEventHandler = new RoutedEventHandler(delegate
            {
                imageSlot.Loaded -= loadedEventHandler;
                Microsoft.Surface.Presentation.Generic.SurfaceShadowChrome ssc;
                ssc = imageSlot.Template.FindName("shadow", imageSlot) as Microsoft.Surface.Presentation.Generic.SurfaceShadowChrome;
                ssc.Visibility = Visibility.Hidden;
            });
            imageSlot.Loaded += loadedEventHandler;



            textSlot.Width = textSlotSize[0];
            textSlot.Height = textSlotSize[1];
            textSlot.Center = new Point(positionBase[xValue] + textSlotOffset[xValue],
                                        positionBase[yValue] + textSlotOffset[yValue]);

            textSlot.Orientation = orientation;

            textSlot.CanMove = false;
            textSlot.CanRotate = false;
            textSlot.CanScale = false;
            //textSlot.IsEnabled = false;
            textSlot.ShowsActivationEffects = false;
            
       }

        /// <summary>
        /// create area for finished articles
        /// </summary>
        private void createFinishedArticles()
        {

            finishedArticles.Width = finishedArticlesSize[0];
            finishedArticles.Height = finishedArticlesSize[1];
            finishedArticles.Center = new Point(positionBase[xValue] + finishedArticlesOffset[xValue],
                                         positionBase[yValue] + finishedArticlesOffset[yValue]);

            finishedArticles.Orientation = orientation;

            finishedArticles.CanMove = false;
            finishedArticles.CanRotate = false;
            finishedArticles.CanScale = false;
            //finishedArticles.IsEnabled = false;
            finishedArticles.ShowsActivationEffects = false;

        }

        /// <summary>
        /// create element to show current points
        /// </summary>
        private void createPointDisplay()
        {
            
            pointDisplay.Width = pointDisplaySize[0];
            pointDisplay.MinWidth = 5;
            pointDisplay.MinHeight = 5;
            pointDisplay.Height = pointDisplaySize[1];
            pointDisplay.Center = new Point(positionBase[xValue] + pointDisplayOffset[xValue],
                                            positionBase[yValue] + pointDisplayOffset[yValue]);

            pointDisplay.Orientation = orientation;

            pointDisplay.CanMove = false;
            pointDisplay.CanRotate = false;
            pointDisplay.CanScale = false;
            //pointDisplay.IsEnabled = false;
            pointDisplay.ShowsActivationEffects = false;

            pointDisplay.Content = 0;

            //pointDisplay.Background = Brushes.Transparent;
            pointDisplay.Background = color;
            pointDisplay.Foreground = Brushes.White;
            pointDisplay.FontSize = 30;
            pointDisplay.HorizontalContentAlignment = HorizontalAlignment.Center;
            pointDisplay.VerticalContentAlignment = VerticalAlignment.Center;

            pointDisplay.Padding = new Thickness(0);
        }

    }
}
