using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

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
            _bettorList.Add(new Bettor("Joe", 50, _rbtnBettor1, _txtBet1));
            _bettorList.Add(new Bettor("Bob", 75, _rbtnBettor2, _txtBet2));
            _bettorList.Add(new Bettor("Anna", 45, _rbtnBettor3, _txtBet3));

            //initialize the minimum bet label as required using the minimum value for the
            //bet value control
            _txtMinBet.Text = $"Minimum bet is {MIN_BET_VALUE} dollars.";

            //update all the bettor information
            foreach (Bettor bettor in _bettorList)
            {
                bettor.UpdateLabels();
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

            //use a conditional expression rather an an if/else statement to determine the bet amount (https://msdn.microsoft.com/en-us/library/ty67wk28.aspx)
            int betAmount = _crtSelBettor.HasPlacedBet ? _crtSelBettor.Bet.Amount : MIN_BET_VALUE;
            _txtBetAmount.Text = betAmount.ToString();

            //show the racer the selected bettor has bet on if any (combo box items start at 0 so #RACER HOUND - 1
            _cmbRaceHoundNo.SelectedIndex = _crtSelBettor.HasPlacedBet ? _crtSelBettor.Bet.RaceHound - 1 : -1;
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

                int switchValue = _crtSelBettor.PlaceBet(betAmount, racerNo);

                switch (switchValue)
                {

                    case 0:
                        throw new InvalidOperationException("Bet amount must be greater than or equal to 5");

                    case 1:
                        MessageDialog msgDlg = new MessageDialog($"{_crtSelBettor.Name} does not have enough money to place this bet.",
                                                               "Race Track Simulator");
                        await msgDlg.ShowAsync();
                        break;

                    case 2:
                        break;                 
                }
            }

            catch (InvalidOperationException ioEx)
            {
                _txtBetAmount.Text = "N/A";
                MessageDialog msgDlg = new MessageDialog(ioEx.Message);
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
            try
            {
                if(_bettorList[0].HasPlacedBet || _bettorList[1].HasPlacedBet || _bettorList[2].HasPlacedBet)
                {
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
                        //affect the calculation. Logically though we should not try to collect from
                        //a bettor that has not placed a bet. This causes an actual problem with the 
                        //final implementation where the _bet is set to null in the bettor when the
                        //bettor doesn't have a bet
                        if (bettor.HasPlacedBet)
                        {
                            bettor.Collect(racerNo);
                        }
                    }

                    //TODO: What happens if there is a tie? Debug through this scenario and 
                    //plan and/or implement a good and fair solution
                    break;
                }
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
