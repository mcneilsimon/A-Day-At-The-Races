﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using RaceTrackLogicLibrary;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace RaceTrackSim
{
    /// <summary>
    /// The race track simulator page where the race takes place. This is the main page of the program
    /// that is launched when the application starts
    /// </summary>
    public sealed partial class MainPage : Page
    {
        /// <summary>
        /// The list of bettors. The form works for up to three bettors
        /// </summary>
        private List<Bettor> _bettorList;

        /// <summary>
        /// The list of race hounds. the form works for up to four bettors
        /// </summary>
        private List<Greyhound> _raceHoundList;

        /// <summary>
        /// List of image controls used to display the greyhound racers. By adding the individual control variables
        /// to a list we can write reusable code using loops to initialize and work with these images.
        /// </summary>
        private List<Image> _racerImageList;

        /// <summary>
        /// The randomizer object created by the form and provided to all the 
        /// race hound objects to be used while racing
        /// </summary>
        private Random _formRandomizer;

        /// <summary>
        /// The currently selected bettor, determined using the radio buttonson the form
        /// </summary>
        private Bettor _crtSelBettor;

        /// <summary>
        /// The description of the bettor for the textblock
        /// </summary>
        private TextBlock _uiBetDesc;


        /// <summary>
        /// Declares radiobutton of bettor and content
        /// </summary>
        private RadioButton _uiBettor;

        /// <summary>
        /// The minimum bet value allowed in the program
        /// </summary>
        private const int MIN_BET_VALUE = 5;

        /// <summary>
        /// Timer that generates a Tick event at a predetermined time interval
        /// </summary>
        private DispatcherTimer  _tmRaceTimer;

        public MainPage()
        {
            //The current bettor is set by the event handler that is triggered when the 
            //radio button is checked, which is done in the following call to 
            //InitializeComponent()
            _crtSelBettor = null;

            this.InitializeComponent();

            //initialize the timer field variable and setup a timer event handler that is called
            //at a given time interval
            _tmRaceTimer = new DispatcherTimer();
            _tmRaceTimer.Tick += OnRaceTimerTick;
            _tmRaceTimer.Interval = TimeSpan.FromMilliseconds(25); //TODO: define a constant field variable for 100 and use it here

            //create the randomizer for all objects of the form
            _formRandomizer = new Random();

            //NOTE it is important for the bettors and race hounds to be created AFTER
            //the controls have been created by the InitializeComponent() method
            CreateBettors();
            CreateRaceHounds();
        }

        /// <summary>
        /// Event handler for the Loaded event of the Page itself. Similar to Load event in WindowsForms. This event is needed
        /// if the code needs valid coordinates and control dimensions that have been calculated after the layout engine has
        /// completed the initial layout. Note that a LayoutUpdated event exists as well.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            //initialize the start position and track length for each racer
            for (int iRacer = 0; iRacer < _raceHoundList.Count; iRacer++)
            {
                Greyhound racer = _raceHoundList[iRacer];
                Image racerImg = _racerImageList[iRacer];

                //set the start position for the racehound racer
                racer.StartPosition = Canvas.GetLeft(racerImg);

                //calculate and set the race track distance between the front of the dog (which is the end or right coordinate of the image)
                //and the end of the track
                double racerImgRightCoord = Canvas.GetLeft(racerImg) + racerImg.ActualWidth;
                racer.RaceTrackLength = _raceTrackCanvas.ActualWidth - racerImgRightCoord;
            }
        }

        /// <summary>
        /// Creates the list of race hounds and the greyhound objects in it. Each greyhound
        /// object is linked with the control it works with per the LAB's requirements
        /// </summary>
        private void CreateRaceHounds()
        {
            //create the list of greyhounds 
            _raceHoundList = new List<Greyhound>();
            _racerImageList = new List<Image>();

            //initialize all greyhound instances with the same shared randomizer object
            Greyhound.Randomizer = _formRandomizer;

            //creates the four grey hounds, initialize each object and add it to the list
            _raceHoundList.Add(new Greyhound(_imgRacingHound1));
            _racerImageList.Add(_imgRacingHound1);

            _raceHoundList.Add(new Greyhound(_imgRacingHound2));
            _racerImageList.Add(_imgRacingHound2);

            //Using object initializers uses in practice the indentation below
            _raceHoundList.Add(new Greyhound(_imgRacingHound3));
            _racerImageList.Add(_imgRacingHound3);

            _raceHoundList.Add(new Greyhound(_imgRacingHound4));
            _racerImageList.Add(_imgRacingHound4);
        }

        /// <summary>
        /// Creates the list of bettors and the bettor objects in it. Each bettor object
        /// is linked with the control it works with per the LAB's requirement.
        /// </summary>
        private void CreateBettors()
        {
            //create the list of bettors
            _bettorList = new List<Bettor>();

            //create the three bettor objects and initialize their properties
            _bettorList.Add(new Bettor("Joe", 50));
            _bettorList.Add(new Bettor("Bob", 75));
            _bettorList.Add(new Bettor("Anna", 45));

            //initialize the minimum bet label as required using the minimum value for the
            //bet value control
            _txtMinBet.Text = $"Minimum bet is {MIN_BET_VALUE} dollars.";

            //update all the bettor information to initial value 
            _txtBet1.Text = DetermineBetDescUi(_bettorList[0]).Text;
            _txtBet2.Text = DetermineBetDescUi(_bettorList[1]).Text;
            _txtBet3.Text = DetermineBetDescUi(_bettorList[2]).Text;

            

            //update all bettor information to inital cash value for radioButton content
            foreach(Bettor bettorUI in _bettorList)
            {
                DetermineBettorUi(bettorUI);
            }

        }
        
        /// <summary>
        /// Event handler when the user selects the current bettor using one of the
        /// three radio buttons. The same event handler is shared by all three radio 
        /// buttons to avoid code duplication
        /// </summary>
        /// <param name="sender">one of the three radio buttons that represent the bettors</param>
        /// <param name="e">not used</param>
        private void OnBettorSelectorChecked(object sender, RoutedEventArgs e)
        {
            //check the sender of the event and select the correct bettor in the list
            //alternatively you can use the Tag property of the radio buttons by setting
            //it to the appropriate bettor object when the objects are created and here use
            //only extract the correct bettor from the Tag property value. See the final branch
            //for this solution
            if (sender == _rbtnBettor1)
            {
                _crtSelBettor = _bettorList[0];
            }
            else if (sender == _rbtnBettor2)
            {
                _crtSelBettor = _bettorList[1];
            }
            else if (sender == _rbtnBettor3)
            {
                _crtSelBettor = _bettorList[2];
            }
            else
            {
                Debug.Assert(false, "Unexpected bettor selector control. Cannot select the current bettor");
                return;
            }

            //use the current bettor information to display the bet information
            _txtCrtBettorName.Text = $"{_crtSelBettor.Name} bets";

        }

        /// <summary>
        /// Event handler raised when the user places the bet for the current bettor
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnPlaceBet(object sender, RoutedEventArgs e)
        {
            try
            {
                //Obtain the bet amount
                int betAmount = int.Parse(_txtBetAmount.Text);

                //Determine the racer the bet is on
                ComboBoxItem racerItem = _cmbRaceHoundNo.SelectedItem as ComboBoxItem;
               
                int racerNo = int.Parse(racerItem.Content.ToString());

                //Ask the current bettor to place a bet

                //Calls PlaceBet method and returns an int value
                int switchValue = _crtSelBettor.PlaceBet(betAmount, racerNo);

                //Switch statements to determine exception error handling or to continue to update labels.
                switch (switchValue)
                {

                    //Checks if bet is less than 5
                    case 0:
                        throw new InvalidOperationException("Bet amount must be greater than or equal to 5");
                    
                    //User defined exception that checks if the bettor has enough money
                    case 1:

                        throw new InsufficientFundsException($"{_crtSelBettor.Name} does not have enough money to place this bet.");
             
                    //updates the bet labels changing bettor name and the amount they bet. Also says that a bettor has now placed a bet. 
                    case 2:
                        UpdateBettorLabels(_crtSelBettor);
                        break;              
                }
            }

            //The follwoing are the catch operators for the previous exception try statements
            catch (InvalidOperationException ioEx)
            {
                _txtBetAmount.Text = "N/A";
                MessageDialog msgDlg = new MessageDialog(ioEx.Message);
                await msgDlg.ShowAsync();
            }

            catch (InsufficientFundsException ifEx)
            {
                _txtBetAmount.Text = "N/A";
                MessageDialog msgDlg = new MessageDialog(ifEx.Message);
                await msgDlg.ShowAsync();
            }

            catch (FormatException)
            {
                _txtBetAmount.Text = "N/A";

                MessageDialog msgDlg = new MessageDialog("Invalid Input\nPlease enter a certain amount of money within your balanace.");
                await msgDlg.ShowAsync();
            }

            catch (NullReferenceException)
            {
                MessageDialog msgDlg = new MessageDialog("Please select a better and place a bet before placing bets");
                await msgDlg.ShowAsync();
            }

        }

        /// <summary>
        /// Event handler raised when the user clicks on the Start Race button. Starts
        /// the race timer to get the race hounds to move
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnStartRace(object sender, RoutedEventArgs e) 
        {
            //checks to see if one bettor has atleast placed a bet before running the program 
            try
            {
                if(_bettorList[0].HasPlacedBet || _bettorList[1].HasPlacedBet || _bettorList[2].HasPlacedBet)
                {
                    //if one player has at least place one bet, loop will run
                    foreach (Greyhound racer in _raceHoundList)
                    {
                        racer.TakeStartingPosition();
                    }
                    _tmRaceTimer.Start();
                }

                else
                {
                    throw new InvalidOperationException("Place at least one bet before starting the race");
                }
            
            }

            catch (InvalidOperationException ioEx)
            {
                MessageDialog msgDlg = new MessageDialog(ioEx.Message);
                await msgDlg.ShowAsync();
            }
        }

        /// <summary>
        /// Race timer that is called at a rate determined using the _tmRaceTimer.Interval.
        /// </summary>
        /// <param name="sender">the timer object _tmRaceTimer</param>
        /// <param name="e">not used</param>
        private async void OnRaceTimerTick(object sender, object e)
        {
            //repeat going through each dog and ask them to run. The dog will let us
            //know if they won
            for (int racerNo = 1; racerNo <= _raceHoundList.Count; racerNo++)
            {
                Greyhound racer = _raceHoundList[racerNo - 1];

                //ask the racer to run and check if they won
                if (racer.Run())
                {
                    //the racer crossed the finish line and won                   
                    //stop the timer to stop the race
                    _tmRaceTimer.Stop();

                    //let the user know who won
                    MessageDialog msgDlg = new MessageDialog($"The Greyhound #{racerNo} won the race!");
                    await msgDlg.ShowAsync();

                    //collect the winnings
                    foreach (Bettor bettor in _bettorList)
                    {
                        //check whether the bettor has a bet to collect. The app works without it
                        //because the bettor has a bet with the amount zero on dog zero which doesn't
                        //affect the calculation. We fix this issue in the HasPlacedBet method by
                        //checking if the bettor HasPlacedAbit and if returns true then it collects 
                        //bettors winnings or loosing 
                        if (bettor.HasPlacedBet)
                        {
                            bettor.Collect(racerNo);
                            DetermineBettorUi(bettor);

                            bettor.ClearBet();

                            if(bettor.Name == "Joe")
                            {
                                _txtBet1.Text = DetermineBetDescUi(bettor).Text;
                            }
                            else if(bettor.Name == "Bob")
                            {
                                _txtBet2.Text = DetermineBetDescUi(bettor).Text;
                            }
                            else if(bettor.Name == "Anna")
                            {
                                _txtBet3.Text = DetermineBetDescUi(bettor).Text;
                            }

                            else
                            {
                                Debug.Assert(false, "Unexpected bettor selector control. Cannot select the current bettor");
                                return;
                            }
                 
                        }  
                    }
                    break;
                }
            }
        }

        /// <summary>
        /// Checks Bet description and sets it to initial value when program runs
        /// and then updates after a bettor has placed a bet
        /// </summary>
        /// <param name="bettor"></param>
        /// <returns></returns>
        private TextBlock DetermineBetDescUi(Bettor bettor)
        { 
            _uiBetDesc = new TextBlock();

            //checks if a bettor has placed a bet and then updates bettors label
            if(bettor.HasPlacedBet == true)
            {
                _uiBetDesc.Text = $"{bettor.Name} bets {_txtBetAmount.Text}$ on dog #{_cmbRaceHoundNo.SelectedIndex+1}";
                return _uiBetDesc;
            }

            {
                _uiBetDesc.Text = $"{bettor.Name} hasn't placed a bet";
                return _uiBetDesc;
            }
        }

        /// <summary>
        /// updates radio buttons content
        /// Checks which radio button content to update by checking bettors name 
        /// or viewing which radio button has been clicked.
        /// </summary>
        /// <param name="bettor"></param>
        /// <returns></returns>
        private RadioButton DetermineBettorUi(Bettor bettor)
        {

            if ((bool)_rbtnBettor1.IsChecked || bettor.Name == "Joe")
            {
                _rbtnBettor1.Content = $"{bettor.Name} has {bettor.Cash} bucks";
                _uiBettor = _rbtnBettor1;
                return _uiBettor;
            }

            else if ((bool)_rbtnBettor2.IsChecked || bettor.Name == "Bob")
            {
                _rbtnBettor2.Content = $"{bettor.Name} has {bettor.Cash} bucks";
                _uiBettor = _rbtnBettor2;
                return _uiBettor;
            }

            else if ((bool)_rbtnBettor3.IsChecked || bettor.Name == "Anna")
            {
                _rbtnBettor3.Content = $"{bettor.Name} has {bettor.Cash} bucks";
                return _uiBettor = _rbtnBettor3;
            }

            else
            {
                Debug.Assert(false, "Unexpected bettor selector control. Cannot select the current bettor");
                return null;
            }
        }

        /// <summary>
        /// the follwoing calls DetermineBettorUi and checks which radiobutton has been selected to update
        /// it equivalent _txtBet label
        /// </summary>

        private void UpdateBettorLabels(Bettor bettor)
        {

            RadioButton radioBettor = DetermineBettorUi(bettor);

            if (radioBettor == _rbtnBettor1)
            {
                _txtBet1.Text = DetermineBetDescUi(bettor).Text;
            }

            else if(radioBettor == _rbtnBettor2)
            {
                _txtBet2.Text = DetermineBetDescUi(bettor).Text;
            }

            else if(radioBettor == _rbtnBettor3)
            {
                _txtBet3.Text = DetermineBetDescUi(bettor).Text;
            }
        }     
        /// <summary>
        /// Event handler called when the user selects a race hound to place a bet on. Used to
        /// enable or disable the Place Bet button depending on whether an actual race hound
        /// has been selected or not
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnGreyhoundRacerSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Enable the Place Bet button if the user has selected a racer to bet on and disable it if not
            _btnPlaceBet.IsEnabled = (_cmbRaceHoundNo.SelectedItem != null);

        }

    }
}
