using System;
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

/*
 * represents an article with image and headline
 */

namespace PhotoPaint
{
    class Article
    {

        public readonly String nameID;

        public readonly ScatterViewItem imageItem;
        public readonly ScatterViewItem textItem;

        public Storyboard imageStoryboard; // current image animation
        public Storyboard textStoryboard; // current text animation

        public int imagePlayer; // which player (1-4) has the image on his island? 0 = none
        public int textPlayer; // which player (1-4) has the headline on his island? 0 = none

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="path">Path to the image, corresponding text file is supposed to have the same name with an txt extension</param>
        public Article(String path)
        {
            
            // ID is full path of the image
            nameID = path;

            // no one owns the new pieces yet
            imagePlayer = 0;
            textPlayer = 0;

            // create image and text object
            imageItem = new ScatterViewItem();
            textItem  = new ScatterViewItem();
            createImage(path);
            createText(path.Substring(0, path.Length - 4) + @".txt");

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
            //  Uncomment if needed...
        //    imageItem.TouchDown += new EventHandler<TouchEventArgs>(onTouch);
    //        imageItem.MouseDown += new MouseButtonEventHandler(onClick);
    //        imageItem.MouseEnter += new MouseEventHandler(onEnter);  
            
            // should abstract both touch and mouse interactions
            imageItem.ContainerActivated += onStartInteraction;
            imageItem.ContainerDeactivated += onStopInteraction;


            
            // event handlers for releasing a piece
    //        imageItem.TouchLeave += new EventHandler<TouchEventArgs>(onTouchLeave);
    //        imageItem.MouseUp += new MouseButtonEventHandler(onClickUp);
    //        imageItem.MouseLeave += new MouseEventHandler(onLeave);

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
            textItem.FontSize = 14;
            textItem.Background = Brushes.White;
            textItem.Padding = new Thickness(8);

            Control.Instance.mainScatterView.Items.Add(textItem);

      //      textItem.TouchDown += new EventHandler<TouchEventArgs>(onTouch);
      //      textItem.MouseDown += new MouseButtonEventHandler(onClick);
      //      textItem.MouseEnter += new MouseEventHandler(onEnter);

            // event handlers for releasing a piece
     //       textItem.TouchLeave += new EventHandler<TouchEventArgs>(onTouchLeave);
    //        textItem.MouseUp += new MouseButtonEventHandler(onClickUp);
        //    textItem.MouseLeave += new MouseEventHandler(onLeave);
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
        /// React to a touch event
        /// </summary>
        public void onTouch(object sender, TouchEventArgs e)
        {
            onSelect((ScatterViewItem)sender);
        }

        /// <summary>
        /// React to a click event (doesn't work)
        /// </summary>
        public void onClick(object sender, MouseButtonEventArgs e)
        {
            onSelect((ScatterViewItem)sender);
        }

        /// <summary>
        /// React to a mouseenter event, 
        /// just a substitute for non-working onClick-Event to test at home
        /// </summary>
        public void onEnter(object sender, MouseEventArgs e)
        {
            onSelect((ScatterViewItem)sender);
        }

        /// <summary>
        /// React to a releasing event (touch)
        /// </summary>
        public void onTouchLeave(object sender, TouchEventArgs e)
        {
            onRelease((ScatterViewItem)sender);
        }

        /// <summary>
        /// React to a releasing event (click) (doesn't work)
        /// </summary>
        public void onClickUp(object sender, MouseButtonEventArgs e)
        {
            onRelease((ScatterViewItem)sender);
        }

        /// <summary>
        /// React to a mouse leaving event, 
        /// just a substitute for non-working onClickUp-Event to test at home
        /// </summary>
        public void onLeave(object sender, MouseEventArgs e)
        {
            onRelease((ScatterViewItem)sender);
        }

        /// <summary>
        /// Selecting a piece
        /// </summary>
        public void onSelect(ScatterViewItem sender)
        {
            stopItem(sender);
        }

        /// <summary>
        /// Releasing a piece
        /// </summary>
        public void onRelease(ScatterViewItem sender)
        {
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
            Point endPoint = new Point(Control.Instance.rnd.Next(1920), Control.Instance.rnd.Next(1080));
            //Point endPoint = new Point(1024 / 2, 768 / 2);
            if (item.ActualCenter.X > -1)
            {
                moveCenter.From = item.ActualCenter;
            }
            else
            {
                moveCenter.From = new Point(1920 / 2, 1080 / 2);
            }
            moveCenter.To = endPoint;
            moveCenter.Duration = new Duration(TimeSpan.FromSeconds(10.0));
            moveCenter.FillBehavior = FillBehavior.Stop;
            stb.Children.Add(moveCenter);
            Storyboard.SetTarget(moveCenter, item);
            Storyboard.SetTargetProperty(moveCenter, new PropertyPath(ScatterViewItem.CenterProperty));
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
            //stb.
            //stb.RepeatBehavior = RepeatBehavior.Forever;
            //stb.Completed += new EventHandler(test);

        }

        //private void test(object sender, EventArgs e)
        //{
        //    MessageBox.Show("Ende");
        //    textItem.Center = new Point(20, 20);
        //}

        /// <summary>
        /// Stop movement of an article piece
        /// </summary>
        /// <param name="item">The piece to stop</param>
        public void stopItem(ScatterViewItem item)
        {
            foreach (Timeline t in imageStoryboard.Children.ToArray()) {
                Debug.WriteLine(t.ToString());
            }
            if (item.Equals(imageItem))
            {
                imageStoryboard.Stop(Control.Instance.window1);
            }
            else if (item.Equals(textItem))
            {
                textStoryboard.Stop(Control.Instance.window1);
            }
        }

    }
}
