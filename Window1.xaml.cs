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
using System.Threading;

namespace NeSt
{
    /// <summary>
    /// Interaction logic for Window1.xaml.
    /// </summary>
    public partial class Window1 : Window
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

        //private Storyboard stb;

        /// <summary>
        /// Duration of playback loop (in seconds) when no video is present.
        /// </summary>
        private const int duration = 12;
        //private MediaElement video;
        //public event RoutedEventHandler mMediaEnded;

        /// <summary>
        /// Key used to track strokes in a touch device's user data.
        /// </summary>
        private readonly object strokeKey = new object();

        /// <summary>
        /// Brushes used for movie buttons
        /// </summary>
        //private static SolidColorBrush buttonHighlightBrush = new SolidColorBrush (Color.FromArgb(0xCC, 0x99, 0x99, 0x99));
        //private static SolidColorBrush buttonBackgroundBrush = new SolidColorBrush (Color.FromArgb(0xCC, 0xCC, 0xCC, 0xCC));


        private ArticleList allArticles;
        private PlayerList players;
        private FinishedArticles finishedArticles;
        

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

            players = new PlayerList(4);
            Control.Instance.playerList = players;

            finishedArticles = new FinishedArticles();
            Control.Instance.finishedArticles = finishedArticles;
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
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

            Control.Instance.window1 = this;
            Control.Instance.mainScatterView = MainScatterView;

            // Load images and video from the public folders.
            // These are default OS folders that will always be in these locations
            // unless the user has deliberately moved them.
            string publicFoldersPath = Environment.GetEnvironmentVariable("public");
            //string path = publicFoldersPath + @"\Pictures\Sample Pictures";
            string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location.ToString()) + @"\..\..\Resources\Articles";

            //Load images and texts from Resource Folder
            //string path = @"pack://application:,,,/Resources/Articles";
            LoadFilesFrom(path);

            string targetVideoPath = publicFoldersPath + @"\Videos\Sample Videos";

            mBackground.BeginInit();
            mBackground.LoadedBehavior = mBackground.UnloadedBehavior = MediaState.Manual;
            mBackground.EndInit();
            mBackground.Position = TimeSpan.Zero;
            mBackground.Play();
                
            VideoDrawing vd = new VideoDrawing();
            VisualBrush vb = new VisualBrush();
                
            vb.Visual = mBackground;
            vb.Stretch = Stretch.Fill;
            vb.TileMode = TileMode.None;
                
            mWindow.Background = vb;

            AddHandler(Keyboard.KeyDownEvent, (KeyEventHandler)OnKeyDownHandler); // catch keyboard events

            allArticles.initialize();
            //allArticles.animateAll(); // start moving

        }

        /// <summary>
        /// reacting to keyboard events
        /// <summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        // TODO: expand for service tasks (resetting, style switching)
        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            Control.Instance.keyboardInput(e);
        }

        /// <summary>
        /// Load all images and headline texts from a given directory
        /// </summary>
        /// <param name="directoryPath">Path of the directory in which the files are</param>
        private void LoadFilesFrom(string directoryPath)
        {
            if (Directory.Exists(directoryPath))
            {
                var filteredFiles = Directory
                .GetFiles(directoryPath, "*.*")
                .Where(file => file.ToLower().EndsWith("png") || file.ToLower().EndsWith("jpg"))
                .ToList();

                string[] files = Directory.GetFiles(directoryPath, "*.jpg");

                allArticles = new ArticleList();
                Control.Instance.articleList = allArticles;
                for (int i = 0; i < filteredFiles.Count; i++)
                {
                    allArticles.articles.Add(new Article(filteredFiles[i]));
                }

            }
            else
            {
                MessageBox.Show("Error: Article directory not found.");
            }
        }

        #endregion Initalization

        /// <summary>
        /// Automatically move an article piece
        /// </summary>
        /// <param name="item">The piece to move</param>
        //private void MoveItem(ScatterViewItem item)
        //{
        //    stb = new Storyboard();
        //    PointAnimation moveCenter = new PointAnimation();
        //    Point endPoint = new Point(1024 / 2, 768 / 2);
        //    moveCenter.From = item.ActualCenter;
        //    moveCenter.To = endPoint;
        //    moveCenter.Duration = new Duration(TimeSpan.FromSeconds(10.0));
        //    moveCenter.FillBehavior = FillBehavior.Stop;
        //    stb.Children.Add(moveCenter);
        //    Storyboard.SetTarget(moveCenter, item);
        //    Storyboard.SetTargetProperty(moveCenter, new PropertyPath(ScatterViewItem.CenterProperty));
        //    stb.Begin(this);
        //}

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
           
            //foreach (ScatterViewItem item in allItems)
            //{
            //    MoveItem(item);
            //}
        }


       /// <summary>
       /// Handles the MediaEnded event for the MovieCanvas video.
       /// </summary>
       /// <param name="sender">The MediaElement that raised the event.</param>
       /// <param name="args">The arguments for the event.</param>
       private void onMediaEnded(object sender, RoutedEventArgs args)
       {
   //        mBackground.Pause();
           mBackground.Position = new TimeSpan(0, 0, 0);

    //       mBackground.Play();
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

        /// <summary>
        /// (no info)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mBackground_MediaEnded(object sender, RoutedEventArgs e)
        {

        }
    }
   
}
