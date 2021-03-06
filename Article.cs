﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Surface.Presentation.Controls;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Input;
using System.Windows.Media;
using System.Diagnostics;
using System.Threading;

/*
 * represents an article with image and headline
 */

namespace NeSt
{
    class Article
    {

        public readonly String nameID;

        public readonly ScatterViewItem imageItem;
        public readonly ScatterViewItem textItem;

        public Storyboard imageStoryboard; // current image animation
        public Storyboard textStoryboard; // current text animation

        public Player imageOwner; // current owner of image piece
        public Player textOwner; // current owner of text piece

        private int status; // 0 = pieces can appear
                            // 1 = text appeared
                            // 2 = image appeared
                            // 3 = article is not visible, but still in list of finished articles

        //public int imagePlayer; // which player (1-4) has the image on his island? 0 = none
        //public int textPlayer; // which player (1-4) has the headline on his island? 0 = none

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="path">Path to the image, corresponding text file is supposed to have the same name with an txt extension</param>
        public Article(String path)
        {
            
            // ID is full path of the image
            nameID = path;

            // no one owns the new pieces yet
            imageOwner = null;
            textOwner = null;

            // create image and text object
            imageItem = new BoundingScatterViewItem();
            textItem = new BoundingScatterViewItem();
            createImage(path);
            createText(path.Substring(0, path.Length - 4) + @".txt");

            // article pieces are not visible from the start
            status = 0;
            imageItem.Center = new Point(-1000, -1000);
            imageItem.Visibility = Visibility.Hidden;
            textItem.Center  = new Point(-1000, -1000);
            textItem.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// create an image element
        /// </summary>
        /// <param name="path">Path to the image to create an element from</param>
        private void createImage(String path)
        {
            imageItem.Height = 320 / 2.5;
            imageItem.Width = 427 / 2.5;
            imageItem.MinWidth = imageItem.Width;
            imageItem.MaxWidth = imageItem.Width;

            Image img1 = new Image();
            img1.Source = new BitmapImage(new Uri(path));

            imageItem.Content = img1;

            Control.Instance.mainScatterView.Items.Add(imageItem);


            Debug.WriteLine("adding event handlers");

            // event handlers for taking up a piece
            
            // should abstract both touch and mouse interactions
            imageItem.ContainerActivated += onStartInteraction;
            imageItem.ContainerDeactivated += onStopInteraction;

        }

        /// <summary>
        /// create a text element
        /// </summary>
        /// <param name="path">Path to the text file to read</param>
        private void createText(String path)
        {
            // load text from corresponding text file

            path = path.Substring(0, path.Length - 4) + @".txt";
            string line;
            string text = "";

            if (File.Exists(path))
            {
                StreamReader file = null;
                try
                {
                    file = new StreamReader(path, System.Text.Encoding.UTF8);

                    while ((line = file.ReadLine()) != null)
                    {
                        text += line;
                    }
                }
                catch (Exception e)
                {
                    text = "The file could not be read: " + e.Message;
                }
                finally
                {
                    if (file != null)
                        file.Close();
                }
            }
            else
            {
                text = path + " not found";
            }

            // create text/headline object

            text = AddLineBreaks(text);
            textItem.Content = text;
            textItem.Height = 80;
            textItem.Width = 427 / 2.5;
            textItem.MinWidth = 427 / 2.5;
            textItem.MaxWidth = 427 / 2.5;
            textItem.FontSize = 12;
            textItem.Background = Brushes.White;
            textItem.Padding = new Thickness(4);

            Control.Instance.mainScatterView.Items.Add(textItem);

            textItem.ContainerActivated += onStartInteraction;
            textItem.ContainerDeactivated += onStopInteraction;
        }

        /// <summary>
        /// add breaks to a given text
        /// </summary>
        /// <param name="text">The text to add breaks to</param>
        private string AddLineBreaks(string text)
        {
            string textToReturn = "";
            string[] words = text.Split(' ');
            int lineLength = 0;
            for (int i = 0; i < words.Length; i++)
            {

                if (words[i].Length + lineLength > 22)
                {
                    textToReturn += Environment.NewLine;
                    lineLength = 0;
                }
                textToReturn += words[i] + " ";
                lineLength += words[i].Length + 1;
            }
            return textToReturn;
        }

       
        public void onStartInteraction(object sender, RoutedEventArgs e)
        {
            // just a method to test event handlers firing
            onSelect((ScatterViewItem)sender);
            Debug.WriteLine("interaction happened");
        }

        public void onStopInteraction(object sender, RoutedEventArgs e)
        {
            // just a method to test event handlers firing
            onRelease((ScatterViewItem)sender);
            Debug.WriteLine("interaction happened");
        }

        /// <summary>
        /// Selecting a piece
        /// </summary>
        public void onSelect(ScatterViewItem sender)
        {
            if (sender.Equals(imageItem))
            {
                imageOwner = null;
            }
            else
            {
                textOwner = null;
            }
            stopItem(sender);
        }

        /// <summary>
        /// Releasing a piece
        /// </summary>
        public void onRelease(ScatterViewItem sender)
        {

            double distance = 100; // distance in px to snap into a grid

            double x1 = sender.Center.X;
            double y1 = sender.Center.Y;
            double x2, y2;

            // snap image pieces
            if (sender.Equals(imageItem))
            {
                
                // check for each island
                for (int i = 0; i < Control.Instance.playerList.players.Count(); i++)
                {
                    if (Control.Instance.playerList.players[i].island.imageSlot.IsVisible == false) { break; }

                    x2 = Control.Instance.playerList.players[i].island.imageSlot.Center.X;
                    y2 = Control.Instance.playerList.players[i].island.imageSlot.Center.Y;

                    if (Math.Pow(Math.Abs(x1 - x2), 2) + Math.Pow(Math.Abs(y1 - y2), 2) < Math.Pow(distance, 2))
                    {

                        bool cancel = false;
                        foreach (Article article in Control.Instance.articleList.articles)
                        {
                            if (article.imageOwner == Control.Instance.playerList.players[i] && (article.status == 1 || article.status == 2))
                            {
                                cancel = true;
                                break;
                            }
                        }
                        if (cancel)
                        {
                            break;
                        }

                        sender.Center = Control.Instance.playerList.players[i].island.imageSlot.Center;
                        sender.Orientation = Control.Instance.playerList.players[i].island.orientation;
                        imageOwner = Control.Instance.playerList.players[i];
                        checkFitting();
                        return;
                    }
                }
            }
            // snap text pieces
            else
            {
                // check for each island
                for (int i = 0; i < Control.Instance.playerList.players.Count(); i++)
                {
                    
                    if (Control.Instance.playerList.players[i].island.imageSlot.IsVisible == false) { break; }

                    x2 = Control.Instance.playerList.players[i].island.textSlot.Center.X;
                    y2 = Control.Instance.playerList.players[i].island.textSlot.Center.Y;

                    if (Math.Pow(Math.Abs(x1 - x2), 2) + Math.Pow(Math.Abs(y1 - y2), 2) < Math.Pow(distance, 2))
                    {

                        bool cancel = false;
                        foreach (Article article in Control.Instance.articleList.articles)
                        {
                            if (article.textOwner == Control.Instance.playerList.players[i] && (article.status == 1 || article.status == 2))
                            {
                                cancel = true;
                                break;
                            }
                        }
                        if (cancel)
                        {
                            break;
                        }

                        sender.Center = Control.Instance.playerList.players[i].island.textSlot.Center;
                        sender.Orientation = Control.Instance.playerList.players[i].island.orientation;
                        textOwner = Control.Instance.playerList.players[i];
                        checkFitting();
                        return;
                    }
                }
            }

            // move item again if it didn't snap
            moveItem(sender);

        }

        /// <summary>
        /// Automatically move an article piece
        /// </summary>
        /// <param name="item">The piece to move</param>
        public void moveItem(ScatterViewItem item)
        {

            Storyboard stb = new Storyboard();
            PointAnimation moveCenter = new PointAnimation();

            // find a point to move to without crossing an island
            
            int[] islandSize = new int[] { 561, 295 };
            //Point endPoint = new Point(Control.Instance.rnd.Next(1920), Control.Instance.rnd.Next(1080));

            List<Point> possiblePoints = new List<Point>();
            if (Control.Instance.players3and4)
            {
                possiblePoints.Add(new Point(Control.Instance.rnd.Next(islandSize[1] + 200, 1920 - 200 - islandSize[1]),
                                             Control.Instance.rnd.Next(islandSize[1] + 200, 1080 - 200 - islandSize[1])));
            }
            else
            {
                possiblePoints.Add(new Point(Control.Instance.rnd.Next(islandSize[1] + 200, 1920 - 200 - islandSize[1]),
                                             Control.Instance.rnd.Next(0, 1080)));
            }
            possiblePoints.Add(new Point(Control.Instance.rnd.Next(0, islandSize[1] - 200), Control.Instance.rnd.Next(0, islandSize[1] - 200)));
            possiblePoints.Add(new Point(Control.Instance.rnd.Next(0, islandSize[1] - 200), Control.Instance.rnd.Next(1080 - islandSize[1] + 200, 1080)));
            possiblePoints.Add(new Point(Control.Instance.rnd.Next(1920 - islandSize[1] + 200, 1920), Control.Instance.rnd.Next(0, islandSize[1] - 200)));
            possiblePoints.Add(new Point(Control.Instance.rnd.Next(1920 - islandSize[1] + 200, 1920), Control.Instance.rnd.Next(1080 - islandSize[1] + 200, 1080)));

            Point endPoint;
            if (item.Center.X < islandSize[1] || item.Center.X > 1920 - islandSize[1] || item.Center.Y < islandSize[1] || item.Center.Y > 1080 - islandSize[1])
            {
                endPoint = possiblePoints[0];
            }
            else
            {
                endPoint = possiblePoints[Control.Instance.rnd.Next(4)];
            }

            //Point endPoint = new Point(1024 / 2, 768 / 2);
            if (item.ActualCenter.X > -1)
            {
                moveCenter.From = item.ActualCenter;
            }
            else
            {
                moveCenter.From = new Point(1920 / 2, 1080 / 2);
            }

            //Rotation Animation
            DoubleAnimation turnAnimation = new DoubleAnimation();
            if (item.Orientation >= 0)
            {
                turnAnimation.From = item.Orientation;
            }
            else
            {
                turnAnimation.From = item.Orientation = 0;
            }
            //rotates to a rnd orientation between 0 and 360
            turnAnimation.To = Control.Instance.rnd.NextDouble() * 360;
            int timespan = Control.Instance.rnd.Next(10, 12);
            turnAnimation.Duration = new Duration(TimeSpan.FromSeconds(timespan)); //same duration as movement
            stb.Children.Add(turnAnimation);
            Storyboard.SetTarget(turnAnimation, item);
            Storyboard.SetTargetProperty(turnAnimation, new PropertyPath(ScatterViewItem.OrientationProperty));
             
            moveCenter.To = endPoint;
            moveCenter.Duration = new Duration(TimeSpan.FromSeconds(timespan));
            moveCenter.FillBehavior = FillBehavior.Stop;
            stb.Children.Add(moveCenter);
            Storyboard.SetTarget(moveCenter, item);
            Storyboard.SetTargetProperty(moveCenter, new PropertyPath(ScatterViewItem.CenterProperty));
            stb.Completed += new EventHandler((sender, e) => onAnimationEnd(sender, e, item));
            stb.Begin(Control.Instance.window1, true);
            //item.Center = endPoint;

            if (item.Equals(imageItem))
            {
                imageStoryboard = stb;
            }
            else
            {
                textStoryboard = stb;
            }

        }

        /// <summary>
        /// reaction at the end of an animation
        /// </summary>
        private void onAnimationEnd(object sender, EventArgs e, ScatterViewItem item)
        {
            // start new animation
            moveItem(item);
        }

        /// <summary>
        /// Stop movement of an article piece
        /// </summary>
        /// <param name="item">The piece to stop</param>
        public void stopItem(ScatterViewItem item)
        {
            item.Center = item.ActualCenter;
            item.Orientation = item.ActualOrientation;
            //foreach (Timeline t in imageStoryboard.Children.ToArray()) {
            //    Debug.WriteLine(t.ToString());
            //}
            if (item.Equals(imageItem))
            {
                imageStoryboard.Stop(Control.Instance.window1);
            }
            else if (item.Equals(textItem))
            {
                textStoryboard.Stop(Control.Instance.window1);
            }
        }

        /// <summary>
        /// checks if both pieces are currently on the same island and reacts to it
        /// </summary>
        private void checkFitting()
        {
            if (imageOwner != null && textOwner != null && imageOwner.Equals(textOwner))
            {
                imageOwner.setPoints(imageOwner.getPoints() + 1);
                setStatus(3);
                Control.Instance.finishedArticles.add(this);
                Control.Instance.articleList.showNext();
            }
        }

        /// <summary>
        /// get current status of this article (see definition of variable 'status')
        /// </summary>
        public int getStatus()
        {
            return status;
        }

        /// <summary>
        /// set status of this article (see definition of variable 'status')
        /// </summary>
        /// <param name="item">The new status for this article</param>
        public void setStatus(int statusNew)
        {
            switch (statusNew)
            {
                case 0:
                    textOwner = null;
                    imageOwner = null;
                    break;
                case 1:
                    textItem.Center = new Point(1920 / 2, 1080 / 2);
                    textItem.Visibility = Visibility.Visible;
                    blend(textItem, false);
                    moveItem(textItem);
                    break;
                case 2:
                    imageItem.Center = new Point(1920 / 2, 1080 / 2);
                    imageItem.Visibility = Visibility.Visible;
                    blend(imageItem, false);
                    moveItem(imageItem);
                    break;
                case 3:
                    //shrink(imageItem);
                    blend(imageItem, true);
                    moveTo(imageItem, imageOwner.island.finishedArticles.Center);
                    //shrink(textItem);
                    blend(textItem, true);
                    moveTo(textItem, imageOwner.island.finishedArticles.Center);
                    break;
            }
            status = statusNew;
        }

        /// <summary>
        /// blend in/out an item
        /// </summary>
        /// <parameter name="item">The item to blend in/out</parameter>
        /// <parameter name="blendIn">If the item will be blended in or out</parameter>
        private void blend(ScatterViewItem item, bool blendIn)
        {
            DoubleAnimation myDoubleAnimation = new DoubleAnimation();
            if (blendIn)
            {
                myDoubleAnimation.From = 1.0;
                myDoubleAnimation.To = 0.0;
                myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.25));
            }
            else
            {
                myDoubleAnimation.From = 0.0;
                myDoubleAnimation.To = 1.0;
                myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(1.0));
            }
            myDoubleAnimation.FillBehavior = FillBehavior.Stop;
            Storyboard myStoryboard;
            myStoryboard = new Storyboard();
            myStoryboard.Children.Add(myDoubleAnimation);
            Storyboard.SetTarget(myDoubleAnimation, item);
            Storyboard.SetTargetProperty(myDoubleAnimation, new PropertyPath(ScatterViewItem.OpacityProperty));
            myStoryboard.Begin(Control.Instance.window1, true);

            if (blendIn)
            {
                item.Opacity = 0.0;
            }
            else
            {
                item.Opacity = 1.0;
            }
        }

        /// <summary>
        /// shrink an item
        /// </summary>
        /// <parameter name="item">The item to shrink</parameter>
        private void shrink(ScatterViewItem item) // does not work
        {
            DoubleAnimation myDoubleAnimation = new DoubleAnimation();
            //myDoubleAnimation.From = item.ActualWidth;
            myDoubleAnimation.To = 1;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            Storyboard myStoryboard;
            myStoryboard = new Storyboard();
            myStoryboard.Children.Add(myDoubleAnimation);
            Storyboard.SetTarget(myDoubleAnimation, item);
            Storyboard.SetTargetProperty(myDoubleAnimation, new PropertyPath(ScaleTransform.ScaleXProperty));
            myStoryboard.Begin();
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
            stb.Completed += new EventHandler((sender, e) => resetPieces(sender, e, item));
            stb.Begin(Control.Instance.window1, true);
            item.Center = endPoint;
        }

        /// <summary>
        /// reset a piece after it's animation
        /// </summary>
        private void resetPieces(object sender, EventArgs e, ScatterViewItem item)
        {
            item.Center = new Point(-1000, -1000);
            item.Opacity = 1;
        }

    }
    class BoundingScatterViewItem : ScatterViewItem
    {
        public BoundingScatterViewItem()
        {
            //this.IsManipulationEnabled = false;
            this.AddHandler(UIElement.ManipulationDeltaEvent, new EventHandler<ManipulationDeltaEventArgs>(BoundingScatterViewItem_ManipulationDelta), true);

        }

        void BoundingScatterViewItem_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            //remove large inertia values
            if (e.IsInertial)
            {
                if (e.CumulativeManipulation.Translation.X > 5 ||
                    e.CumulativeManipulation.Translation.Y > 5)
                {
                    e.Complete();
                }
            }
        }
    }

}
