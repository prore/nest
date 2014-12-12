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
            
        /// <summary>
        /// Duration of playback loop (in seconds) when no video is present.
        /// </summary>
        private const int duration = 12;
        
        
        /// <summary>
        /// Key used to track strokes in a touch device's user data.
        /// </summary>
        private readonly object strokeKey = new object();

        /// <summary>
        /// Brushes used for movie buttons
        /// </summary>
        private static SolidColorBrush buttonHighlightBrush = new SolidColorBrush (Color.FromArgb(0xCC, 0x99, 0x99, 0x99));
        private static SolidColorBrush buttonBackgroundBrush = new SolidColorBrush (Color.FromArgb(0xCC, 0xCC, 0xCC, 0xCC));

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
            
            //LoadImg(publicFoldersPath + @"\Pictures\Sample Pictures\Desert.jpg");
            //LoadImg(publicFoldersPath + @"\Pictures\Sample Pictures\Jellyfish.jpg");
            LoadAllImgFrom(publicFoldersPath + @"\Pictures\Sample Pictures");

        }

        //TODO
        private void LoadImg(string path)
        {
            ScatterViewItem photoPad = new ScatterViewItem();
            //photoPad.Name="a";
            photoPad.Height = 320/2;
            photoPad.Width=427/2;
            photoPad.MinWidth=296/2;
            photoPad.MaxWidth=300;


            Image img1 = new Image();
            img1.Source = new BitmapImage(new Uri(path));
            
            photoPad.Content = img1;

            MainScatterView.Items.Add(photoPad);
            
        }

        private void LoadAllImgFrom(string imgDirectoryPath)
        {
            if (Directory.Exists(imgDirectoryPath))
            {
                string[] files = Directory.GetFiles(imgDirectoryPath,  "*.jpg");
                for(int i = 0; i<files.Length; i++) 
                {
                    LoadImg(files[i]);
                }
            }
        }

        /// <summary>
        /// Sets the content for the PhotoCanvas.
        /// </summary>
        /// <param name="publicPhotosPath">Path to public photos.</param>
  /*      private void SetPhotoPadContent(string publicPhotosPath)
        {

            if (Directory.Exists(publicPhotosPath))
            {
                // Use this images if it exists.
                string targetPhotoPath = publicPhotosPath + @"\Chrysanthemum.jpg";
                /*if (File.Exists(targetPhotoPath))
                {
                    // Target image exists, use it
                    Photo.Source = new BitmapImage(new Uri(targetPhotoPath));
                }
                else
                {
                    // Target image does not exist, use the first JPG found in the directory.
                    string[] files = Directory.GetFiles(publicPhotosPath, "*.jpg");
                    if (files.Length > 0)
                    {
                        Photo.Source = new BitmapImage(new Uri(files[0]));
                    }
                //}       
            }

            if (Photo.Source == null)
            {
                // If there aren't any images at all, use a blank canvas.
                PostcardCanvas.Background = Brushes.White;
            }
            else
            {
                UpdatePhotoSize();
            }
        }
*/
        #endregion Initalization
        
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
    }

}
