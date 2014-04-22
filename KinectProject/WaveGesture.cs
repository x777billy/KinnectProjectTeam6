using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;

namespace KinectProject
{
    class WaveGesture
    {
        //simple gestures that last for approximately a second, a window size of 30 or 50 will work
        readonly int WINDOW_SIZE = 50; //Number of frames for that gesture to last

        IGestureSegment[] _segments1;
        IGestureSegment[] _segments3;
        //IGestureSegment[] _segments2;
        int _currentSegment1 = 0;
        //int _currentSegment2 = 0;
        int _frameCount1 = 0; //number of frames we ask for data is called window size
        //int _frameCount2 = 0;
        public static int z;

        public event EventHandler GestureRecognized;

        public WaveGesture()
        {
            WaveSegment1 waveSegment1 = new WaveSegment1(); //Hanlde Hand right of the elbow
            WaveSegment2 waveSegment2 = new WaveSegment2(); //Handle Hand left of the elbow
            HandOverHead raisedHand = new HandOverHead();
            //LeftSwipeSegment1 leftSwipe1 = new LeftSwipeSegment1();
            //LeftSwipeSegment2 leftSwipe2 = new LeftSwipeSegment2();

            //Gesture parts and specify their order in the the _segments array - waving
            _segments1 = new IGestureSegment[] { waveSegment1, waveSegment2 };
            _segments3 = new IGestureSegment[] { raisedHand };
            //_segments2 = new IGestureSegment[] { leftSwipe1, leftSwipe2 };
        }

        public void Update(Skeleton skeleton)
        {
            //check every segment 
            GesturePartResult result1 = _segments1[_currentSegment1].Update(skeleton);
            GesturePartResult result3 = _segments3[0].Update(skeleton);
            //GesturePartResult result2 = _segments2[_currentSegment2].Update(skeleton);

            //check every segment for success 
            if (result1 == GesturePartResult.Succeeded)
            {
                if (_currentSegment1 + 1 < _segments1.Length)
                {
                    _currentSegment1++;  //Go to the next gesture part to check
                    _frameCount1 = 0;
                }
                else
                {
                    if (GestureRecognized != null)
                    {
                        GestureRecognized(this, new EventArgs());  //Gesture found
                        Reset1(); //Start the gesture over
                    }
                }
            }
            // Goes to the Main Screen if hand is raised over head
            if (result3 == GesturePartResult.ThirdOption)
            {
                z = 100;
            }
            ///////////////////////////////////////////////////////////////////
            //if (result2 == GesturePartResult.Succeeded)
            //{
            //    if (_currentSegment2 + 1 < _segments2.Length)
            //    {
            //        _currentSegment2++;  //Go to the next gesture part to check
            //        _frameCount2 = 0;
            //    }
            //    else
            //    {
            //        if (GestureRecognized != null)
            //        {
            //            z = 100;
            //            GestureRecognized(this, new EventArgs());  //Gesture found
            //            Reset2(); //Start the gesture over
            //        }
            //    }
            //}
            /////////////////////////////////////////////////////////////////////////
            else if (result1 == GesturePartResult.Failed || _frameCount1 == WINDOW_SIZE)
            {
                Reset1();  //Start the gesture over
            }
            //////////////////////////////////////////////////////////////////////////
            //else if (result2 == GesturePartResult.Failed || _frameCount2 == WINDOW_SIZE)
            //{
            //    Reset2();  //Start the gesture over
            //}
            ////////////////////////////////////////////////////////////////////////////
            else
            {
                _frameCount1++;  //Count the frames
                //_frameCount2++;         // IDK WHAT THIS MEANS
            }
            //////////////////////////////////////////////////////////////////////////
        }

        public void Reset1()
        {
            _currentSegment1 = 0;
            _frameCount1 = 0;
        }

        //public void Reset2()
        //{
        //    _currentSegment2 = 0;
        //    _frameCount2 = 0;
        //}
    }
}
