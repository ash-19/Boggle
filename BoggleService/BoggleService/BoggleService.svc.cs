// Developed by Snehashish Mishra on 29th March for CS 3500 
// offered by The University of Utah, Spring 2016.

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Dynamic;
using System.IO;
using System.Net;
using System.ServiceModel.Web;
using static System.Net.HttpStatusCode;

namespace Boggle
{
    /// <summary>
    /// Provides implementations of the operations belonging to this 
    /// service (BoggleService).
    /// </summary>
    public class BoggleService : IBoggleService
    {

        /// <summary>
        /// The connection string to the DB
        /// </summary>
        private static string BoggleDB;

        /// <summary>
        /// Words provided by the dictionary.
        /// </summary>
        private HashSet<string> words = new HashSet<string>();

        // Begin implementations of the operations of this service

        /// <summary>
        /// Creates database connection
        /// </summary>
        static BoggleService()
        {
            BoggleDB = ConfigurationManager.ConnectionStrings["BoggleDB"].ConnectionString;
        }

        /// <summary>
        /// Creates a new user.
        /// (1) If Nickname is null, or is empty when trimmed, responds with status 
        ///     403 (Forbidden).
        /// (2) Otherwise, creates a new user with a unique UserToken and the trimmed 
        ///     Nickname. The returned UserToken should be used to identify the user 
        ///     in subsequent requests. Responds with status 201 (Created).
        /// </summary>
        /// <param name="user">Nickname of the user</param>
        /// <returns>The UserToken. Null if invalid user name.</returns>
        public CreateUserReturnInfo CreateUser(PlayerInfo user)
        {

            string playerName = user.Nickname.Trim();
            if (String.IsNullOrEmpty(playerName) || playerName.Length > 50)
            {
                SetStatus(Forbidden);
                return null;
            }

            using (SqlConnection conn = new SqlConnection(BoggleDB))
            {
                conn.Open();

                using (SqlTransaction trans = conn.BeginTransaction())
                {

                    using (SqlCommand command = new SqlCommand("insert into Users (UserID, Nickname) values(@UserID, @Nickname)", conn, trans))
                    {

                        string userID = Guid.NewGuid().ToString();

                        // This is where the placeholders are replaced.
                        command.Parameters.AddWithValue("@UserID", userID);
                        command.Parameters.AddWithValue("@Nickname", playerName);

                        command.ExecuteNonQuery();
                        SetStatus(Created);

                        trans.Commit();

                        CreateUserReturnInfo info = new CreateUserReturnInfo();
                        info.UserToken = userID;
                        return info;
                    }
                 }
            }    
        }

        /// <summary>
        /// Join a game.
        /// 
        /// (1) If UserToken is invalid, TimeLimit is less than 5, or greater than 120, 
        ///     responds with status 403 (Forbidden). 
        /// (2) Otherwise, if UserToken is already a player in the pending game, responds 
        ///     with status 409 (Conflict).
        /// (3) Otherwise, if there is already one player in the pending game, adds UserToken 
        ///     as the second player. The pending game becomes active. The active game's time limit 
        ///     is the integer average of the time limits requested by the two players. Returns 
        ///     the new active game's GameID (which should be the same as the old pending game's 
        ///     GameID). Responds with status 201 (Created).
        /// (4) Otherwise, adds UserToken as the first player of the pending game, and the 
        ///     TimeLimit as the pending game's requested time limit. Returns the pending game's 
        ///     GameID. Responds with status 202 (Accepted).
        /// </summary>
        /// <param name="game">Contains the body of the request</param>
        /// <returns>The created game's ID. Null if invalid userToken or timelimit.</returns>
        public JoinGameReturnInfo JoinGame(JoinGameInfo game)
        {
            // Check valid time limit, and ensure that the usertoken is in the Users DB
            if(game.TimeLimit < 5 || game.TimeLimit > 120 || IsUserTokenInvalid(game.UserToken))
            {
                SetStatus(Forbidden);
                return null;
            }

            // Else if already a part of a pending game, set status to conflict and 
            // return null
            if (UserIsInPendingGame(game.UserToken))
            {
                SetStatus(Conflict);
                return null;
            }

            // Else if there is pending game that this player is not a part of, adds UserToken 
            // as the second player. The pending game becomes active. The active game's time limit 
            // is the integer average of the time limits requested by the two players. Returns 
            // the new active game's GameID (which should be the same as the old pending game's 
            // GameID). Responds with status 201 (Created).
            int foundPendingGameID = FindPendingGame(game.UserToken);

            if (foundPendingGameID != -1)
            {
                int activeGameID = MakeAnActiveGame(game.UserToken, game.TimeLimit, foundPendingGameID);
                
                    SetStatus(Created);
                    JoinGameReturnInfo info = new JoinGameReturnInfo();
                    info.GameID = activeGameID.ToString();
                    return info;   
            }

            // Otherwise, add this player as the first player of the pending game, and set the
            // TimeLimit as the pending game's requested time limit. Set the status and return 
            // the pending game's GameID. 
            else
            {
                int pendingGameID = CreatePendingGame(game.UserToken, game.TimeLimit);
                SetStatus(Accepted);
                JoinGameReturnInfo info = new JoinGameReturnInfo();
                info.GameID = pendingGameID.ToString();
                return info;
            }
        }

        /// <summary>
        /// Creates a new pending game with the player1 as the passed player 
        /// and the timeLimit as the passed timelimit.
        /// </summary>
        /// <param name="userToken">The id of player</param>
        /// <param name="timeLimit">The time limit requested by player</param>
        /// <returns>The game id of this pending game</returns>
        private int CreatePendingGame(string userToken, int timeLimit)
        {
            using (SqlConnection conn = new SqlConnection(BoggleDB))
            {
                conn.Open();

                using (SqlTransaction trans = conn.BeginTransaction())
                {

                    using (SqlCommand command = new SqlCommand("insert into Games (Player1, TimeLimit) output inserted.GameID values(@PlayerID, @TimeLimit)", conn, trans))
                    {
                        command.Parameters.AddWithValue("@PlayerID", userToken);
                        command.Parameters.AddWithValue("@TimeLimit", timeLimit);

                        // Retrieve the game id of this insertion and return it
                        int gameID = (int) command.ExecuteScalar();
                        
                        trans.Commit();
                        return gameID;
                    }
                }
            }
        }

        /// <summary>
        /// Makes the passed pending game active by adding the passed player 
        /// as player 2, the timelimits, etc.
        /// </summary>
        /// <param name="userToken">The id of player 2</param>
        /// <param name="timeLimit">The time limit requested by player 2</param>
        /// <param name="gameID">The ID of the game to be made active</param>
        /// <returns>The game id of this game (same as pending one)</returns>
        private int MakeAnActiveGame(string userToken, int timeLimit, int gameID)
        {
            using (SqlConnection conn = new SqlConnection(BoggleDB))
            {
                conn.Open();

                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    // Make the game with passed GameID active
                    using (SqlCommand cmd = new SqlCommand("update Games set Player2 = @PlayerID, TimeLimit = (Games.TimeLimit + @TimeLimit)/2, Board = @Board, StartTime = @StartTime where GameID = @GameID", conn, trans))
                    {
                        cmd.Parameters.AddWithValue("@PlayerID", userToken);
                        cmd.Parameters.AddWithValue("@TimeLimit", timeLimit);
                        cmd.Parameters.AddWithValue("@Board", new BoggleBoard().ToString());
                        cmd.Parameters.AddWithValue("@StartTime", DateTime.Now);
                        cmd.Parameters.AddWithValue("@GameID", gameID);

                        // Check if 1 row was modified
                        if (cmd.ExecuteNonQuery() == 1)
                        {
                            trans.Commit();
                            return gameID;
                        }
                        else
                        {
                            trans.Commit();
                            return -1;
                        }        
                    }
                }
            }
        }

        /// <summary>
        /// Finds a pending game which the passed player is not a part of.
        /// </summary>
        /// <param name="userToken">The id of the player</param>
        /// <returns>The game id of the pending game. -1 if no such game</returns>
        private int FindPendingGame(string userToken)
        {
            int gameID = -1;
            using (SqlConnection conn = new SqlConnection(BoggleDB))
            {
                conn.Open();

                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    // Find a game with a null player2 and player1 not the same the passed player2
                    using (SqlCommand cmd = new SqlCommand("select * from Games where Player2 is NULL AND Player1 != @PlayerID", conn, trans))
                    {
                        cmd.Parameters.AddWithValue("@PlayerID", userToken);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            // No pending games
                            if (!reader.HasRows)
                            {
                                return gameID;
                            }
                            // Return the 1st pending game's id
                            while (reader.Read())
                            {
                                gameID = (int) reader["GameID"];
                                break;                             
                            }
                            return gameID;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Cancel a pending request to join a game.
        /// 
        /// (1) If UserToken is invalid or is not a player in the pending game, 
        ///     responds with status 403 (Forbidden).
        /// (2) Otherwise, removes UserToken from the pending game and responds 
        ///     with status 200 (OK).
        /// </summary>
        /// <param name="player">The object containing UserToken</param>
        /// <returns>Only the status response</returns>
        public void CancelJoin(PlayerInfo player)
        {
            // Make sure that the usertoken is 36 chars and ensure the usertoken is a valid user
            if (player.UserToken.Trim().Length != 36 || IsUserTokenInvalid(player.UserToken))
            {
                SetStatus(Forbidden);
            }

            using (SqlConnection conn = new SqlConnection(BoggleDB))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    // Remove the passed player from any pending games
                    using (SqlCommand command = new SqlCommand("delete from Games where Player1 = @UserID", conn, trans))
                    {
                        command.Parameters.AddWithValue("@UserID", player.UserToken);
                        if (command.ExecuteNonQuery() == 0)
                        {
                            // Not in a pending game
                            SetStatus(Forbidden);
                        }
                        else
                        {
                            // Removed from the pending game.
                            SetStatus(OK);
                        }
                        trans.Commit();
                    }
                }
            }
        }

        /// <summary>
        /// Play a word in a game.
        /// 
        /// (1) If Word is null or empty when trimmed, or if GameID or UserToken is 
        ///     missing or invalid, or if UserToken is not a player in the game 
        ///     identified by GameID, responds with response code 403 (Forbidden).
        /// (2) Otherwise, if the game state is anything other than "active", responds 
        ///     with response code 409 (Conflict).
        /// (3) Otherwise, records the trimmed Word as being played by UserToken in the 
        ///     game identified by GameID. Returns the score for Word in the context of 
        ///     the game (e.g. if Word has been played before the score is zero). Responds 
        ///     with status 200 (OK). Note: The word is not case sensitive.
        /// </summary>
        /// <param name="wordPlayed">Contains the body of the request</param>
        /// <param name="GameID">ID of the game</param>
        /// <returns>The score of the played word</returns>
        public PlayWordReturnInfo PlayWord(string GameID, WordInfo wordPlayed)
        {
            // Check the validity
            GameInfo currentGameInfo = GetGameInfo(GameID);
            bool isInvalidObject = (wordPlayed.Word == null || wordPlayed.UserToken == null || 
                                        GameID == null || currentGameInfo == null);

            // If the passed object contains non-null elements, begin processing it
            if (!isInvalidObject)
            {
                string word = wordPlayed.Word.Trim().ToUpper();

                // If Word is null or empty when trimmed, or if GameID or UserToken is 
                // missing or invalid, or if UserToken is not a player in the game identified 
                // by GameID, responds with response code 403 (Forbidden).
                if (String.IsNullOrEmpty(word) || String.IsNullOrWhiteSpace(word) || 
                     currentGameInfo == null || String.IsNullOrEmpty(GameID) ||
                    (currentGameInfo.Player1Token != wordPlayed.UserToken &&
                     currentGameInfo.Player2Token != wordPlayed.UserToken))
                {
                    SetStatus(Forbidden);
                    return null;
                }

                // Otherwise, if the game state is anything other than "active", responds 
                // with response code 409 (Conflict).
                else if (currentGameInfo.GameState != "active")
                {
                    SetStatus(Conflict);
                    return null;
                }

                // Otherwise, records the trimmed Word as being played by this player in this 
                // game identified by the GameID.
                else
                {
                    // Form a new board from the game's board and retrieve the player's info
                    BoggleBoard board = new BoggleBoard(currentGameInfo.Board);

                    // If the word was already played, add it as an attempt and 
                    // return the score is 0 (since duplicate word).
                    if ( WasWordPlayed(wordPlayed.UserToken, GameID, wordPlayed.Word) )
                    {
                        SetStatus(OK);
                        AddWordToDB(wordPlayed.Word, wordPlayed.UserToken, GameID, 0);
                        PlayWordReturnInfo info = new PlayWordReturnInfo();
                        info.Score = "0";
                        return info;
                    }

                    // Now, if the word was never played before, first ensure that the 
                    // word is valid on the board. If so, add the word to DB and return 
                    // this play's score.
                    else if (board.CanBeFormed(wordPlayed.Word))
                    {
                        // Compute the score and add it to the Words DB as the player's play 
                        // in this game identified the passed GameID
                        int score = scoreWord(wordPlayed.Word, GameID, wordPlayed.UserToken);
                        AddWordToDB(wordPlayed.Word, wordPlayed.UserToken, GameID, score);
                        
                        SetStatus(OK);
                        PlayWordReturnInfo info = new PlayWordReturnInfo();
                        info.Score = score + "";
                        return info;
                    }

                    // Otherwise, this never-before-played word cannot be formed on this 
                    // game's board. As such, add this word to DB and return this play's 
                    // score which will be -1.
                    else
                    {
                        AddWordToDB(wordPlayed.Word, wordPlayed.UserToken, GameID, -1);

                        SetStatus(OK);
                        PlayWordReturnInfo info = new PlayWordReturnInfo();
                        info.Score = "-1";
                        return info;
                    }               
                }
            }

            // The dynamic object sent with the PUT request contained a null object
            else
            {
                SetStatus(Forbidden);
                return null;
            }
        }

        /// <summary>
        /// If a word has fewer than three characters scores zero points
        /// If the word is a duplicate scores zero points
        /// If a word is 3 or 4 letters long scores 1 point
        /// If a word is 5 letters long scores 2 points
        /// If a word is 6 letters long scores 3 points
        /// If a word is 7 letters long scores 5 points
        /// If a word is over 7 letters long scores 11 points
        /// If a word is not in the dictionary scores -1
        /// </summary>
        /// <param name="word">Word to be scored</param>
        /// <param name="userToken">ID of player playing the word</param>
        /// <param name="gameID">ID of the game</param>
        /// <returns>The score for the given word.</returns>
        private int scoreWord(string word, string gameID, string userToken)
        {
            // Word has fewer than 3 characters or is a duplicate
            if (word.Length < 3 || WasWordPlayed(userToken, gameID, word))
            {
                return 0;
            }

            // Build the dictionary
            if (words.Count == 0)
            {
                string[] lines = File.ReadAllLines("dictionary.txt");
                words = new HashSet<string>(lines);
            }

            // Check it the word is in the dictionary. If it is, return the 
            // score as per rules. Else return -1.
            if (words.Contains(word))
            {
                if (word.Length == 3 || word.Length == 4) { return 1; }
                else if (word.Length == 5) { return 2; }
                else if (word.Length == 6) { return 3; }
                else if (word.Length == 7) { return 5; }
                else
                {
                    return 11;
                }
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// Get game status information.
        /// 
        /// (1) If GameID is invalid, responds with status 403 (Forbidden).
        /// (2) Otherwise, returns information about the game named by GameID as illustrated 
        ///     below. Note that the information returned depends on whether "Brief=yes" was 
        ///     included as a parameter as well as on the state of the game. Responds with 
        ///     status code 200 (OK). Note: The Board and Words are not case sensitive.
        /// </summary>
        /// <param name="GameID">ID of the game whose status is being requested.</param>
        /// <param name="brief">
        /// (Optional) "yes" means brief status, anything else means full status</param>
        /// <returns>The status of the game (brief or full)</returns>
        public GameStatusReturnInfo GameStatus(string GameID, string brief)
        {
            GameInfo gameStatus = GetGameInfo(GameID);
            string player1Nickname, player2Nickname = "";
            int player1Score, player2Score = 0;

            // Invalid GameID (since GetGameInfo couldn't retrieve the row info of GameID).
            if (gameStatus == null)
            {
                SetStatus(Forbidden);
                return null;
            }

            // Otherwise, returns information about the game named by GameID. The level of 
            // information returned depends on whether "Brief=yes" was included as a 
            // parameter as well as on the state of the game.
            else
            {
                // Reinitialize the GameInfo object to get the latest state associated with the 
                // passed GameID and the approriate objects to return
                gameStatus = GetGameInfo(GameID);
                GameStatusReturnInfo status = new GameStatusReturnInfo();
                PlayerGameStatusReturnInfo player1Object = new PlayerGameStatusReturnInfo();
                PlayerGameStatusReturnInfo player2Object = new PlayerGameStatusReturnInfo();

                // Add the game state to the state object (to be returned), retrieve player 
                // nicknames and scores
                status.GameState = gameStatus.GameState;
                player1Nickname = GetPlayerNickname(gameStatus.Player1Token);
                player1Score = GetTotalPlayerScore(gameStatus.Player1Token, GameID);

                if (gameStatus.GameState != "pending")
                {
                    player2Nickname = GetPlayerNickname(gameStatus.Player2Token);
                    player2Score = GetTotalPlayerScore(gameStatus.Player2Token, GameID);
                }

                // Info to return if the GameState is "pending"
                if (gameStatus.GameState == "pending")
                {
                    SetStatus(OK);
                    return status;
                }

                // Info to return if the GameState is active/completed and brief == yes
                else if (brief == "yes")
                {
                    // Create the Player info objects containing only their total scores.
                    player1Object.Score = player1Score;
                    player2Object.Score = player2Score;

                    // Add the timeLeft and player info object to the final status return object
                    status.TimeLeft = gameStatus.TimeLeft;
                    status.Player1 = player1Object;
                    status.Player2 = player2Object;
                    if (status.TimeLeft <= 0)
                    {
                        status.GameState = "completed";
                    }

                    SetStatus(OK);
                    return status;
                }

                // Info to return if brief != yes and GameState is "active"
                else if (brief != "yes" && gameStatus.GameState == "active")
                {
                    // Add the NickName and total Score to each player's object
                    player1Object.Nickname = player1Nickname;
                    player1Object.Score = player1Score;

                    player2Object.Nickname = player2Nickname;
                    player2Object.Score = player2Score;

                    // Add the remaining relevant info to the final status object
                    status.Board = gameStatus.Board;
                    status.TimeLimit = gameStatus.TimeLimit;
                    status.TimeLeft = gameStatus.TimeLeft;

                    // If the timeLeft > 0 (i.e, game is still active, return this object).
                    // Else, fall through and return the object as per the brief != yes, 
                    // GameState = "completed" scheme.
                    if (status.TimeLeft > 0)
                    {
                        status.Player1 = player1Object;
                        status.Player2 = player2Object;

                        SetStatus(OK);
                        return status;
                    }
                }

                // Otherwsie, the game has completed. Return the status info as per the scheme 
                // for brief != yes, GameState = "completed". 

                // Create and fill an array of WordsPlayed and its scores for both the players.
                List<WordPlayedReturnInfo> player1PlayedWords = new List<WordPlayedReturnInfo>();
                List<WordPlayedReturnInfo> player2PlayedWords = new List<WordPlayedReturnInfo>();

                player1PlayedWords = GetWordsPlayedList(gameStatus.Player1Token, GameID);
                player2PlayedWords = GetWordsPlayedList(gameStatus.Player2Token, GameID);

                // Fill each player's info objects
                player1Object.Nickname = player1Nickname;
                player1Object.Score = player1Score;
                player1Object.WordsPlayed = player1PlayedWords.ToArray();

                player2Object.Nickname = player2Nickname;
                player2Object.Score = player2Score;
                player2Object.WordsPlayed = player2PlayedWords.ToArray();

                // Add the remaining relevant info to the final status return object
                status.Board = gameStatus.Board;
                status.TimeLimit = gameStatus.TimeLimit;
                status.TimeLeft = 0;
                status.Player1 = player1Object;
                status.Player2 = player2Object;
                status.GameState = "completed";

                SetStatus(OK);
                return status;
            }
        }

        /// <summary>
        /// Calculate how many seconds are left in the game based on dateStarted
        /// </summary>
        /// <param name="dateStarted">The date/time that the game was started</param>
        /// <param name="timeLimit">Time limit for the game</param>
        /// <returns>How many seconds are left in the game</returns>
        private int calculateTimeRemaining(DateTime dateStarted, int timeLimit)
        {
            // Difference in seconds between the two times
            int secondsSinceGameStarted = (int) (DateTime.Now - dateStarted).TotalSeconds;

            return timeLimit - secondsSinceGameStarted;
        }

        /// <summary>
        /// The most recent call to SetStatus determines the response code used when
        /// an http response is sent.
        /// </summary>
        /// <param name="status"></param>
        private static void SetStatus(HttpStatusCode status)
        {
            WebOperationContext.Current.OutgoingResponse.StatusCode = status;
        }

        /// <summary>
        /// Returns a Stream version of index.html.
        /// </summary>
        /// <returns></returns>
        public Stream API()
        {
            SetStatus(OK);
            WebOperationContext.Current.OutgoingResponse.ContentType = "text/html";
            return File.OpenRead(AppDomain.CurrentDomain.BaseDirectory + "index.html");
        }

        /// <summary>
        /// A helper to check the validity of the passed user token.
        /// </summary>
        /// <param name="userToken">The user token</param>
        /// <returns>true if invalid, false if valid.</returns>
        private bool IsUserTokenInvalid(string userToken)
        {
            // Check if the usertoken is in the users table
            using (SqlConnection conn = new SqlConnection(BoggleDB))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {

                    using (SqlCommand command = new SqlCommand("select UserID from Users where UserID = @UserID", conn, trans))
                    {
                        command.Parameters.AddWithValue("@UserID", userToken);

                        // Execute the command and check if any such users exist.
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (!reader.HasRows)
                            {
                                return true;
                            }
                            return false;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Checks if the user token is currently in a game.
        /// </summary>
        /// <param name="userToken">The user token</param>
        /// <returns>Returns true if the user token is a play in a game.</returns>
        private bool UserIsInPendingGame(string userToken)
        {
            using (SqlConnection conn = new SqlConnection(BoggleDB))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {

                    using (SqlCommand command = new SqlCommand("select Player1 from Games where Player1 = @UserID", conn, trans))
                    {
                        command.Parameters.AddWithValue("@UserID", userToken);

                        // Execute the command and check if the passed user is in any 
                        // pending games. Return true if it does, false othewise
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (!reader.HasRows)
                            {
                                return false;
                            }
                            return true;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Returns a GameInfo object containing all the columns elements in 
        /// the DB associated with the passed GameID row. Null if no such 
        /// game with the passed gameid is found the DB.
        /// </summary>
        /// <param name="passedGameID">The id of the game to be found</param>
        /// <returns>The GameInfo object. Null if no such game with the passed 
        /// gameID.</returns>
        private GameInfo GetGameInfo(string passedGameID)
        {
            GameInfo foundGameInfo = new GameInfo();

            using (SqlConnection conn = new SqlConnection(BoggleDB))
            {
                conn.Open();

                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    // Find the game with the passed game id
                    using (SqlCommand cmd = new SqlCommand("select * from Games where Games.GameID = @GameID", conn, trans))
                    {
                        cmd.Parameters.AddWithValue("@GameID", passedGameID);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            // No game with the given game id
                            if (!reader.HasRows)
                            {
                                return null;
                            }

                            // Else it has found the game with the given gameID, make and return the GameInfo object
                            while (reader.Read())
                            {
                                string computedGameState;

                                int computedTimeLeft = 0;

                                // Find the state of the game : active, pending, completed
                                if (!reader.IsDBNull(reader.GetOrdinal("Player2")))
                                {
                                    computedTimeLeft = calculateTimeRemaining((DateTime)reader["StartTime"],
                                                                (int)reader["TimeLimit"]);
                                    if (computedTimeLeft > 0)
                                    {
                                        computedGameState = "active";
                                    }
                                    else
                                    {
                                        computedGameState = "completed";
                                    } 
                                }
                                else
                                {
                                    computedGameState = "pending";
                                    // Create the GameInfo object containing info on this game
                                    foundGameInfo = new GameInfo
                                    {
                                        GameID = passedGameID,
                                        TimeLimit = (int)reader["TimeLimit"],
                                        Player1Token = (string)reader["Player1"],
                                        GameState = computedGameState
                                    };
                                    return foundGameInfo;
                                }
                                
                                // Create the GameInfo object containing info on this game
                                foundGameInfo = new GameInfo
                                {
                                    GameID = passedGameID,
                                    Board = (string)reader["Board"],
                                    TimeLimit = (int)reader["TimeLimit"],
                                    TimeLeft = computedTimeLeft,
                                    Player1Token = (string)reader["Player1"],
                                    Player2Token = (string)reader["Player2"],
                                    GameState = computedGameState
                                };
                            }
                            return foundGameInfo;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Returns the nickname of the player associated with this userToken.
        /// </summary>
        /// <param name="userToken">The id of the player</param>
        /// <returns>The nickname of the player. Empty string of no such player 
        /// found.</returns>
        private string GetPlayerNickname(string userToken)
        {
            string nickname = "";
            using (SqlConnection conn = new SqlConnection(BoggleDB))
            {
                conn.Open();

                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    // Get the nickname of the player with the passed userToken
                    using (SqlCommand cmd = new SqlCommand("select * from Users where UserID = @PlayerID", conn, trans))
                    {
                        cmd.Parameters.AddWithValue("@PlayerID", userToken);

                        // Check if any row was returned (1 row containing the nickname)
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            // If no such player exists, return an empty string. Else return its nickname
                            if (!reader.HasRows)
                            {
                                return "";
                            }
                            while(reader.Read())
                            {
                                nickname = reader["Nickname"].ToString();
                            }
                            return nickname;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Computes and returns the total score from all the words played by 
        /// the passed player in the passed game.
        /// </summary>
        /// <param name="userToken">The id of the player</param>
        /// <param name="passedGameID">The id of the game</param>
        /// <returns>The total score of this player in this game</returns>
        private int GetTotalPlayerScore(string userToken, string passedGameID)
        {
            int totalScore = 0;
            using (SqlConnection conn = new SqlConnection(BoggleDB))
            {
                conn.Open();

                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    // Compute the score of all the words played by this player in this game
                    using (SqlCommand cmd = new SqlCommand("select Sum(Score) as TotalScore from Words where Player = @PlayerID AND GameID = @GameID", conn, trans))
                    {
                        cmd.Parameters.AddWithValue("@PlayerID", userToken);
                        cmd.Parameters.AddWithValue("@GameID", passedGameID);

                        // Get the value and return it. Return 0 of a pending game.
                        try { Convert.ToInt32(cmd.ExecuteScalar()); }
                        catch(InvalidCastException) { totalScore = 0; }
                        return totalScore;
                    }
                }
            }
        }

        /// <summary>
        /// Creates and returns a list of all the words played by the passed player in 
        /// the passed game. The list element are the object containing a single word and 
        /// its score.
        /// </summary>
        /// <param name="userToken">The id of the player</param>
        /// <param name="passedGameID">The id of the game</param>
        /// <returns>A list of words played with scores. An empty list if no words were 
        /// found for this player in this game in the DB.</returns>
        private List<WordPlayedReturnInfo> GetWordsPlayedList(string userToken, string passedGameID)
        {
            using (SqlConnection conn = new SqlConnection(BoggleDB))
            {
                conn.Open();

                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    // Find all the words played with scores by the passed player in the passed game
                    using (SqlCommand cmd = new SqlCommand("select Word, Score from Words where Player = @PlayerID AND GameID = @GameID", conn, trans))
                    {
                        cmd.Parameters.AddWithValue("@PlayerID", userToken);
                        cmd.Parameters.AddWithValue("@GameID", passedGameID);

                        List<WordPlayedReturnInfo> playerPlayedWords = new List<WordPlayedReturnInfo>();

                        // Execute the command and obtain the result
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            // No words were played
                            if (!reader.HasRows)
                            {
                                return playerPlayedWords;
                            }

                            // Multiple rows
                            while (reader.Read())
                            {
                                // Extract each word and score pair, encapsulate it in a object and 
                                // add it to the list.
                                playerPlayedWords.Add(new WordPlayedReturnInfo
                                {
                                    Word = (string) reader["Word"],
                                    Score = (int) reader["Score"],
                                });
                            }
                            return playerPlayedWords;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Returns whether the passed word was played before by the passed player, 
        /// in the passed game.
        /// </summary>
        /// <param name="userToken">The id of the player</param>
        /// <param name="passedGameID">The id of the game</param>
        /// <param name="word">The word to be found</param>
        /// <returns>true if word is in DB for this player in this game. False, otherwise</returns>
        private bool WasWordPlayed(string userToken, string passedGameID, string word)
        {
            using (SqlConnection conn = new SqlConnection(BoggleDB))
            {
                conn.Open();

                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    // Find word played by passed player in the passed game
                    using (SqlCommand cmd = new SqlCommand("select * from Words where (Player = @PlayerID) AND (GameID = @GameID) AND (Word = @Word)", conn, trans))
                    {
                        cmd.Parameters.AddWithValue("@GameID", passedGameID);
                        cmd.Parameters.AddWithValue("@PlayerID", userToken);
                        cmd.Parameters.AddWithValue("@Word", word);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            // The passed word was not played
                            if (!reader.HasRows)
                            {
                                return false;
                            }
                            return true;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Adds the passed word in the Words DB.
        /// </summary>
        /// <param name="word">The word to be added</param>
        /// <param name="userToken">The id of player who played this word</param>
        /// <param name="gameID">The id of the game in which this word was played</param>
        /// <param name="score">The score of this word</param>
        /// <returns>True if word was added. False otherwise</returns>
        private bool AddWordToDB(string word, string userToken, string gameID, int score)
        {
            using (SqlConnection conn = new SqlConnection(BoggleDB))
            {
                // Connections must be opened
                conn.Open();

                using (SqlTransaction trans = conn.BeginTransaction())
                {

                    using (SqlCommand command = new SqlCommand("insert into Words (Word, GameID, Player, Score) values(@Word, @GameID, @PlayerID, @Score)", conn, trans))
                    {
                        command.Parameters.AddWithValue("@Word", word);
                        command.Parameters.AddWithValue("@GameID", gameID);
                        command.Parameters.AddWithValue("@PlayerID", userToken);
                        command.Parameters.AddWithValue("@Score", score);

                        // Check if exactly 1 row was added. If so, commit the transaction and 
                        // return true. Else, discard the transaction and return false.
                        int numRows = command.ExecuteNonQuery();
                        if(numRows == 1)
                        {
                            trans.Commit();
                            return true;
                        }
                        trans.Commit();
                        return false;
                    }
                }
            }
        }

        /// <summary>
        /// Generates a unique ID.
        /// </summary>
        /// <returns>Returns a unique ID</returns>
        private string GenerateID()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
