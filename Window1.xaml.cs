using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

using Microsoft.Surface;
using Microsoft.Surface.Presentation;
using Microsoft.Surface.Presentation.Controls;
using Microsoft.Surface.Presentation.Input;
using System.Linq;

using System.Collections;

using System.Windows.Media.Animation;

namespace PhotoPaint
{
    /// <summary>
    /// Interaction logic for Window1.xaml.
    /// </summary>
    public partial class Window1 : SurfaceWindow
    {


        /// <summary>
        /// A mapping of each stroke to list of point timings in milliseconds.
        /// </summary>
        private readonly Dictionary<Stroke, List<long>> recordedStrokes = new Dictionary<Stroke, List<long>>();

        /// <summary>
        /// Keeps track of strokes as they are played back.
        /// Maps the new stroke (copy) being constructed during playback to the original stroke.
        /// </summary>
        private readonly Dictionary<Stroke, Stroke> newStrokes = new Dictionary<Stroke, Stroke>();

        private Storyboard stb;

        /// <summary>
        /// Duration of playback loop (in seconds) when no video is present.
        /// </summary>
        private const int duration = 12;
        private MediaElement video;
        public event RoutedEventHandler mMediaEnded;

        /// <summary>
        /// Key used to track strokes in a touch device's user data.
        /// </summary>
        private readonly object strokeKey = new object();

        /// <summary>
        /// Brushes used for movie buttons
        /// </summary>
        private static SolidColorBrush buttonHighlightBrush = new SolidColorBrush (Color.FromArgb(0xCC, 0x99, 0x99, 0x99));
        private static SolidColorBrush buttonBackgroundBrush = new SolidColorBrush (Color.FromArgb(0xCC, 0xCC, 0xCC, 0xCC));

        private List<ScatterViewItem> allItems = new List<ScatterViewItem>();
        

         #region Initalization

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Window1()
        {
            InitializeComponent();

            // Add handlers for window availability events
            AddWindowAvailabilityHandlers();

            // listen for changes to the primary InteractiveSurfaceDevice.
            InteractiveSurface.PrimarySurfaceDevice.Changed += OnPrimarySurfaceDeviceChanged;
        }

        /// <summary>
        /// Occurs when the window is about to close.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            
            // Remove handlers for window availability events
            RemoveWindowAvailabilityHandlers();
            // Stop listening for InteractiveSurfaceDevice changes when the window closes.
            InteractiveSurface.PrimarySurfaceDevice.Changed -= OnPrimarySurfaceDeviceChanged;
        }

        /// <summary>
        /// Update the size of photo
        /// </summary>
        private void OnPrimarySurfaceDeviceChanged(object sender, DeviceChangedEventArgs e)
        {
            //UpdatePhotoSize();
        }

        /// <summary>
        /// Called when the application is finished initalizing
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            // Load images and video from the public folders.
            // These are default OS folders that will always be in these locations
            // unless the user has deliberately moved them.
            string publicFoldersPath = Environment.GetEnvironmentVariable("public");

            string path = publicFoldersPath + @"\Pictures\Sample Pictures";
            LoadAllImgFrom(path);
            LoadAllTextFrom(path);

           
            string targetVideoPath = publicFoldersPath + @"\Videos\Sample Videos";
            
          //      video = new MediaElement();
                mBackground.BeginInit();
                mBackground.LoadedBehavior = mBackground.UnloadedBehavior = MediaState.Manual;
             //   mBackground.MediaEnded += mMediaEnded; 
                mBackground.Source = new Uri(targetVideoPath + @"\video.mp4");

                mBackground.EndInit();
                mBackground.Position = TimeSpan.Zero;
                mBackground.Play();
                
            
                VideoDrawing vd = new VideoDrawing();
                VisualBrush vb = new VisualBrush();
                

                vb.Visual = mBackground;
                vb.Stretch = Stretch.Fill;
                vb.TileMode = TileMode.None;
                
                mWindow.Background = vb;
          //      MainScatterView.Items.Add(video);

         //   }

         //   Movie.Play();  

        }



        private void CreateTextItem(string text)
        {
            ScatterViewItem textItem = new ScatterViewItem();
            // Set the content of the label.
            text = AddLineBreaks(text);
            //text = text.Replace("]", Environment.NewLine);
            textItem.Content = text;
            //TODO: max größe
            textItem.Height = 80;
            textItem.Width = 427 / 2.5;
            textItem.MinWidth = 427 / 2.5;
            textItem.MaxWidth = 427 / 2.5;
            textItem.FontSize = 14;
            textItem.Padding = new Thickness(8);
            
            // Add the label to the ScatterView control.
            // It is automatically wrapped in a ScatterViewItem control.
            allItems.Add(textItem);
            MainScatterView.Items.Add(textItem);

        }

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

        private void LoadAllTextFrom(string textDirectoryPath)
        {
            string path = textDirectoryPath + @"\TextFile.txt";
            string line; 
 
            if (File.Exists(path))
            {
                StreamReader file = null;
                try
                {
                    file = new StreamReader(path, System.Text.Encoding.UTF8);
                   
                    while ((line = file.ReadLine()) != null)
                    {
                        CreateTextItem(line);
                    }
                }
                catch (Exception e)
                {
                    CreateTextItem("The file could not be read:" + (e.Message));
                }
                finally
                {
                    if (file != null)
                        file.Close();
                }
            } 
        }
      
        private void LoadImg(string path)
        {

            ScatterViewItem photoPad = new ScatterViewItem();
            //photoPad.Name="a";
            photoPad.Height = 320/2.5;
            photoPad.Width=427/2.5;
            photoPad.MinWidth = photoPad.Width;
            photoPad.MaxWidth = photoPad.Width;


            Image img1 = new Image();
            img1.Source = new BitmapImage(new Uri(path));
            
            photoPad.Content = img1;

            allItems.Add(photoPad);

            MainScatterView.Items.Add(photoPad);
            
        }

        private void LoadAllImgFrom(string imgDirectoryPath)
        {
            if (Directory.Exists(imgDirectoryPath))
            {
                var filteredFiles = Directory
                .GetFiles(imgDirectoryPath, "*.*")
                .Where(file => file.ToLower().EndsWith("png") || file.ToLower().EndsWith("jpg"))
                .ToList();

                string[] files = Directory.GetFiles(imgDirectoryPath,  "*.jpg");

                for (int i = 0; i < filteredFiles.Count; i++) 
                {
                    LoadImg(filteredFiles[i]);
                }
            }
        }

        #endregion Initalization

        private void MoveItem(ScatterViewItem item)
        {
            stb = new Storyboard();
            PointAnimation moveCenter = new PointAnimation();
            Point endPoint = new Point(1024 / 2, 768 / 2);
            moveCenter.From = item.ActualCenter;
            moveCenter.To = endPoint;
            moveCenter.Duration = new Duration(TimeSpan.FromSeconds(10.0));
            moveCenter.FillBehavior = FillBehavior.Stop;
            stb.Children.Add(moveCenter);
            
            Storyboard.SetTarget(moveCenter, item);
            Storyboard.SetTargetProperty(moveCenter, new PropertyPath(ScatterViewItem.CenterProperty));
           
            stb.Completed += addNewEndPoint;
     //       moveCenter.RemoveRequested += addNewEndPoint;
     //       moveCenter.Completed += addNewEndPoint;

            stb.Begin(this);
           

        }

        private void addNewEndPoint(object sender, EventArgs e)
        {
            Debug.Print("bla");
            Random rnd = new Random();
            // TODO: add a new endpoint here
            PointAnimation moveCenter = new PointAnimation();
            Point endPoint = new Point(rnd.Next(1, 1000), rnd.Next(1, 500));
    //        moveCenter.From = item.ActualCenter;
            moveCenter.To = endPoint;
            moveCenter.Duration = new Duration(TimeSpan.FromSeconds(10.0));
            moveCenter.FillBehavior = FillBehavior.Stop;
            stb.Children.Add(moveCenter);

           // Storyboard.SetTarget(moveCenter, item);
            Storyboard.SetTargetProperty(moveCenter, new PropertyPath(ScatterViewItem.CenterProperty));

            stb.Completed += addNewEndPoint;


        }



        /// <summary>
        /// Loads an image.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static Image LoadImageFromPath(string path)
        {
            ImageSourceConverter converter = new ImageSourceConverter();
            Image image = new Image();
            image.Source = (ImageSource)converter.ConvertFromString(path);
            return image;
        }

        private void onDebugButtonClick(Object sender,
    RoutedEventArgs e)
        {
            foreach (ScatterViewItem item in allItems)
            {
                MoveItem(item);
            }
        }

        /// <summary>
        /// Adds handlers for window availability events.
        /// </summary>
        private void AddWindowAvailabilityHandlers()
        {
            // Subscribe to surface window availability events
            ApplicationServices.WindowInteractive += OnWindowInteractive;
            ApplicationServices.WindowNoninteractive += OnWindowNoninteractive;
            ApplicationServices.WindowUnavailable += OnWindowUnavailable;

        }

        /// <summary>
        /// Removes handlers for window availability events.
        /// </summary>
        private void RemoveWindowAvailabilityHandlers()
        {
            // Unsubscribe from surface window availability events
            ApplicationServices.WindowInteractive -= OnWindowInteractive;
            ApplicationServices.WindowNoninteractive -= OnWindowNoninteractive;
            ApplicationServices.WindowUnavailable -= OnWindowUnavailable;
        }

        /// <summary>
        /// This is called when the user can interact with the application's window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
       private void OnWindowInteractive(object sender, EventArgs e)
        {
            // Enable audio for our movie.
            //Movie.IsMuted = false;
           
            foreach (ScatterViewItem item in allItems)
            {
                MoveItem(item);
            }
        }


       /// <summary>
       /// Handles the MediaEnded event for the MovieCanvas video.
       /// </summary>
       /// <param name="sender">The MediaElement that raised the event.</param>
       /// <param name="args">The arguments for the event.</param>
       private void onMediaEnded(object sender, RoutedEventArgs args)
       {
           mBackground.Pause();
           mBackground.Position = new TimeSpan(0, 0, 1);

           mBackground.Play();
       }


        /// <summary>
        /// This is called when the user can see but not interact with the application's window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowNoninteractive(object sender, EventArgs e)
        {
            // Disable audio for our movie.
           // Movie.IsMuted = true;
        }

        /// <summary>
        /// This is called when the application's window is not visible or interactive.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowUnavailable(object sender, EventArgs e)
        {
            // If our movie is currently playing, stop it.
           // StopMovie();
        }

        private void mBackground_MediaEnded(object sender, RoutedEventArgs e)
        {

        }
    }

   
}
