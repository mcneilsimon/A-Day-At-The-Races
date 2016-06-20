﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace RaceTrackSim
{
    /// <summary>
    /// Represents a client of the betting parlor, placing bets on race hounds winning the race.
    /// This represents the "Guy" class in the textbook lab
    /// </summary>
    public class Bettor
    {
        /// <summary>
        /// The  name of the bettor as shown in the user interface
        /// </summary>
        private string _name;

        /// <summary>
        /// The bet the bettor is placing before a race
        /// </summary>
        private Bet _bet;

        /// <summary>
        /// The amount of cash the bettor has. The bets made are using this cash and can increase
        /// and decrease this amount depending on whether the bettor wins or looses. 
        /// </summary>
        private int _cash;

        /// <summary>
        /// The UI control used to represent the bettor on the form
        /// TODO: PRESENTATION-TIER concern
        /// </summary>
        private RadioButton _uiBettor;

        /// <summary>
        /// The UI control used to show the bet amount this bettor is placing
        /// TODO: PRESENTATION-TIER concern
        /// </summary>
        private TextBlock _uiBetDesc;

        public Bettor(string name, int cash, RadioButton uiBettor, TextBlock uiBetDesc)
        {
            _name = name;
            _bet = new Bet(0, 0, this); //TODO: the bet should not be created here as there is no info but the author assumes that
            _cash = cash;

            //TODO: PRESENTATION - TIER concern
            _uiBettor = uiBettor;
            _uiBetDesc = uiBetDesc;
        }

        /// <summary>
        /// Property to provide access to the name of the bettor
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public Bet Bet
        {
            get { return _bet; }
        }

        public bool HasPlacedBet
        {
            get { return _bet.Amount > 0;  }
        }
        /// <summary>
        /// Clears the bet so that it's set to zero
        /// </summary>
        public void ClearBet()
        {
            //reset the bet value to zero
        }

        /// <summary>
        /// Place a bet with the given amount on the given racing hound
        /// and store it in the appropriate fields
        /// </summary>
        /// <param name="betAmount">the amount to bet</param>
        /// <param name="houndToWin">the hound the bet is on to win</param>
        /// <returns>
        ///     - true: if the bettor has enough money to place to bet
        ///     - false: if the bettor doesn't have enough money and cannot place the bet
        /// </returns>
        public bool PlaceBet(int betAmount, int houndToWin)
        {
            //if the bettor has enough money place the bet
            if (betAmount <= _cash)
            {
                //Place a new bet and store it in the _bet field variable
                _bet = new Bet(betAmount, houndToWin, this);

                //update the UI
                UpdateLabels();

                //inform the caller that the bet was placed
                return true;
            }
            else
            {
                //the bettor doesn't have enough money
                return false;
            }
        }

        /// <summary>
        /// Collects the money won from the bet, clears the bet and 
        /// updates the UI (TODO: latter is a PRESENTATION-TIER concern)
        /// </summary>
        /// <param name="winnerHound"></param>
        public void Collect(int winnerHound)
        {
            //Ask the bet to pay out and update the cash of this bettor
            _cash += _bet.PayOut(winnerHound);

            //Clear the bet
            _bet = new Bet(0, 0, this); //TODO: the bet should not be created here as there is no info but the author assumes that

            //Update the UI (TODO: PRESENTATION-TIER concern)
            UpdateLabels();
        }

        /// <summary>
        /// Updates the UI with the name of the bettor and the description of its bet
        /// </summary>
        public void UpdateLabels()
        {
            //set the label to the bet's description; assumes a bet object exists with zero value even
            //when the bettor hasn't placed a bet. Not the best design but that's what the requirements
            //of the lab are (TODO: Fix this)
            _uiBetDesc.Text = _bet.GetDescription();
            
            //set the radio button to show the bettor
            _uiBettor.Content = $"{_name} has {_cash} bucks";
        }

    }
}