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
        /// The current editing mode for the DrawingPadCanvas. 
        /// When set, it updates the UI accordingly.
        /// </summary>
        private SurfaceInkEditingMode CurrentEditingMode
        {
            get
            {
                return DrawingPadCanvas.EditingMode;
            }
            set
            {
                if (value == SurfaceInkEditingMode.Ink)
                {
                    DrawingPadCanvas.EditingMode = SurfaceInkEditingMode.Ink;

                    // Update button image to show that we are now in ink mode.
                    EditModeButton.Content = DrawButtonImage;

                }
                else
                {
                    DrawingPadCanvas.EditingMode = SurfaceInkEditingMode.EraseByPoint;

                    // Update button image to show that we are now in erase mode.
                    EditModeButton.Content = FindResource("EraseButtonImage"); ;
                }

            }
        }

        /// <summary>
        /// The most recent stroke (used for undo operations).
        /// </summary>
        private Stroke mostRecentStroke;

        /// <summary>
        /// A mapping of each stroke to list of point timings in milliseconds.
        /// </summary>
        private readonly Dictionary<Stroke, List<long>> recordedStrokes = new Dictionary<Stroke, List<long>>();

        /// <summary>
        /// A copy of the recordedStrokes used for playback.
        /// Strokes are removed from this Dictionary as they are played back.
        /// </summary>
        private Dictionary<Stroke, List<long>> recordedStrokesCopy;

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

        /// <summary>
        /// True when the current color is tapped to open the color wheel
        /// </summary>
        private bool tappedOpen;
        

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
            UpdatePhotoSize();
        }

        /// <summary>
        /// Calculate the maximum size of photo based on logical dpi
        /// </summary>
        private void UpdatePhotoSize()
        {
            PhotoPadSVI.MaxWidth = Photo.Source.Width / (InteractiveSurface.PrimarySurfaceDevice.LogicalDpiX / 96);
            PhotoPadSVI.MaxHeight = Photo.Source.Height / (InteractiveSurface.PrimarySurfaceDevice.LogicalDpiX / 96);
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

            //SetMoviePadContent(publicFoldersPath + @"\Videos\Sample Videos");
            SetPhotoPadContent(publicFoldersPath + @"\Pictures\Sample Pictures");

        }

        //TODO
        private void OurPhotoPad()
        {
            ScatterViewItem photoPad = new ScatterViewItem();
            photoPad.Name="a";
            photoPad.Height = 320;
            photoPad.Width=427;
            photoPad.MinWidth=296;
            photoPad.MaxWidth=1024;

        }



        /// <summary>
        /// Sets the content for the PhotoCanvas.
        /// </summary>
        /// <param name="publicPhotosPath">Path to public photos.</param>
        private void SetPhotoPadContent(string publicPhotosPath)
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
                {*/
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

        #endregion Initalization

        #region InkCanvasEvents

        /// <summary>
        /// Toggles the edit mode of a SurfaceInkCanvas between EraseByPoint and Ink.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="args">The arguments for the event.</param>
        private void EditModeClicked(object sender, RoutedEventArgs args)
        {
            SurfaceButton button = (SurfaceButton)sender;
            if (CurrentEditingMode == SurfaceInkEditingMode.Ink)
            {
                CurrentEditingMode = SurfaceInkEditingMode.EraseByPoint;
            }
            else
            {
                CurrentEditingMode = SurfaceInkEditingMode.Ink;
            }
        }

        #endregion InkCanvasEvents

        #region Drawing Pad Specific Code

        /// <summary>
        /// Handles the OnStrokeCollected event for SurfaceInkCanvas.
        /// </summary>
        /// <param name="sender">The SurfaceInkCanvas that raised the event.</param>
        /// <param name="args">The arguments for the event.</param>
        private void OnStrokeCollected(object sender, InkCanvasStrokeCollectedEventArgs args)
        {
            mostRecentStroke = args.Stroke;
        }

        /// <summary>
        /// Handles the click event for the undo button.
        /// </summary>
        /// <param name="sender">The button that raised the event.</param>
        /// <param name="e">The arguments for the event.</param>
        void UndoClicked(object sender, RoutedEventArgs e)
        {
            if (mostRecentStroke != null)
            {
                DrawingPadCanvas.Strokes.Remove(mostRecentStroke);
                mostRecentStroke = null;
            }
        }

        /// <summary>
        /// Handles the TouchDown event for the ColorWheel.
        /// </summary>
        /// <param name="sender">The color wheel.</param>
        /// <param name="args">The arguments for the event.</param>
        private void OnColorWheelTouchDown(object sender, TouchEventArgs args)
        {
            HandleInputDown(sender, args, false);
        }

        /// <summary>
        /// Handles the MouseDown event for the ColorWheel.
        /// </summary>
        /// <param name="sender">The color wheel.</param>
        /// <param name="args">The arguments for the event.</param>
        private void OnColorWheelMouseDown(object sender, MouseButtonEventArgs args)
        {
            HandleInputDown(sender, args, false);
        }

        /// <summary>
        /// Handles the LostTouchCapture event for the ColorWheel.
        /// </summary>
        /// <param name="sender">The color wheel.</param>
        /// <param name="args">The arguments for the event.</param>
        private void OnColorWheelLostTouchCapture(object sender, TouchEventArgs args)
        {
            HandleColorWheelLostCapture(sender, args);
        }

        /// <summary>
        /// Handles the LostMouseCapture event for the ColorWheel.
        /// </summary>
        /// <param name="sender">The color wheel.</param>
        /// <param name="args">The arguments for the event.</param>
        private void OnColorWheelLostMouseCapture(object sender, MouseEventArgs args)
        {
            HandleColorWheelLostCapture(sender, args);
        }

        private void HandleColorWheelLostCapture(object sender, InputEventArgs args)
        {
            UIElement element = sender as UIElement;
            // If there are input devices captured to the color wheel,
            // choose a color based on the position of the last touch device
            if (!element.GetAreAnyInputDevicesCaptured())
            {
                // Select a color if hit, but keep the color wheel open if not
                if (ChooseColor(args, true))
                {
                    // If an actual color was chosen, release any capture on the current
                    // color indicator as the color wheel was already dismissed
                    CurrentColor.ReleaseAllCaptures();
                }
            }
        }

        /// <summary>
        /// Handles the Tap gesture event for the current color indicator.
        /// </summary>
        /// <param name="sender">The current color indicator.</param>
        /// <param name="e">The arguments for the event.</param>
        private void OnCurrentColorTap(object sender, TouchEventArgs e)
        {
            // This event will be followed by a LostTouchCapture event.
            // Flag that it was tapped so that method can ensure the
            // color wheel stays open.
            tappedOpen = true;
        }

        /// <summary>
        /// Handles the LostTouchCapture event for the current color indicator.
        /// </summary>
        /// <param name="sender">The current color indicator.</param>
        /// <param name="args">The arguments for the event.</param>
        private void OnCurrentColorLostTouchCapture(object sender, TouchEventArgs args)
        {
            // Select a color if hit, based on the position of the only touch device
            // that just lost capture on the CurrentColor indicator and always dismiss
            // the color wheel
            ChooseColor(args, tappedOpen);
            tappedOpen = false;
        }

        /// <summary>
        /// Handles the LostMouseCapture event for the current color indicator.
        /// </summary>
        /// <param name="sender">The current color indicator.</param>
        /// <param name="args">The arguments for the event.</param>
        private void OnCurrentColorLostMouseCapture(object sender, MouseEventArgs args)
        {
            // Select a color if hit, based on the position of the only touch device
            // that just lost capture on the CurrentColor indicator and always dismiss
            // the color wheel
            ChooseColor(args, false);
        }

        /// <summary>
        /// Handles the MouseUp event for the current color indicator or color wheel.
        /// </summary>
        /// <param name="sender">The current color indicator or color wheel.</param>
        /// <param name="args">The arguments for the event.</param>
        private void OnMouseUp(object sender, MouseButtonEventArgs args)
        {
            // If the mouse was already captured to the sender, release it
            IInputElement element = sender as IInputElement;
            if (args.Device.GetCaptured() == element)
            {
                element.ReleaseMouseCapture();
            }
        }

        /// <summary>
        /// Select a color based on the postion of args.TouchDevice if hit.
        /// </summary>
        /// <param name="args">The arguments for the input event.</param>
        /// <param name="closeOnlyOnHit">Indicates if the ColorWheel should
        /// be kept open when an actual color is chosen.</param>
        /// <returns> true if a color was actually chosen.</returns>
        private bool ChooseColor(InputEventArgs args, bool closeOnlyOnHit)
        {
            // If the color wheel is not visible, bail out
            if (ColorWheel.Visibility == Visibility.Hidden)
            {
                return false;
            }

            // Set the color on the CurrentColor indicator and on the SurfaceInkCanvas
            Color color = GetPixelColor(args.Device);

            // Black means the user touched the transparent part of the wheel. In that 
            // case, leave the color set to its current value
            bool hit = color != Colors.Black;

            // close the colorwheel if caller always requests or only if a
            // color is actually chosen.
            bool close = !closeOnlyOnHit || hit;
            
            if (hit)
            {
                DrawingPadCanvas.DefaultDrawingAttributes.Color = color;
                CurrentColor.Fill = new SolidColorBrush(color);
            }

            if (close)
            {
                CurrentEditingMode = SurfaceInkEditingMode.Ink;

                // Replace the color wheel with the current color button
                ColorWheel.Visibility = Visibility.Hidden;
            }

            args.Handled = true;
            return hit;
        }


        /// <summary>
        /// Handles the TouchtDownEvent for the current color indicator.
        /// </summary>
        /// <param name="sender">The current color indicator that raised the event.</param>
        /// <param name="args">The arguments for the event.</param>
        private void OnCurrentColorTouchDown(object sender, TouchEventArgs args)
        {
            HandleInputDown(sender, args, true);
        }

        /// <summary>
        /// Handles the MouseDownEvent for the current color indicator.
        /// </summary>
        /// <param name="sender">The current color indicator that raised the event.</param>
        /// <param name="args">The arguments for the event.</param>
        private void OnCurrentColorMouseDown(object sender, MouseButtonEventArgs args)
        {
            HandleInputDown(sender, args, true);
        }

        private void HandleInputDown(object sender, InputEventArgs args, bool makeVisible)
        {
            // Capture the touch device and handle the event 
            IInputElement element = sender as IInputElement;
            if (element != null && args.Device.Capture(element))
            {
                args.Handled = true;
            }

            if (makeVisible)
            {
                // Overlay the current color button with the color wheel
                ColorWheel.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Gets the color of a specific pixel.
        /// </summary>
        /// <param name="pt">The point from which to get a color.</param>
        /// <returns>The color of the point.</returns>
        private System.Windows.Media.Color GetPixelColor(InputDevice inputDevice)
        {
            // Translate the input point to bitmap coordinates
            double transformFactor = ColorWheel.Source.Width / ColorWheel.ActualWidth;
            Point inputPoint = inputDevice.GetPosition(ColorWheel);
            Point bitmapPoint = new Point(inputPoint.X * transformFactor, inputPoint.Y * transformFactor);

            // The point is outside the color wheel. Return black.
            if (bitmapPoint.X < 0 || bitmapPoint.X >= ColorWheel.Source.Width ||
                bitmapPoint.Y < 0 || bitmapPoint.Y >= ColorWheel.Source.Height)
            {
                return Colors.Black;
            }

            // The point is inside the color wheel. Find the color at the point.
            CroppedBitmap cb = new CroppedBitmap(ColorWheel.Source as BitmapSource, new Int32Rect((int)bitmapPoint.X, (int)bitmapPoint.Y, 1, 1));
            byte[] pixels = new byte[4];
            cb.CopyPixels(pixels, 4, 0);
            return Color.FromRgb(pixels[2], pixels[1], pixels[0]);
        }

        #endregion Drawing Pad Specific Code

        #region Photo Pad Specific Code

        /// <summary>
        /// Handles the click event for the clear button.
        /// </summary>
        /// <param name="sender">The button that reaised the event.</param>
        /// <param name="e">The arguments for the event.</param>
        void ClearClicked(object sender, RoutedEventArgs e)
        {
            PostcardCanvas.Strokes.Clear();
        }

        /// <summary>
        /// Handles the click event for the Move/Draw mode button on the PhotoPad.
        /// </summary>
        /// <param name="sender">The button that raised the event.</param>
        /// <param name="e">The arguments for the event.</param>
        void InkCanvasOnOffChanged(object sender, RoutedEventArgs e)
        {
            SurfaceButton button = (SurfaceButton) sender;
            if (PostcardCanvas.IsHitTestVisible)
            {
                // Prevent the SurfaceinkCanvas from processing input
                PostcardCanvas.IsHitTestVisible = false;

                // Load the new button image
                button.Content = MoveButtonImage;
            }
            else
            {
                // Let the SurfaceinkCanvas start processing input
                PostcardCanvas.IsHitTestVisible = true;

                // Load the new button image
                button.Content = FindResource("EditButtonImage");
            }
        }

        #endregion Photo Pad Specific Code




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
