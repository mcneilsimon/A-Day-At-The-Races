using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaceTrackLogicLibrary
{
    /// <summary>
    /// Represents the bet made by a bettor and determines the payout when the bet is a win
    /// </summary>
    public class Bet
    {
        /// <summary>
        /// The amount of cash that was bet
        /// </summary>
        private int _amount;

        /// <summary>
        /// The number of the race hound the bet is on
        /// </summary>
        private int _raceHound;

        /// <summary>
        /// The bettor object that has placed (owns) this bet. This is a very interesting 
        /// relationship between this object and the "parent" object that created it
        /// </summary>
        private Bettor _bettor;

        public Bet(int amount, int raceHound, Bettor bettor)
        {
            _amount = amount;
            _raceHound = raceHound;
            _bettor = bettor;
        }

        /// <summary>
        /// Read-only property to provide the bet amount. The bet amount cannot be changed
        /// once the bet was made
        /// </summary>
        public int Amount
        {
            get { return _amount; }
        }

        /// <summary>
        /// Read-only property for the race hound the bet was on. Once the bet is made
        /// the race hound the bet is for cannot be changed
        /// </summary>
        public int RaceHound
        {
            get { return _raceHound; }
        }

        public int PayOut(int winnerNo)
        {
            //check the winner and return the amount positive or negative depending on whether
            //the bet was win or a bust
            if (_raceHound == winnerNo)
            {
                return _amount;
            }
            else
            {
                return -_amount;
            }
            
        }

    }
}
