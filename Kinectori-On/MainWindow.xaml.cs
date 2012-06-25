using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Kinect;

namespace Kinectori_On
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Member Variables
        private const int controlGridSpacing = 60;
        private Image[,] controlGridImages;
        private Image[] metronomeGridImages;
        private BitmapImage[] gridImageBitmaps;
        private KinectSensor _kinect;
        private short[] depthImagePixelData = new short[320 * 240];
        private MusicPlayer musicPlayer;
        #endregion Member Variables


        public MainWindow()
        {
            InitializeComponent();
            SetupControlGridAndMetronomeGrid();

            musicPlayer = new MusicPlayer();
            musicPlayer.KinectoriWindow = this;
            musicPlayer.MetronomeTicked += MusicPlayer_MetronomeTicked;

            // for bindings to the BPM Controls
            TopToolBar.DataContext = this;

            this.Loaded += (s, e) => { DiscoverKinectSensor(); };
            this.Unloaded += (s, e) => { this.Kinect = null; };
        }



        #region Methods

        #region Setup Methods

        private void SetupControlGridAndMetronomeGrid()
        {
            gridImageBitmaps = new BitmapImage[7];
            gridImageBitmaps[0] = new BitmapImage(new Uri("pack://application:,,,/Images/GridLight-off.png"));
            gridImageBitmaps[1] = new BitmapImage(new Uri("pack://application:,,,/Images/GridLight-Green.png"));
            gridImageBitmaps[2] = new BitmapImage(new Uri("pack://application:,,,/Images/GridLight-Red.png"));
            gridImageBitmaps[3] = new BitmapImage(new Uri("pack://application:,,,/Images/GridLight-Blue.png"));
            gridImageBitmaps[4] = new BitmapImage(new Uri("pack://application:,,,/Images/GridLight-Yellow.png"));
            gridImageBitmaps[5] = new BitmapImage(new Uri("pack://application:,,,/Images/GridLight-Magenta.png"));
            gridImageBitmaps[6] = new BitmapImage(new Uri("pack://application:,,,/Images/GridLight-Cyan.png"));


            int rowCount = ((int)ControlGrid.Height / controlGridSpacing);
            int columnCount = ((int)ControlGrid.Width / controlGridSpacing);

            // should probably move grid definitions here from the xaml, to make the grid actually dynamic...

            controlGridImages = new Image[columnCount, rowCount];

            for (int row = 0; row < rowCount; row++)
            {
                for (int col = 0; col < columnCount; col++)
                {
                    controlGridImages[col, row] = new Image();
                    controlGridImages[col, row].Source = gridImageBitmaps[0];
                    controlGridImages[col, row].Width = controlGridSpacing;
                    controlGridImages[col, row].Height = controlGridSpacing;

                    ControlGrid.Children.Add(controlGridImages[col, row]);
                    Grid.SetColumn(controlGridImages[col, row], col);
                    Grid.SetRow(controlGridImages[col, row], row);
                }
            }


            metronomeGridImages = new Image[columnCount];
            for (int col = 0; col < columnCount; col++)
            {
                metronomeGridImages[col] = new Image();
                metronomeGridImages[col].Source = gridImageBitmaps[0];
                metronomeGridImages[col].Width = MetronomeGrid.Height;
                metronomeGridImages[col].Height = MetronomeGrid.Height;

                MetronomeGrid.Children.Add(metronomeGridImages[col]);
                Grid.SetColumn(metronomeGridImages[col], col);
                Grid.SetRow(metronomeGridImages[col], 0);
            }
        }


        private void DiscoverKinectSensor()
        {
            KinectSensor.KinectSensors.StatusChanged += KinectSensors_StatusChanged;
            this.Kinect = KinectSensor.KinectSensors.FirstOrDefault(x => x.Status == KinectStatus.Connected);
        }


        private void KinectSensors_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            switch (e.Status)
            {
                case KinectStatus.Connected:
                    if (this.Kinect == null)
                    {
                        this.Kinect = e.Sensor;
                    }
                    break;

                case KinectStatus.Disconnected:
                    if (this.Kinect == e.Sensor)
                    {
                        this.Kinect = null;
                        this.Kinect = KinectSensor.KinectSensors.FirstOrDefault(x => x.Status == KinectStatus.Connected);
                        if (this.Kinect == null)
                        {
                            Console.WriteLine("The Kinect sensor is disconnected");
                        }
                    }
                    break;

                // Handle all other statuses according to needs
            }
        }


        private void InitializeKinectSensor(KinectSensor sensor)
        {
            if (sensor != null)
            {
                sensor.DepthStream.Enable(DepthImageFormat.Resolution320x240Fps30);
                sensor.SkeletonStream.Enable(); // note: the skeleton stream must be enabled for the depth pixel values to include the player ID

                sensor.DepthFrameReady += Kinect_DepthFrameReady;
                sensor.Start();
            }
        }


        private void UninitializeKinectSensor(KinectSensor sensor)
        {
            if (sensor != null)
            {
                sensor.Stop();
                sensor.DepthFrameReady -= Kinect_DepthFrameReady;
            }
        }


        #endregion Setup Methods


        #region Kinect Processing Methods

        private void Kinect_DepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
        {
            using (DepthImageFrame frame = e.OpenDepthImageFrame())
            {
                if (frame != null)
                {
                    // update depth image
                    frame.CopyPixelDataTo(depthImagePixelData);
                    MapPlayerShapeToControlGrid();
                }
            }
        }


        private void MapPlayerShapeToControlGrid()
        {
            // Depth Image is 320 * 240

            // Strech the depth image from the Kinect to match the width and height of the control grid
            // In each grid cell, count the number of depth image pixels that represent a player
            // If the number of player pixels is > 25% of the total number of pixels, 'turn on' that cell

            int gridRowCount = ((int)ControlGrid.Height / controlGridSpacing);
            int gridColumnCount = ((int)ControlGrid.Width / controlGridSpacing);

            int cellWidthInDepthPixels = this.Kinect.DepthStream.FrameWidth / gridColumnCount;
            int cellHeightInDepthPixels = this.Kinect.DepthStream.FrameHeight / gridColumnCount;

            int numOfDepthPixelsPerCell = cellHeightInDepthPixels * cellWidthInDepthPixels;

            for (int gridRow = 0; gridRow < gridRowCount; gridRow++)
            {
                for (int gridColumn = 0; gridColumn < gridColumnCount; gridColumn++)
                {
                    int[] playersPixelCountsInCell = new int[7];

                    for (int depthPixelY = gridRow * cellHeightInDepthPixels; depthPixelY < (gridRow + 1) * cellHeightInDepthPixels; depthPixelY++)
                    {
                        // shortcut to save checking all pixels if possible
                        if (playersPixelCountsInCell.Max() > (numOfDepthPixelsPerCell / 4)) break;

                        for (int depthPixelX = gridColumn * cellWidthInDepthPixels; depthPixelX < (gridColumn + 1) * cellWidthInDepthPixels; depthPixelX++)
                        {
                            // shortcut to save checking all pixels if possible
                            if (playersPixelCountsInCell.Max() > (numOfDepthPixelsPerCell / 4)) break;

                            int playerIndex = this.depthImagePixelData[(depthPixelY * this.Kinect.DepthStream.FrameWidth) + depthPixelX] & DepthImageFrame.PlayerIndexBitmask;
                            playersPixelCountsInCell[playerIndex]++;
                        }
                    }

                    if (playersPixelCountsInCell[0] >= (numOfDepthPixelsPerCell * 0.75))
                    {
                        controlGridImages[gridColumn, gridRow].Source = gridImageBitmaps[0];
                    }
                    else
                    {
                        controlGridImages[gridColumn, gridRow].Source = gridImageBitmaps[Array.IndexOf(playersPixelCountsInCell, (playersPixelCountsInCell.Max()))];
                    }
                }
            }
        }

        #endregion Kinect Processing Methods




        #region Music Player Event Handling Methods

        private void MusicPlayer_MetronomeTicked(MusicPlayer player, EventArgs e)
        {
            int beatIndex = musicPlayer.BeatIndex;
            int columnCount = this.ControlGridColumnCount();

            if (beatIndex == 0)
            {
                metronomeGridImages[columnCount - 1].Source = gridImageBitmaps[0];
            }
            else
            {
                metronomeGridImages[beatIndex - 1].Source = gridImageBitmaps[0];
            }

            metronomeGridImages[beatIndex].Source = gridImageBitmaps[2];
        }

        #endregion Music Player Event Handling Methods



        #region UI Action Methods

        private void LoadSample(object sender, RoutedEventArgs e)
        {
            Button clickedButton = (Button)sender;
            int buttonTag = Convert.ToInt32(clickedButton.Tag.ToString());

            // Configure open file dialog box 
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.DefaultExt = ".wav"; // Default file extension 
            openFileDialog.Filter = "Wave Audio Files (.wav)|*.wav"; // Filter files by extension 

            // Show open file dialog box 
            Nullable<bool> result = openFileDialog.ShowDialog();

            // Process open file dialog box results 
            if (result == true)
            {
                // Open file 
                string filename = openFileDialog.FileName;
                musicPlayer.setWavFilePathForSampleIndex(filename, buttonTag);

                // Update UI
                TextBlock sampleNameTextBlock = SampleControlsGrid.Children.OfType<TextBlock>().Where(t => t.Tag.Equals(clickedButton.Tag)).First();
                sampleNameTextBlock.Text = System.IO.Path.GetFileNameWithoutExtension(filename);

                clickedButton.Content = "Clear";
                clickedButton.Click -= LoadSample;
                clickedButton.Click += ClearSample;
            }
        }


        private void ClearSample(object sender, RoutedEventArgs e)
        {
            Button clickedButton = (Button)sender;
            int buttonTag = Convert.ToInt32(clickedButton.Tag.ToString());

            musicPlayer.clearWavFilePathForSampleIndex(buttonTag);

            TextBlock sampleNameTextBlock = SampleControlsGrid.Children.OfType<TextBlock>().Where(t => t.Tag.Equals(clickedButton.Tag)).First();
            sampleNameTextBlock.Text = "Empty";

            clickedButton.Content = "Load";
            clickedButton.Click -= ClearSample;
            clickedButton.Click += LoadSample;
        }


        private void IncreaseBPM(object sender, RoutedEventArgs e)
        {
            this.TheMusicPlayer.BPM++;
        }


        private void DecreaseBPM(object sender, RoutedEventArgs e)
        {
            this.TheMusicPlayer.BPM--;
        }


        private void Start(object sender, RoutedEventArgs e)
        {
            musicPlayer.Start();
        }


        private void Stop(object sender, RoutedEventArgs e)
        {
            musicPlayer.Stop();
        }


        private void Reset(object sender, RoutedEventArgs e)
        {
            int beatIndex = musicPlayer.BeatIndex;
            musicPlayer.Reset();
            metronomeGridImages[beatIndex].Source = gridImageBitmaps[0];
        }


        #endregion UI Action Methods


        #region Public Methods

        public int ControlGridRowCount()
        {
            return (int)(ControlGrid.Height / controlGridSpacing);
        }


        public int ControlGridColumnCount()
        {
            return (int)(ControlGrid.Width / controlGridSpacing);
        }


        // Returns the status of the control Grid Cell at the specific row & column.
        // A value of 0 indicates the cell is off.
        // A value of >0 indicates the cell is on, and the value is the ID of the player who has activated the cell.
        public int statusOfControlGridCell(int row, int column)
        {
            Image imageForCell = ControlGrid.Children.OfType<Image>().Where(c => Grid.GetRow(c) == row && Grid.GetColumn(c) == column).First();
            return Array.IndexOf(gridImageBitmaps, imageForCell.Source);
        }

        #endregion Public Methods

        #endregion Methods




        #region Properties

        public KinectSensor Kinect
        {
            get { return this._kinect; }

            set
            {
                if (this._kinect != value)
                {
                    if (this._kinect != null)
                    {
                        UninitializeKinectSensor(this._kinect);
                        this._kinect = null;
                    }

                    if (value != null && value.Status == KinectStatus.Connected)
                    {
                        this._kinect = value;
                        InitializeKinectSensor(this._kinect);
                        Console.WriteLine("Kinect Ready");
                    }
                }
            }
        }

        // Expose the musicPlayer as a property to bind the BPM UI
        public MusicPlayer TheMusicPlayer
        {
            get {return this.musicPlayer;}
        }

        #endregion Properties;
    }
}
