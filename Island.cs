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
using System.Windows.Media.Animation;


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

        public List<ScatterViewItem> listImages; // images of finished articles
        public List<ScatterViewItem> listTexts; // texts of finished articles

        private String path;
        public int playerNumber;

        private Brush color;

        //private int posX, imageX, textX, y;

        //private int[] islandSize           = new int[] { 450, 250 };
        public int[] islandSize            = new int[] { 561, 295 };
        private int[] textSlotSize         = new int[] { 170,  80 };
        private int[] imageSlotSize        = new int[] { 170, 128 };
        private int[] finishedArticlesSize = new int[] { 234, 224 };
        //private int[] pointDisplaySize     = new int[] {  50,  50 };
        private int[] pointDisplaySize     = new int[] { 59,   58 };

        private int[] textSlotOffset         = new int[] {-125,   50 };
        private int[] imageSlotOffset        = new int[] {-125,  -70 };
        private int[] finishedArticlesOffset = new int[] {  92,   22 };
        private int[] pointDisplayOffset     = new int[] {   0,  130 };

        private int[] tableResolution = new int[] { 1920, 1080 };

        private int[] positionBase;

        private int xValue; // parameter index of coordinates that defines horizontal values
        private int yValue; // parameter index of coordinates that defines vertical values

        public int orientation;

        private int articlesToShow = 3; // how many articles should be shown in the list of the last finished articles
        private int nextIndexToChange = -1; // next index of article elements that gets rewritten

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

            createArticles();

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
            img.UriSource = new Uri("pack://application:,,,/Resources/" + "island_sand2.png", UriKind.Absolute);
            img.EndInit();

            island.Background = new ImageBrush(img);

            island.ShowsActivationEffects = false;

            // make background transparent without shadows
            island.Loaded += makeTransparent;

            //RoutedEventHandler loadedEventHandler = null;
            //loadedEventHandler = new RoutedEventHandler(delegate
            //{
            //    island.Loaded -= loadedEventHandler;
            //    Microsoft.Surface.Presentation.Generic.SurfaceShadowChrome ssc;
            //    ssc = island.Template.FindName("shadow", island) as Microsoft.Surface.Presentation.Generic.SurfaceShadowChrome;
            //    ssc.Visibility = Visibility.Hidden;
            //});
            //island.Loaded += loadedEventHandler;


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
            imageSlot.ShowsActivationEffects = false;
            imageSlot.Loaded += makeTransparent;

            imageSlot.Background = new SolidColorBrush(Colors.Transparent);

            BitmapImage img = new BitmapImage();
            //load the image from a local resource
            img.BeginInit();
            img.UriSource = new Uri("pack://application:,,,/Resources/" + "imageSlot.png", UriKind.Absolute);
            img.EndInit();

            imageSlot.Background = new ImageBrush(img);

            imageSlot.Content = "Bild hier ablegen";
            imageSlot.VerticalContentAlignment = VerticalAlignment.Center;
            imageSlot.HorizontalContentAlignment = HorizontalAlignment.Center;
            imageSlot.FontFamily = new FontFamily("Poiret One");
            imageSlot.FontSize = 14;
            //imageSlot.Foreground = Brushes.DarkGoldenrod;

            //imageSlot.ShowsActivationEffects = false;
            //RoutedEventHandler loadedEventHandler = null;
            //loadedEventHandler = new RoutedEventHandler(delegate
            //{
            //    imageSlot.Loaded -= loadedEventHandler;
            //    Microsoft.Surface.Presentation.Generic.SurfaceShadowChrome ssc;
            //    ssc = imageSlot.Template.FindName("shadow", imageSlot) as Microsoft.Surface.Presentation.Generic.SurfaceShadowChrome;
            //    ssc.Visibility = Visibility.Hidden;
            //});
            //imageSlot.Loaded += loadedEventHandler;



            textSlot.Width = textSlotSize[0];
            textSlot.Height = textSlotSize[1];
            textSlot.Center = new Point(positionBase[xValue] + textSlotOffset[xValue],
                                        positionBase[yValue] + textSlotOffset[yValue]);

            textSlot.Orientation = orientation;

            textSlot.Background = Brushes.Transparent;
            textSlot.Loaded += makeTransparent;

            textSlot.CanMove = false;
            textSlot.CanRotate = false;
            textSlot.CanScale = false;
            //textSlot.IsEnabled = false;
            textSlot.ShowsActivationEffects = false;

            textSlot.Background = new SolidColorBrush(Colors.Transparent);

            img = new BitmapImage();
            //load the image from a local resource
            img.BeginInit();
            img.UriSource = new Uri("pack://application:,,,/Resources/" + "textSlot.png", UriKind.Absolute);
            img.EndInit();

            textSlot.Background = new ImageBrush(img);

            textSlot.Content = "Headline hier ablegen";
            textSlot.VerticalContentAlignment = VerticalAlignment.Center;
            textSlot.HorizontalContentAlignment = HorizontalAlignment.Center;
            textSlot.FontFamily = new FontFamily("Poiret One");
            textSlot.FontSize = 14;
            //textSlot.Foreground = Brushes.DarkGoldenrod;
            
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

            finishedArticles.Background = Brushes.Transparent;
            //finishedArticles.Visibility = Visibility.Hidden; // object not really needed anymore
            finishedArticles.FontFamily = new FontFamily("Poiret One");
            finishedArticles.FontSize = 30;
            finishedArticles.Padding = new Thickness(10);
            finishedArticles.HorizontalContentAlignment = HorizontalAlignment.Center;

            //finishedArticles.Content = "NEWS STREAM";

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

            pointDisplay.Background = Brushes.Transparent;
            pointDisplay.Loaded += makeTransparent;
            //pointDisplay.Background = color;



            pointDisplay.Background = new SolidColorBrush(Colors.Transparent);

            BitmapImage img = new BitmapImage();
            //load the image from a local resource
            img.BeginInit();
            string uri = "pack://application:,,,/Resources/" + "player_red.png";
            if (color == Brushes.DarkBlue) uri = "pack://application:,,,/Resources/" + "player_blue.png"; else
            if (color == Brushes.DarkGreen) uri = "pack://application:,,,/Resources/" + "player_green.png"; else
            if (color == Brushes.DarkMagenta) uri = "pack://application:,,,/Resources/" + "player_magenta.png";
            img.UriSource = new Uri(uri, UriKind.Absolute);
            img.EndInit();

            pointDisplay.Background = new ImageBrush(img);



            pointDisplay.Foreground = Brushes.White;
            pointDisplay.FontSize = 30;
            pointDisplay.HorizontalContentAlignment = HorizontalAlignment.Center;
            pointDisplay.VerticalContentAlignment = VerticalAlignment.Center;

            pointDisplay.Padding = new Thickness(0);
        }

        /// <summary>
        /// create article templates for the list
        /// </summary>
        private void createArticles()
        {

            double imageHeightFactor = 0.8; // how big is an image in a line? 1 = 100%;
            int[] imageSize = new int[2];
            imageSize[1] = (int)(finishedArticlesSize[1] / articlesToShow * imageHeightFactor);
            imageSize[0] = (int)(imageSize[1] * (4f / 3));

            int[] articleSize = new int[] { finishedArticlesSize[0] - imageSize[0],
                                            finishedArticlesSize[1] / articlesToShow};

            listImages = new List<ScatterViewItem>();
            listTexts = new List<ScatterViewItem>();

            ScatterViewItem image;
            ScatterViewItem text;

            int[] startPositionOffset = new int[2]; // offset of first article

            for (int i = 0; i < articlesToShow; i++)
            {
                image = new ScatterViewItem();
                text = new ScatterViewItem();
                listImages.Add(image);
                listTexts.Add(text);

                // formatting text

                text.Width = articleSize[0];
                text.MinWidth = articleSize[0];
                text.Height = articleSize[1];
                text.MinHeight = articleSize[1];

                startPositionOffset[xValue] = 0;
                startPositionOffset[yValue] = articlesToShow / 2 * articleSize[1] * -1;

                int direction = 1;
                if (orientation == 0 || orientation == 270)
                {
                    startPositionOffset[yValue] *= -1;
                }
                else
                {
                    direction *= -1;
                }
                int imageIndentDirection = 1;
                if (orientation == 180 || orientation == 270)
                {
                    imageIndentDirection *= -1;
                }

                if (xValue == 0)
                {
                    text.Center = new Point(positionBase[xValue] + finishedArticlesOffset[xValue] - startPositionOffset[0] + (imageSize[0] / 2) * imageIndentDirection,
                                            positionBase[yValue] + finishedArticlesOffset[yValue] - startPositionOffset[1] + i * articleSize[1] * direction);
                }
                else
                {
                    text.Center = new Point(positionBase[xValue] + finishedArticlesOffset[xValue] - startPositionOffset[0] + i * articleSize[1] * direction,
                                            positionBase[yValue] + finishedArticlesOffset[yValue] - startPositionOffset[1] + (imageSize[0] / 2) * imageIndentDirection);
                }

                text.Orientation = orientation;

                text.CanMove = false;
                text.CanRotate = false;
                text.CanScale = false;
                text.ShowsActivationEffects = false;

                text.Background = Brushes.Transparent;
                text.Loaded += makeTransparent;

                text.Foreground = Brushes.White;
                text.FontSize = 10;
                text.HorizontalContentAlignment = HorizontalAlignment.Left;
                text.VerticalContentAlignment = VerticalAlignment.Center;

                text.Padding = new Thickness(6);

                text.ZIndex = 3;

                Control.Instance.mainScatterView.Items.Add(text);

                // formatting image

                image.Width = imageSize[0];
                image.MinWidth = imageSize[0];
                //image.Height = imageSize[1];
                //image.MinHeight = imageSize[1];
                image.Height = articleSize[1];
                image.MinHeight = articleSize[1];

                if (xValue == 0)
                {
                    image.Center = new Point(text.Center.X - (text.Width / 2 + image.Width / 2) * imageIndentDirection,
                                             text.Center.Y);
                }
                else
                {
                    image.Center = new Point(text.Center.X,
                                             text.Center.Y - (text.Width / 2 + image.Width / 2) * imageIndentDirection);
                }

                image.Orientation = orientation;

                image.CanMove = false;
                image.CanRotate = false;
                image.CanScale = false;
                image.ShowsActivationEffects = false;

                image.Background = Brushes.Transparent;
                image.Loaded += makeTransparent;

                image.Padding = new Thickness(6);

                image.ZIndex = 3;

                Control.Instance.mainScatterView.Items.Add(image);

            }

        }

        /// <summary>
        /// update list of finished articles
        /// </summary>
        public void updateArticles()
        {

            List<Article> finishedArticles = Control.Instance.finishedArticles.getList();

            Point positionCacheImage;
            Point positionCacheText;

            positionCacheImage = new Point(listImages[0].Center.X, listImages[0].Center.Y);
            positionCacheText = new Point(listTexts[0].Center.X, listTexts[0].Center.Y);

            for (int i = 0; i < listImages.Count() - 1; i++ )
            {

                if (i == nextIndexToChange)
                {
                    // hide part that gets changed
                    listImages[i].Opacity = 0;
                    listTexts[i].Opacity = 0;
                    // change position
                    listImages[i].Center = listImages[i + 1].ActualCenter;
                    listTexts[i].Center = listTexts[i + 1].ActualCenter;
                }
                else
                {
                    // move articles
                    moveTo(listImages[i], listImages[i + 1].Center);
                    moveTo(listTexts[i], listTexts[i + 1].Center);
                }

                //listImages[i].Center = new Point(listImages[i + 1].Center.X, listImages[i + 1].Center.Y);
                //listTexts[i].Center = new Point(listTexts[i + 1].Center.X, listTexts[i + 1].Center.Y);
            }

            if (listTexts.Count() - 1 == nextIndexToChange)
            {
                listImages[listTexts.Count() - 1].Center = positionCacheImage;
                listTexts[listTexts.Count() - 1].Center = positionCacheText;
            }
            else
            {
                moveTo(listImages[listTexts.Count() - 1], positionCacheImage);
                moveTo(listTexts[listTexts.Count() - 1], positionCacheText);
            }

            //listImages[listTexts.Count() - 1].Center = positionCacheImage;
            //listTexts[listTexts.Count() - 1].Center = positionCacheText;

            if ( nextIndexToChange == -1)
                nextIndexToChange = listTexts.Count() - 1; // quick and dirty bugfix

            // load data of new article

            Image img1 = new Image();
            listImages[nextIndexToChange].Content = img1;
            ((Image)listImages[nextIndexToChange].Content).Source = new BitmapImage(new Uri(finishedArticles[finishedArticles.Count() - 1].nameID));
            listImages[nextIndexToChange].Background = finishedArticles[finishedArticles.Count() - 1].imageOwner.island.color;

            listTexts[nextIndexToChange].Content = finishedArticles[finishedArticles.Count() - 1].textItem.Content;
            listTexts[nextIndexToChange].Background = finishedArticles[finishedArticles.Count() - 1].imageOwner.island.color;

            // show new article

            blend(listImages[nextIndexToChange]);
            blend(listTexts[nextIndexToChange]);

            //DoubleAnimation myDoubleAnimation = new DoubleAnimation();
            //myDoubleAnimation.From = 0.0;
            //myDoubleAnimation.To = 1.0;
            //myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
            //Storyboard myStoryboard;
            //myStoryboard = new Storyboard();
            //myStoryboard.Children.Add(myDoubleAnimation);
            //Storyboard.SetTarget(myDoubleAnimation, listImages[nextIndexToChange]);
            //Storyboard.SetTargetProperty(myDoubleAnimation, new PropertyPath(ScatterViewItem.OpacityProperty));
            //myStoryboard.Begin();

            // set next piece to change
            if (nextIndexToChange <= 0)
                nextIndexToChange = listTexts.Count() - 1;
            else
                nextIndexToChange -= 1;

        }

        /// <summary>
        /// move item to a given point
        /// </summary>
        /// <parameter name="item">The item to move</parameter>
        /// <parameter name="point">The point to move to</parameter>
        private void moveTo(ScatterViewItem item, Point point)
        {
            Storyboard stb = new Storyboard();
            PointAnimation moveCenter = new PointAnimation();
            Point endPoint = point;
            moveCenter.To = endPoint;
            moveCenter.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            moveCenter.FillBehavior = FillBehavior.Stop;
            stb.Children.Add(moveCenter);
            Storyboard.SetTarget(moveCenter, item);
            Storyboard.SetTargetProperty(moveCenter, new PropertyPath(ScatterViewItem.CenterProperty));
            //stb.Completed += new EventHandler((sender, e) => onAnimationEnd(sender, e, item));
            stb.Begin(Control.Instance.window1, true);
            item.Center = endPoint;
        }

        /// <summary>
        /// blend in an item
        /// </summary>
        /// <parameter name="item">The item to blend in</parameter>
        private void blend(ScatterViewItem item)
        {
            DoubleAnimation myDoubleAnimation = new DoubleAnimation();
            myDoubleAnimation.From = 0.0;
            myDoubleAnimation.To = 1.0;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
            Storyboard myStoryboard;
            myStoryboard = new Storyboard();
            myStoryboard.Children.Add(myDoubleAnimation);
            Storyboard.SetTarget(myDoubleAnimation, item);
            Storyboard.SetTargetProperty(myDoubleAnimation, new PropertyPath(ScatterViewItem.OpacityProperty));
            myStoryboard.Begin();
        }

        /// <summary>
        /// make an object's background transparent including removing the shadow
        /// </summary>
        private void makeTransparent(object sender, EventArgs e)
        {
            ((ScatterViewItem)sender).Loaded -= makeTransparent;
            Microsoft.Surface.Presentation.Generic.SurfaceShadowChrome ssc;
            ssc = ((ScatterViewItem)sender).Template.FindName("shadow", ((ScatterViewItem)sender)) as Microsoft.Surface.Presentation.Generic.SurfaceShadowChrome;
            ssc.Visibility = Visibility.Hidden;
        }

    }
}
