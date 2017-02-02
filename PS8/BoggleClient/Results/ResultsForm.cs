// Developed by Snehashish Mishra (u0946268) on 24th March for
// CS 3500 offered by The University of Utah, Spring 2016.

using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace BoggleClient
{
    /// <summary>
    /// Partial class containing methods and events for manipulating 
    /// the ResultsForm window. Implements the IResultsForm interface.
    /// </summary>
    public partial class ResultsForm : Form, IResultsForm
    {
        /// <summary>
        /// Sets the username of the winner
        /// </summary>
        public string WinnerName
        {
            set  {  winnerLabel.Text = value;  }
        }

        /// <summary>
        /// Sets the home player label
        /// </summary>
        public string HomePlayerName
        {
            set  {  homePlayerResultsLabel.Text = value; }
        }

        /// <summary>
        /// Sets the opponent player label
        /// </summary>
        public string OpponentPlayerName
        {
            set { remotePlayerResultsLabel.Text = value; }
        }

        /// <summary>
        /// Sets the home player score label
        /// </summary>
        public string HomePlayerTotalScore
        {
            set { homePlayerTotalScoreLabel.Text = value; }
        }

        /// <summary>
        /// Sets the opponent score label
        /// </summary>
        public string OpponentPlayerTotalScore
        {
            set { remotePlayerTotalScoreLabel.Text = value; }
        }

        /// <summary>
        /// Sets the text for the done button
        /// </summary>
        public string DoneButtonLabel
        {
            set { doneButtonResults.Text = value; }
        }

        /// <summary>
        /// List of words played by the home player
        /// </summary>
        public List<string> HomePlayerWordsPlayedList
        {
            set { homePlayerWordListBox.DataSource = value; }
        }

        /// <summary>
        /// List of words played by the opponent player
        /// </summary>
        public List<string> OpponentPlayerWordsPlayedList
        {
            set { remotePlayerWordListBox.DataSource = value; }
        }

        /// <summary>
        /// List of scores per word for the home player
        /// </summary>
        public List<string> HomePlayerScoresList
        {
            set { homePlayerScoresListBox.DataSource = value; }
        }

        /// <summary>
        /// List of scores per word for the opponent player
        /// </summary>
        public List<string> OpponentPlayerScoresList
        {
            set { remotePlayerScoresListBox.DataSource = value; }
        }

        /// <summary>
        /// Indicates whether this form is closing.
        /// </summary>
        private bool closing;

        /// <summary>
        /// Provides the event when the user is finished 
        /// looking at the results and clicks the done button 
        /// or presses the return key
        /// </summary>
        public event Action DoneViewingResults;

        /// <summary>
        /// Initializes this form
        /// </summary>
        public ResultsForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// If the done button is clicked, fires an event to indicate the user 
        /// has done interacting with the results windows and thus close it 
        /// and open a new Matchmaking window.
        /// </summary>
        /// <param name="sender">The default sender</param>
        /// <param name="e">The default event arguments</param>
        private void doneButtonResults_Click(object sender, EventArgs e)
        {
            if (DoneViewingResults != null)
            {
                DoneViewingResults();
            }
        }

        /// <summary>
        /// If the return key is pressed, fires an event to indicate the user 
        /// has done interacting with the results windows and thus close it 
        /// and open a new Matchmaking window.
        /// </summary>
        /// <param name="sender">The default sender</param>
        /// <param name="e">Contains the Key pressed</param>
        private void doneButtonResults_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar.Equals('\r'))
            {
                doneButtonResults.PerformClick();
            }
        }

        /// <summary>
        /// Closes this form.
        /// </summary>
        public void DoClose()
        {
            Close();
        }

        /// <summary>
        /// shows this form.
        /// </summary>
        public void Present()
        {
            Show();
        }

        /// <summary>
        /// Fired when the x button is clicked.
        /// </summary>
        private void ResultsForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (!closing)
            {
                closing = true;
                doneButtonResults.PerformClick();
            }
        }
    }
}
