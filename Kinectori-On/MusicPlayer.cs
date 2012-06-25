using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using System.Media;
using System.ComponentModel;
using System.Windows;
using System.Runtime.InteropServices;

namespace Kinectori_On
{
    public class MusicPlayer : INotifyPropertyChanged
    {
        const int MAX_SAMPLE_COUNT = 16;
        const int MAX_FMOD_CHANNELS = 128;

        #region Member Variables
        private DispatcherTimer metronome;
        private DispatcherTimer fmodUpdateTimer;
        private int beatIndex = 0;
        private int bpm;

        private FMOD.System fmodSystem;
        private List<FMOD.Sound>[] inactiveFMODSoundsByWavFile;
        private string[] wavFilePaths = new string[MAX_SAMPLE_COUNT];
        private FMOD.CHANNEL_CALLBACK channelCallback;

        #endregion Member Variables




        #region Metronome Event
        public delegate void MetronomeTickedHandler(MusicPlayer sender, EventArgs e);
        public event MetronomeTickedHandler MetronomeTicked;
        #endregion Metronome Event



        // Notifications for Bindings
        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion



        #region Constructor and Destructor

        public MusicPlayer()
        {
            // Default property values
            this.BPM = 120;
            beatIndex = -1;

            // Setup inactive sounds arrays
            inactiveFMODSoundsByWavFile = new List<FMOD.Sound>[MAX_SAMPLE_COUNT];
            for (int i = 0; i < MAX_SAMPLE_COUNT; i++)
            {
                inactiveFMODSoundsByWavFile[i] = new List<FMOD.Sound>();
            }

            // Create and init the FMOD System
            checkFMODError(FMOD.Factory.System_Create(ref fmodSystem));;

            uint version =0 ;
            checkFMODError(fmodSystem.getVersion(ref version));
            if (version < FMOD.VERSION.number)
            {
                MessageBox.Show("Error!  You are using an old version of FMOD " + version.ToString("X") + ".  This program requires " + FMOD.VERSION.number.ToString("X") + ".");
                Application.Current.Shutdown(-1);
            }

            checkFMODError(fmodSystem.setSoftwareChannels(MAX_FMOD_CHANNELS));

            checkFMODError(fmodSystem.init(MAX_FMOD_CHANNELS, FMOD.INITFLAGS.NORMAL | FMOD.INITFLAGS.VOL0_BECOMES_VIRTUAL, (IntPtr)null));

            // Channel Callback Delegate
            channelCallback = new FMOD.CHANNEL_CALLBACK(FMODChannelCallback);
        }


        ~MusicPlayer()
        {
            Stop();

            // get all currently playing sounds and clean them up
            for (int channelIndex = 0; channelIndex < MAX_FMOD_CHANNELS; channelIndex++)
            {
                FMOD.Channel channel = null;
                fmodSystem.getChannel(channelIndex, ref channel);

                if(channel != null)
                {
                    channel.stop();
                    
                    FMOD.Sound sound = null;
                    channel.getCurrentSound(ref sound);

                    if (sound == null)
                        continue;

                    IntPtr samplePathPtr = new IntPtr();
                    checkFMODError(sound.getUserData(ref samplePathPtr));

                    Marshal.FreeHGlobal(samplePathPtr); // this memory was allocated when the sound was created in MetronomeTick()
                    checkFMODError(sound.release());
                }
            }

            // clean up inactive sounds
            foreach (List<FMOD.Sound> soundList in inactiveFMODSoundsByWavFile)
            {
                foreach (FMOD.Sound sound in soundList)
                {
                    IntPtr samplePathPtr = new IntPtr();
                    checkFMODError(sound.getUserData(ref samplePathPtr));

                    Marshal.FreeHGlobal(samplePathPtr); // this memory was allocated when the sound was created in MetronomeTick()
                    checkFMODError(sound.release());
                }
            }

            // close & release the FMOD system
            if (fmodSystem != null)
            {
                checkFMODError(fmodSystem.close());
                checkFMODError(fmodSystem.release());
            }
        }

        #endregion Constructor and Destructor





        #region Public Methods

        public void setWavFilePathForSampleIndex(string path, int sampleIndex)
        {
            if (sampleIndex >= MAX_SAMPLE_COUNT)
            {
                Console.WriteLine("MusicPlayer setWavFilePathForSampleIndex(), sampleIndex {0} is beyond bounds ({1})", sampleIndex, MAX_SAMPLE_COUNT);
				return;
            }

            wavFilePaths[sampleIndex] = path;

            foreach (FMOD.Sound sound in inactiveFMODSoundsByWavFile[sampleIndex])
            {
                checkFMODError(sound.release());
            }
            inactiveFMODSoundsByWavFile[sampleIndex] = new List<FMOD.Sound>();
        }


        public void clearWavFilePathForSampleIndex(int sampleIndex)
        {
            if (sampleIndex >= MAX_SAMPLE_COUNT)
            {
                Console.WriteLine("MusicPlayer setWavFilePathForSampleIndex(), sampleIndex {0} is beyond bounds ({1})", sampleIndex, MAX_SAMPLE_COUNT);
				return;
            }

            wavFilePaths[sampleIndex] = null;

            foreach (FMOD.Sound sound in inactiveFMODSoundsByWavFile[sampleIndex])
            {
                checkFMODError(sound.release());
            }
            inactiveFMODSoundsByWavFile[sampleIndex] = new List<FMOD.Sound>();
        }


        public void Start()
        {
            if ( this.KinectoriWindow == null || metronome != null )
            {
                return;
            }

            metronome = new DispatcherTimer();
            metronome.Interval = TimeSpan.FromSeconds(1.0/ ((double)this.BPM / 60.0));
            metronome.Tick += MetronomeTick;
            metronome.Start();

            fmodUpdateTimer = new DispatcherTimer();
            fmodUpdateTimer.Interval = TimeSpan.FromSeconds(1.0 / 60.0);
            fmodUpdateTimer.Tick += UpdateFMODSystem;
            fmodUpdateTimer.Start();
        }


        public void Stop()
        {
            if (metronome != null)
            {
                metronome.Stop();
                metronome = null;
            }

            if (fmodUpdateTimer != null)
            {
                fmodUpdateTimer.Stop();
                fmodUpdateTimer = null;
            }
        }


        public void Reset()
        {
            beatIndex = -1;
        }

        #endregion Public Methods




        #region Private Methods

        private void MetronomeTick(object sender, EventArgs e)
        {
            // increment the beat index
            beatIndex++;
            if (beatIndex >= this.KinectoriWindow.ControlGridColumnCount())
            {
                beatIndex = 0;
            }

            // interate through the rows in the control grid and find active cells
            for (int sampleIndex = 0; sampleIndex < this.KinectoriWindow.ControlGridRowCount() && sampleIndex < MAX_SAMPLE_COUNT; sampleIndex++)
            {
                int controlCellStatus = this.KinectoriWindow.statusOfControlGridCell(sampleIndex, beatIndex);
                if (controlCellStatus > 0 && wavFilePaths[sampleIndex] != null)
                {
                    FMOD.Sound fmodSoundForSample = null;

                    int inactivePlayerCount = inactiveFMODSoundsByWavFile[sampleIndex].Count;
                    if (inactivePlayerCount > 0)
                    {
                        // If we have an inactive SoundPlayer for this sample, use it
                        fmodSoundForSample = inactiveFMODSoundsByWavFile[sampleIndex][inactivePlayerCount - 1];
                        inactiveFMODSoundsByWavFile[sampleIndex].RemoveAt(inactivePlayerCount -1);    // removing from the end of a list is the fastest remove operation
                    }
                    else
                    {
                        // otherwise, create a new one
                        checkFMODError(fmodSystem.createSound(wavFilePaths[sampleIndex], FMOD.MODE.CREATESAMPLE | FMOD.MODE.SOFTWARE, ref fmodSoundForSample));
                        fmodSoundForSample.setUserData(Marshal.StringToHGlobalUni(wavFilePaths[sampleIndex]));  // store the sample path as the sound's user data, to know if we can reuse this sound later if the sample is still in use 
                        // note that StringToHGlobalUni() allocates memory for the contents of the string, this gets released in the channel playback-ended callback if the sample is no longer valid and the sound is being released.
                    }

                    FMOD.Channel newChannel = null;

                    // This commented code will prevent a sample that is currently playing from being restarted
					//
                    //fmodSystem.getChannel(sampleIndex, ref newChannel);
                    //if (newChannel != null)
                    //{
                    //    bool isPlaying = false;
                    //    newChannel.isPlaying(ref isPlaying);
                    //    if (isPlaying)
                    //    {
                    //        continue;
                    //    }
                    //}

                    // play the sound
                    newChannel = null;
                    checkFMODError(fmodSystem.playSound((FMOD.CHANNELINDEX)sampleIndex, fmodSoundForSample, false, ref newChannel));
					// Note that the channelid parameter is set to the sampleIndex value - this means only a single instance of any one sample will play at any one time.
					// To have multiple instances of each sample able to be triggered, change the channelid parameter to FMOD.CHANNELINDEX.FREE, which will allocation any available channel, up to the max specified when the system was init'd

                    // store the sample index as the channel's user info, so it can easily retrieved later
                    newChannel.setUserData(new IntPtr(sampleIndex));

                    // register for the callback to get notified when the sound playback ends
                    newChannel.setCallback(channelCallback);
                }
            }

            // trigger the metronome ticked event
            if (MetronomeTicked != null)
            {
                MetronomeTicked(this, EventArgs.Empty);
            }
        }


        private FMOD.RESULT FMODChannelCallback(IntPtr channelraw, FMOD.CHANNEL_CALLBACKTYPE type, IntPtr commanddata1, IntPtr commanddata2)
        {
            if (type == FMOD.CHANNEL_CALLBACKTYPE.END)
            {
                // get the channel
                FMOD.Channel finishedChannel = new FMOD.Channel();
                finishedChannel.setRaw(channelraw);
                
                // retreive the sample index from the channel
                IntPtr sampleIndexPtr = new IntPtr();
                checkFMODError(finishedChannel.getUserData(ref sampleIndexPtr));
                int sampleIndex = sampleIndexPtr.ToInt32();

                // get the sound
                FMOD.Sound sound = null;
                checkFMODError(finishedChannel.getCurrentSound(ref sound));

                // retreive the sample file path from the sound
                IntPtr samplePathPtr = new IntPtr();
                checkFMODError(sound.getUserData(ref samplePathPtr));
                string samplePath = Marshal.PtrToStringUni(samplePathPtr);

                if (sampleIndex >= 0 && sampleIndex < MAX_SAMPLE_COUNT && samplePath.Equals(wavFilePaths[sampleIndex]))
                {
                    // the sample path is still in use, so we can re use this sound object
                    inactiveFMODSoundsByWavFile[sampleIndex].Add(sound);
                }
                else
                {
                    // The user has loaded a new sample into this index, so clean up and get rid of this sound
                    Marshal.FreeHGlobal(samplePathPtr); // this memory was allocated when the sound was created in MetronomeTick()
                    checkFMODError(sound.release());
                }
            }

            return FMOD.RESULT.OK;
        }


        private void UpdateFMODSystem(object sender, EventArgs e)
        {
            fmodSystem.update();
        }


        private void checkFMODError(FMOD.RESULT result)
        {
            if (result != FMOD.RESULT.OK)
            {
                Console.WriteLine("FMOD error! " + result + " - " + FMOD.Error.String(result));
            }
        }

        #endregion Private Methods







        #region Properties

        public MainWindow KinectoriWindow
        {
            get;
            set;
        }

        
        public int BPM
        {
            get
            {
                return this.bpm;
            }
            set
            {
                this.bpm = value;

                if (null != this.PropertyChanged)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("BPM"));
                }

                if (metronome != null)
                {
                    double seconds = 1.0 / ((double)this.BPM / 60.0);
                    metronome.Interval = TimeSpan.FromSeconds(seconds);
                }
            }
        }


        public int BeatIndex
        {
            get
            {
                return this.beatIndex;
            }
        }


        #endregion Properties


    }
}
