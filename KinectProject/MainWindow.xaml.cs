using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using Microsoft.Speech.AudioFormat;
using Microsoft.Speech.Recognition;
using Microsoft.Speech;
using System.Threading;
using System.Globalization;
using System.IO;

namespace KinectProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Grid[] gridArray;   // Holds all Grids in Application
        int gridArrayIndex = 0; // index for gridArray
        KinectAudioSource _kinectSource;
        KinectSensor _kinectSensor;
        SpeechRecognitionEngine _speechEngine;
        Stream _stream;

        public MainWindow()
        {
            InitializeComponent();
            // Initializes Grid Array
            gridArray = new Grid[5] { MainScreen, AudioScreen, ApplicationsScreen, PhoneScreen, SettingsScreen };
        }

        static WaveGesture _gesture = new WaveGesture();

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {

            MainScreen.Visibility = System.Windows.Visibility.Hidden;
            AudioScreen.Visibility = System.Windows.Visibility.Hidden;
            ApplicationsScreen.Visibility = System.Windows.Visibility.Hidden;
            PhoneScreen.Visibility = System.Windows.Visibility.Hidden;
            SettingsScreen.Visibility = System.Windows.Visibility.Hidden;


            _kinectSensor = KinectSensor.KinectSensors[0];
            var recInstalled = SpeechRecognitionEngine.InstalledRecognizers();
            RecognizerInfo rec = (RecognizerInfo)recInstalled[0];
            _speechEngine = new SpeechRecognitionEngine(rec.Id);
            var choices = new Choices();
            choices.Add("main");
            choices.Add("home");
            choices.Add("settings");
            choices.Add("audio");
            choices.Add("apps");
            choices.Add("phone");

            var gb = new GrammarBuilder { Culture = rec.Culture };
            gb.Append(choices);
            var g = new Grammar(gb);

            _speechEngine.LoadGrammar(g);
            //recognized a word or words that may be a component of multiple complete phrases in a grammar.
            _speechEngine.SpeechHypothesized += new EventHandler<SpeechHypothesizedEventArgs>(SpeechEngineSpeechHypothesized);
            //receives input that matches any of its loaded and enabled Grammar objects.
            _speechEngine.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(_speechEngineSpeechRecognized);

            //C# threads are MTA by default and calling RecognizeAsync in the same thread will cause an COM exception.
            var t = new Thread(StartAudioStream);
            t.Start();

            

            var sensor = KinectSensor.KinectSensors.Where(
             s => s.Status == KinectStatus.Connected).FirstOrDefault();

            if (sensor != null)
            {
                sensor.SkeletonStream.Enable();
                //sensor.DepthStream.Range = DepthRange.Near; // (only works on Windows Kinect)
                //sensor.SkeletonStream.EnableTrackingInNearRange = true;
                //sensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated;

                sensor.SkeletonFrameReady += Sensor_SkeletonFrameReady;
                _gesture.GestureRecognized += Gesture_GestureRecognized;
                sensor.Start();
            }
        }

        void StartAudioStream()
        {
            _kinectSource = _kinectSensor.AudioSource;
            _kinectSource.AutomaticGainControlEnabled = false;
            _kinectSource.EchoCancellationMode = EchoCancellationMode.None;
            _stream = _kinectSource.Start();

            _speechEngine.SetInputToAudioStream(_stream,
                            new SpeechAudioFormatInfo(
                                EncodingFormat.Pcm, 16000, 16, 1,
                                32000, 2, null));

            _speechEngine.RecognizeAsync(RecognizeMode.Multiple);
        }


        void _speechEngineSpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result.Text == "main")
            {
                GridWelcome.Visibility = Visibility.Hidden;
                MainScreen.Visibility = Visibility.Visible;
                AudioScreen.Visibility = System.Windows.Visibility.Hidden;
                ApplicationsScreen.Visibility = System.Windows.Visibility.Hidden;
                PhoneScreen.Visibility = System.Windows.Visibility.Hidden;
                SettingsScreen.Visibility = System.Windows.Visibility.Hidden;
            }
            else
                if (e.Result.Text == "apps")
                {
                    GridWelcome.Visibility = Visibility.Hidden;
                    MainScreen.Visibility = Visibility.Hidden;
                    ApplicationsScreen.Visibility = Visibility.Visible;
                    AudioScreen.Visibility = System.Windows.Visibility.Hidden;
                    PhoneScreen.Visibility = System.Windows.Visibility.Hidden;
                    SettingsScreen.Visibility = System.Windows.Visibility.Hidden;
                }

            if (e.Result.Text == "phone")
            {
                GridWelcome.Visibility = Visibility.Hidden;
                MainScreen.Visibility = Visibility.Hidden;
                ApplicationsScreen.Visibility = Visibility.Hidden;
                AudioScreen.Visibility = System.Windows.Visibility.Hidden;
                PhoneScreen.Visibility = System.Windows.Visibility.Visible;
                SettingsScreen.Visibility = System.Windows.Visibility.Hidden;
            }
            if (e.Result.Text == "settings")
            {
                GridWelcome.Visibility = Visibility.Hidden;
                MainScreen.Visibility = Visibility.Hidden;
                ApplicationsScreen.Visibility = Visibility.Hidden;
                AudioScreen.Visibility = System.Windows.Visibility.Hidden;
                PhoneScreen.Visibility = System.Windows.Visibility.Hidden;
                SettingsScreen.Visibility = System.Windows.Visibility.Visible;
            }
            else
                if (e.Result.Text == "audio")
                {
                    GridWelcome.Visibility = Visibility.Hidden;
                    MainScreen.Visibility = Visibility.Hidden;
                    ApplicationsScreen.Visibility = Visibility.Hidden;
                    AudioScreen.Visibility = System.Windows.Visibility.Visible;
                    PhoneScreen.Visibility = System.Windows.Visibility.Hidden;
                    SettingsScreen.Visibility = System.Windows.Visibility.Hidden;
                }
        }

        void SpeechEngineSpeechHypothesized(object sender, SpeechHypothesizedEventArgs e)
        {
            if (e.Result.Text == "main")
            {
                GridWelcome.Visibility = Visibility.Hidden;
                MainScreen.Visibility = Visibility.Visible;
                AudioScreen.Visibility = System.Windows.Visibility.Hidden;
                ApplicationsScreen.Visibility = System.Windows.Visibility.Hidden;
                PhoneScreen.Visibility = System.Windows.Visibility.Hidden;
                SettingsScreen.Visibility = System.Windows.Visibility.Hidden;
            }
            else
                if (e.Result.Text == "application")
                {
                    GridWelcome.Visibility = Visibility.Hidden;
                    MainScreen.Visibility = Visibility.Hidden;
                    ApplicationsScreen.Visibility = Visibility.Visible;
                    AudioScreen.Visibility = System.Windows.Visibility.Hidden;
                    PhoneScreen.Visibility = System.Windows.Visibility.Hidden;
                    SettingsScreen.Visibility = System.Windows.Visibility.Hidden;
                }

            if (e.Result.Text == "phone")
            {
                GridWelcome.Visibility = Visibility.Hidden;
                MainScreen.Visibility = Visibility.Hidden;
                ApplicationsScreen.Visibility = Visibility.Hidden;
                AudioScreen.Visibility = System.Windows.Visibility.Hidden;
                PhoneScreen.Visibility = System.Windows.Visibility.Visible;
                SettingsScreen.Visibility = System.Windows.Visibility.Hidden;
            }

            if (e.Result.Text == "settings")
            {
                GridWelcome.Visibility = Visibility.Hidden;
                MainScreen.Visibility = Visibility.Hidden;
                ApplicationsScreen.Visibility = Visibility.Hidden;
                AudioScreen.Visibility = System.Windows.Visibility.Hidden;
                PhoneScreen.Visibility = System.Windows.Visibility.Hidden;
                SettingsScreen.Visibility = System.Windows.Visibility.Visible;

            }
            else
                if (e.Result.Text == "audio")
                {
                    GridWelcome.Visibility = Visibility.Hidden;
                    MainScreen.Visibility = Visibility.Hidden;
                    ApplicationsScreen.Visibility = Visibility.Hidden;
                    AudioScreen.Visibility = System.Windows.Visibility.Visible;
                    PhoneScreen.Visibility = System.Windows.Visibility.Hidden;
                    SettingsScreen.Visibility = System.Windows.Visibility.Hidden;
                }
        }


        // Constantly running when a skeleton is detected in front of the sensor
        void Sensor_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            using (var frame = e.OpenSkeletonFrame())
            {
                if (frame != null)
                {
                    Skeleton[] skeletons = new Skeleton[frame.SkeletonArrayLength];
                    frame.CopySkeletonDataTo(skeletons);

                    if (skeletons.Length > 0)   // verifies if there is a skeleton
                    {
                        // Grabs first skeleton that it tracks and makes that the User
                        var user = skeletons.Where(
                                   u => u.TrackingState == SkeletonTrackingState.Tracked).FirstOrDefault();

                        if (user != null)
                        {
                            Console.WriteLine("User Found!"); //Debugging information 
                            JointCollection jointCollection = user.Joints; //Debugging information 
                            Console.WriteLine(jointCollection[JointType.ElbowRight].TrackingState.ToString()); //Debugging
                            Console.WriteLine(jointCollection[JointType.HandRight].TrackingState.ToString()); //Debugging

                            Canvas.SetLeft(ellipseElbowRight, jointCollection[JointType.ElbowRight].Position.X * 300); //Debugging
                            Canvas.SetTop(ellipseElbowRight, jointCollection[JointType.ElbowRight].Position.Y * -300); //Debugging

                            Canvas.SetLeft(ellipseHandRight, jointCollection[JointType.HandRight].Position.X * 300); //Debugging
                            Canvas.SetTop(ellipseHandRight, jointCollection[JointType.HandRight].Position.Y * -300); //Debugging
                            _gesture.Update(user);
                        }
                    }
                }
            }
        } //Closing Sensor_SkeletonFrameReady method

        void Gesture_GestureRecognized(object sender, EventArgs e)
        {
            // Makes sure gridArrayIndex stays within the # of pages. Doesn't increment passed the number of Grids
            if (gridArrayIndex < 4)
            {
                gridArrayIndex++;
                txtCounter.Text = gridArrayIndex.ToString();

                GridWelcome.Visibility = System.Windows.Visibility.Hidden;
                MainScreen.Visibility = System.Windows.Visibility.Hidden;
                AudioScreen.Visibility = System.Windows.Visibility.Hidden;
                ApplicationsScreen.Visibility = System.Windows.Visibility.Hidden;
                PhoneScreen.Visibility = System.Windows.Visibility.Hidden;
                SettingsScreen.Visibility = System.Windows.Visibility.Hidden;
                gridArray[gridArrayIndex].Visibility = System.Windows.Visibility.Visible;
            }
            if (WaveGesture.z == 100)
            {
                gridArrayIndex = 0;
                txtCounter.Text = gridArrayIndex.ToString();

                GridWelcome.Visibility = System.Windows.Visibility.Hidden;
                MainScreen.Visibility = System.Windows.Visibility.Hidden;
                AudioScreen.Visibility = System.Windows.Visibility.Hidden;
                ApplicationsScreen.Visibility = System.Windows.Visibility.Hidden;
                PhoneScreen.Visibility = System.Windows.Visibility.Hidden;
                SettingsScreen.Visibility = System.Windows.Visibility.Hidden;
                gridArray[gridArrayIndex].Visibility = System.Windows.Visibility.Visible;
                WaveGesture.z = 0;
            }
        }
    }
}
