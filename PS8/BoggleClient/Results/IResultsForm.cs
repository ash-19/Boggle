// Developed by Snehashish Mishra (u0946268) on 24th March for
// CS 3500 offered by The University of Utah, Spring 2016.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoggleClient
{
    /// <summary>
    /// The interface providing access to the view (ResultsForm) 
    /// for the ResultsController.
    /// </summary>
    interface IResultsForm
    {
        /// <summary>
        /// Provides the event when the user is finished 
        /// looking at the results and clicks the done button 
        /// or presses the return key
        /// </summary>
        event Action DoneViewingResults;

        /// <summary>
        /// Sets the label of the winner
        /// </summary>
        string WinnerName { set; }

        /// <summary>
        /// Sets the label of the home player
        /// </summary>
        string HomePlayerName { set; }

        /// <summary>
        /// Sets the label of the opponent player
        /// </summary>
        string OpponentPlayerName { set; }

        /// <summary>
        /// Sets the label of the home player's total score
        /// </summary>
        string HomePlayerTotalScore { set; }

        /// <summary>
        /// Sets the label of the opponent player's total score
        /// </summary>
        string OpponentPlayerTotalScore { set; }

        /// <summary>
        /// Sets the text of the done button
        /// </summary>
        string DoneButtonLabel { set; }

        /// <summary>
        /// Fills the home player's list with the words played
        /// </summary>
        List<string> HomePlayerWordsPlayedList { set; }

        /// <summary>
        /// Fills the opponent's list with the words played
        /// </summary>
        List<string> OpponentPlayerWordsPlayedList { set; }

        /// <summary>
        /// Fills the home player's individual words play scores list
        /// </summary>
        List<string> HomePlayerScoresList { set; }

        /// <summary>
        /// Fills the opponent's individual words play scores list
        /// </summary>
        List<string> OpponentPlayerScoresList { set; }

        /// <summary>
        /// Handles closing a boggle game.
        /// </summary>
        void DoClose();

        /// <summary>
        /// Presents the gui.
        /// </summary>
        void Present();
    }
}
