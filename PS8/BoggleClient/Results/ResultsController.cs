// Developed by Snehashish Mishra (u0946268) on 24th March for
// CS 3500 offered by The University of Utah, Spring 2016.

using System;
using System.Windows.Forms;

namespace BoggleClient
{
    /// <summary>
    /// Controls the ResultsForm (View) along with a result model.
    /// </summary>
    class ResultsController
    {
        /// <summary>
        /// Game to provide information to the result view
        /// </summary>
        private BoggleGame game;
        
        /// <summary>
        /// Interface to access view from
        /// </summary>
        private IResultsForm window;

        /// <summary>
        /// Constructs a controller with a window and a game. Draw 
        /// the GUI based on the data gathered and received. Also, 
        /// registers an event handler that performs necessary tasks 
        /// once the user has done viewing results (Close window and 
        /// open Matchmaking form for a new game).
        /// </summary>
        public ResultsController(IResultsForm window, BoggleGame game)
        {
            this.window = window;
            this.game = game;
            window.DoneViewingResults += HandleCloseResults;
            DrawUI();
        }

        /// <summary>
        /// Closes this form and opens a new Matchmaking window to 
        /// allow for joining in a new game.
        /// </summary>
        private void HandleCloseResults()
        {
            // Close this ResultsForm window
            window.DoClose();
            
            MatchmakingForm matchmakingWindow = new MatchmakingForm();
            MatchmakerController controller = new MatchmakerController(matchmakingWindow);
            matchmakingWindow.Show();
        }

        /// <summary>
        /// Sets the contents of the ResultsForm (GUI)
        /// </summary>
        private void DrawUI()
        {
            // Set the UI labels
            window.HomePlayerName = game.HomePlayer.Nickname;
            window.OpponentPlayerName = game.OpponentPlayer.Nickname;
            window.HomePlayerTotalScore = game.HomePlayer.Score.ToString();
            window.OpponentPlayerTotalScore = game.OpponentPlayer.Score.ToString();
            window.WinnerName = FindTheWinner(game.HomePlayer.Score.ToString(), 
                                                game.OpponentPlayer.Score.ToString());

            // Set the word-play and scores list for both the players
            window.HomePlayerWordsPlayedList = game.HomePlayerWordsPlayedList;
            window.HomePlayerScoresList = game.HomePlayerScoresList;
            window.OpponentPlayerWordsPlayedList = game.OpponentWordsPlayedList;
            window.OpponentPlayerScoresList = game.OpponentScoresList;
        }

        /// <summary>
        /// A helper method to determine the winner of the game based on the total 
        /// scores of both the players. Also sets the done button according to the 
        /// winner. </summary>
        /// 
        /// <param name="homePlayerTotalScore">Player 1's total score</param>
        /// <param name="opponentTotalScore">Player 2's total score</param>
        /// 
        /// <returns>
        /// The nickname of the winning player or a draw result.
        /// </returns>
        private string FindTheWinner(string homePlayerTotalScore, string opponentTotalScore)
        {
            int player1Score = Int32.Parse(homePlayerTotalScore);
            int player2Score = Int32.Parse(opponentTotalScore);

            if ( player1Score > player2Score )
            {
                window.DoneButtonLabel = "Sweet!";
                return game.HomePlayer.Nickname + " Wins!";
            }
            else if ( player1Score < player2Score )
            {
                window.DoneButtonLabel = "Oh well...";
                return game.OpponentPlayer.Nickname + " Wins!";
            }
            else
            {
                window.DoneButtonLabel = "Sweet...";
                return "It's a draw!";
            }
        }
    }
}
