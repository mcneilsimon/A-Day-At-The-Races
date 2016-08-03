using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace RaceTrackLogicLibrary
{
    /// <summary>
    /// Represents the racer which advances a random distance on the race track.
    /// </summary>
    public class Greyhound
    {
        /// <summary>
        /// The position on the x-axis where the UI for the greyhound is positioned.
        /// TODO: PRESENTATION-TIER concern
        /// </summary>
        private double _startPosition;

        /// <summary>
        /// The length of the race track that needs to be traversed by the race hound
        /// </summary>
        private double _raceTrackLength;

        /// <summary>
        /// The current location of the race hound on the track while racing
        /// </summary>
        private double _location;

        /// <summary>
        /// The random number generator used to advance the race hound on the track
        /// using a random distance. All race hounds MUST share the same randomizer 
        /// object which is created and set by the form.By making it static we ensure
        /// that all instaces of Greyhound share the same field variable
        /// </summary>
        private static Random s_randomizer;

        /// <summary>
        /// Constructor reponsible for creating a race hound to participate in the race
        /// </summary>
        /// <param name="raceHoundUI">the UI control responsisble to show the race hound to the user</param>
        public Greyhound(Image raceHoundUI)
        {
            _startPosition = 0;
            _raceTrackLength = 0;
            _location = 0;
            
        }

        /// <summary>
        /// The position on the x-axis where the UI for the greyhound is positioned.
        /// </summary>
        public double StartPosition
        {
            get { return _startPosition; }
            set { _startPosition = value; }
        }

        /// <summary>
        /// The length of the race track that needs to be traversed by the race hound
        /// </summary>
        public double RaceTrackLength
        {
            get { return _raceTrackLength; }
            set {_raceTrackLength = value; }
        }

        /// <summary>
        /// The current location of the race hound on the track while racing
        /// </summary>
        public double Location
        {
            get { return _location; }
            set { _location = value; }
        }

        /// <summary>
        /// The random number generator used to advance the race hound on the track
        /// using a random distance
        /// </summary>
        public static Random Randomizer
        {
            get { return s_randomizer; }
            set { s_randomizer = value; }
        }

        public OnRacerHoundAtStartPositionCallBack OnRaceHoundAtStartPosition { get; set; }

        public OnRaceHoundAdvanceCallback OnRaceHoundAdvance { get; set; }

        /// <summary>
        /// Resets the position of the race hound to the start of the race
        /// </summary>
        public void TakeStartingPosition()
        {
            //Reset the location to zero...
            _location = 0;

            //... and position the Picture Box UI to starting position
            OnRaceHoundAtStartPosition(this, _location);
        }

        /// <summary>
        /// Move a random number of units between 1 and 4 spaces and update the location of 
        /// the race hound on the UI (on the form)
        /// </summary>
        /// <returns>
        ///     true: the race hound has won the race
        ///     false: the race hound has not yet reached the finish line
        /// </returns>
        public bool Run()
        {
            //assume we are not yet at the finish line

            //Calculate the number of spaces to move at random
            int distance = s_randomizer.Next(1, 10);

            //update the location making sure the racer stops at the end of the track and doesn't go past it
            _location += distance;
            if (_location >= _raceTrackLength)
            {
                _location = _raceTrackLength;
               
            }

            OnRaceHoundAdvance(this, _location);

            //Update the position of the UI element, the picture box to reflect the new position
            

            //return true if the racer reached the end of the track and false otherwise
            return _location == _raceTrackLength;
        }
    }
}
