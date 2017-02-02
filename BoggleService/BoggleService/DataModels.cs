// Developed by Snehashish Mishra (u0946268) on 29th March for CS 3500 
// offered by The University of Utah, Spring 2016.

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Boggle
{
    /// <summary>
    /// Contains all the info indentifying a player 
    /// in a Boggle game.
    /// </summary>
    public class PlayerInfo
    {
        /// <summary>
        /// Every player has a nickname registered.
        /// </summary>
        public string Nickname { get; set; }

        /// <summary>
        /// Every player has total points scored in a 
        /// game.
        /// </summary>
        public int Score { get; set; }

        /// <summary>
        /// Every player has a unique userID
        /// </summary>
        public string UserToken { get; set; }

        /// <summary>
        /// The ID of the current game this player is a part off. 
        /// (the state of the game needs to be either pending or active and 
        /// never completed).
        /// </summary>
        public string GameID { get; set; }
    }

    /// <summary>
    /// Contains info used when making a Join or cancel 
    /// game request.
    /// </summary>
    public class JoinGameInfo
    {
        /// <summary>
        /// Token of user joining game.
        /// </summary>
        public string UserToken { get; set; }

        /// <summary>
        /// Requested time limit.
        /// </summary>
        public int TimeLimit { get; set; }
    }

    /// <summary>
    /// Includes all the info regarding the state of a 
    /// Boggle game (like 
    /// </summary>
    public class GameInfo
    {
        /// <summary>
        /// The current state of a game. Could be 
        /// "active", "pending", "completed"
        /// </summary>
        public string GameState { get; set; }

        /// <summary>
        /// Unique identifier for this game
        /// </summary>
        public string GameID { get; set; }

        /// <summary>
        /// Contains the board being used for this game. 
        /// Is a string of 16 characters where Q should be 
        /// treated as QU.
        /// </summary>
        public string Board { get; set; }

        /// <summary>
        /// The average of the time limits chosen by both the 
        /// player of this game.
        /// </summary>
        public int TimeLimit { get; set; }

        /// <summary>
        /// The time left before this game ends.
        /// </summary>
        public int TimeLeft { get; set; }

        /// <summary>
        /// The time that this game went active.
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// The first player's userToken in this game.
        /// </summary>
        public string Player1Token { get; set; }

        /// <summary>
        /// The second player's userToken in this game.
        /// </summary>
        public string Player2Token { get; set; }
    }

    /// <summary>
    /// Contains all the info used for a word play request.
    /// </summary>
    public class WordInfo
    {
        /// <summary>
        /// Token of user playing the word.
        /// </summary>
        public string UserToken { get; set; }

        /// <summary>
        /// Identifies the game in which the word was played.
        /// </summary>
        public string GameID { get; set; }

        /// <summary>
        /// Word being played.
        /// </summary>
        public string Word { get; set; }
    }

    /// <summary>
    /// Object that is returned from a create user request
    /// </summary>
    public class CreateUserReturnInfo
    {
        /// <summary>
        /// The users token.
        /// </summary>
        public string UserToken { get; set; }
    }

    /// <summary>
    /// Object to be returned when a join game request is made.
    /// </summary>
    public class JoinGameReturnInfo
    {
        /// <summary>
        /// GameID of the game
        /// </summary>
        public string GameID { set; get; }

    }

    /// <summary>
    /// Returned when a client plays a word
    /// </summary>
    public class PlayWordReturnInfo
    {
        /// <summary>
        /// Score for the word
        /// </summary>
        public string Score { set; get; }
    }

    /// <summary>
    /// Returned by the game status method
    /// </summary>
    public class GameStatusReturnInfo
    {
        /// <summary>
        /// Game state of the game
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string GameState { set; get; }
        /// <summary>
        /// Board of the game
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Board { set; get; }
        /// <summary>
        /// Time limit set for the game
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? TimeLimit { set; get; }
        /// <summary>
        /// Time left in the game
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? TimeLeft { set; get; }
        /// <summary>
        /// Player 1
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public PlayerGameStatusReturnInfo Player1 { set; get; }
        /// <summary>
        /// Player 2
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public PlayerGameStatusReturnInfo Player2 { set; get; }
    }

    /// <summary>
    /// Returned inside of a game status call
    /// </summary>
    public class PlayerGameStatusReturnInfo
    {
        /// <summary>
        /// Nickname of the player
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Nickname { set; get; }
        /// <summary>
        /// Score of the player
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? Score { set; get; }

        /// <summary>
        /// A dictionary containing all the words played by this 
        /// player mapping the word played (Key) to its score 
        /// (Value).
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public WordPlayedReturnInfo[] WordsPlayed;
    }

    /// <summary>
    /// Returned with game status call
    /// </summary>
    public class WordPlayedReturnInfo
    {
        /// <summary>
        /// Word played
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Word { set; get; }

        /// <summary>
        /// Scored of the word played
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? Score { set; get; }
    }
}