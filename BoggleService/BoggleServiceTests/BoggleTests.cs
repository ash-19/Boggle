// Skeleton provided by Joe Zachary.
//
// Extended by Snehashish Mishra on 29th March for CS 3500 
// offered by The University of Utah, Spring 2016.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Dynamic;
using static System.Net.HttpStatusCode;
using System.Diagnostics;
using Newtonsoft.Json;
using System.IO;
using System.Timers;

namespace Boggle
{
    /// <summary>
    /// Provides a way to start and stop the IIS web server from within the test
    /// cases.  If something prevents the test cases from stopping the web server,
    /// subsequent tests may not work properly until the stray process is killed
    /// manually.
    /// </summary>
    public static class IISAgent
    {
        // Reference to the running process
        private static Process process = null;

        /// <summary>
        /// Starts IIS
        /// </summary>
        public static void Start(string arguments)
        {
            if (process == null)
            {
                ProcessStartInfo info = new ProcessStartInfo(Properties.Resources.IIS_EXECUTABLE, arguments);
                info.WindowStyle = ProcessWindowStyle.Minimized;
                info.UseShellExecute = false;
                process = Process.Start(info);
            }
        }

        /// <summary>
        ///  Stops IIS
        /// </summary>
        public static void Stop()
        {
            if (process != null)
            {
                process.Kill();
            }
        }
    }

    /// <summary>
    /// Test class which provides comprehensive unit tests for the BoggleService API.
    /// </summary>
    [TestClass]
    public class BoggleTests
    {
        /// <summary>
        /// This is automatically run prior to all the tests to start the server
        /// </summary>
        [ClassInitialize()]
        public static void StartIIS(TestContext testContext)
        {
            IISAgent.Start(@"/site:""BoggleService"" /apppool:""Clr4IntegratedAppPool"" /config:""..\..\..\.vs\config\applicationhost.config""");
        }

        /// <summary>
        /// This is automatically run when all tests have completed to stop the server
        /// </summary>
        [ClassCleanup()]
        public static void StopIIS()
        {
            IISAgent.Stop();
        }

        private RestTestClient client = new RestTestClient("http://localhost:60000/");
        //private RestTestClient client = new RestTestClient("http://bogglecs3500s16.azurewebsites.net/");

        /// <summary>
        /// Create a user with valid nickname
        /// </summary>
        [TestMethod]
        public void CreateUser01()
        {
            dynamic user = new ExpandoObject();
            user.Nickname = "Pikachu";
            Response r = client.DoPostAsync("users", user).Result;
            Assert.AreEqual(Created, r.Status);

            // Check the returned UserToken
            Assert.IsTrue(r.Data.ToString() != "");
            Assert.AreEqual(36, r.Data.UserToken.ToString().Length);
        }

        /// <summary>
        /// Create a user with empty nickname
        /// </summary>
        [TestMethod]
        public void CreateUser02()
        {
            dynamic user = new ExpandoObject();
            user.Nickname = "";

            Response r = client.DoPostAsync("users", user).Result;

            // Check the returned UserToken
            Assert.AreEqual(Forbidden, r.Status);
            Assert.IsNull(r.Data);
        }

        /// <summary>
        /// Create a user with only whitespaces nickname
        /// </summary>
        [TestMethod]
        public void CreateUser03()
        {
            dynamic user = new ExpandoObject();
            user.Nickname = "    ";

            Response r = client.DoPostAsync("users", user).Result;

            // Check the returned UserToken
            Assert.AreEqual(Forbidden, r.Status);
            Assert.IsNull(r.Data);
        }

        /// <summary>
        /// Create 2 users same nickname and ensure different user tokens
        /// </summary>
        [TestMethod]
        public void CreateUser04()
        {
            dynamic user = new ExpandoObject();
            user.Nickname = "Test";

            Response r = client.DoPostAsync("users", user).Result;
            Response r1 = client.DoPostAsync("users", user).Result;

            Assert.AreEqual(Created, r.Status);
            Assert.IsTrue(r.Data.ToString() != "");
            Assert.AreEqual(36, r.Data.UserToken.ToString().Length);

            Assert.AreEqual(Created, r1.Status);
            Assert.IsTrue(r1.Data.ToString() != "");
            Assert.AreEqual(36, r1.Data.UserToken.ToString().Length);

            // Check to see that both tokens are unique.
            Assert.AreNotEqual(r.Data.ToString(), r1.Data.ToString());
        }

        /// <summary>
        /// Join a game with a bad user token
        /// </summary>
        [TestMethod]
        public void JoinGame01()
        {
            dynamic joinGame = new ExpandoObject();
            // Does not exist on the sever
            joinGame.UserToken = "9eb536af-50a1-476f-856e-ffff8f1b25d2";
            joinGame.TimeLimit = 30;

            Response r = client.DoPostAsync("games", joinGame).Result;
            Assert.AreEqual(Forbidden, r.Status);

            // Cancel the game join request from the non-existent player
            dynamic cancelJoin = new ExpandoObject();
            cancelJoin.UserToken = joinGame.UserToken;

            r = client.DoPutAsync(cancelJoin, "games").Result;
            Assert.AreEqual(Forbidden, r.Status);
        }

        /// <summary>
        /// Join a game with a bad time limit
        /// </summary>
        [TestMethod]
        public void JoinGame02()
        {
            // Must simulate a valid usertoken
            dynamic user = new ExpandoObject();
            user.Nickname = "Pikachu";

            Response r = client.DoPostAsync("users", user).Result;
            Assert.AreEqual(Created, r.Status);

            // Check the returned UserToken
            Assert.IsTrue(r.Data.ToString() != "");
            Assert.AreEqual(36, r.Data.UserToken.ToString().Length);

            dynamic joinGame = new ExpandoObject();
            // Exists on the sever
            joinGame.UserToken = r.Data.ToString();
            joinGame.TimeLimit = 0;

            Response r1 = client.DoPostAsync("games", joinGame).Result;
            Assert.AreEqual(Forbidden, r1.Status);

            // Cancel the game join request which hasn't been created
            dynamic cancelJoin = new ExpandoObject();
            cancelJoin.UserToken = joinGame.UserToken;

            r = client.DoPutAsync(cancelJoin, "games").Result;
            Assert.AreEqual(Forbidden, r.Status);
        }

        /// <summary>
        /// Try to join a game when the player is already in a pending game.
        /// </summary>
        [TestMethod]
        public void JoinGame03()
        {
            // Make a user - Player 1
            dynamic user = new ExpandoObject();
            user.Nickname = "Pikachu";
            Response r = client.DoPostAsync("users", user).Result;
            Assert.AreEqual(Created, r.Status);

            // Check the returned UserToken
            Assert.IsTrue(r.Data.ToString() != "");
            Assert.AreEqual(36, r.Data.UserToken.ToString().Length);

            // Join Pikachu in a game
            dynamic joinGame = new ExpandoObject();
            joinGame.UserToken = r.Data.UserToken;
            joinGame.TimeLimit = 30;

            Response r1 = client.DoPostAsync("games", joinGame).Result;
            Assert.AreEqual(Accepted, r1.Status);

            // Try to put the same usertoken in a game.
            Response r2 = client.DoPostAsync("games", joinGame).Result;
            Assert.AreEqual(Conflict, r2.Status);

            // Cancel the join request from the pending game
            dynamic cancelJoin = new ExpandoObject();
            cancelJoin.UserToken = joinGame.UserToken;

            r = client.DoPutAsync(cancelJoin, "games").Result;
            Assert.AreEqual(OK, r.Status);
        }

        /// <summary>
        /// Add 2 valid players
        /// </summary>
        [TestMethod]
        public void JoinGame04()
        {
            simulateGame(5);
        }

        /// <summary>
        /// Cancel a join request successfully.
        /// </summary>
        [TestMethod]
        public void CancelJoin01()
        {
            dynamic user = new ExpandoObject();
            user.Nickname = "Pikachu";
            Response r = client.DoPostAsync("users", user).Result;
            Assert.AreEqual(Created, r.Status);

            dynamic newUser = r.Data;

            dynamic joinGame = new ExpandoObject();
            joinGame.UserToken = newUser.UserToken;
            joinGame.TimeLimit = 30;
            r = client.DoPostAsync("games", joinGame).Result;
            Assert.AreEqual(Accepted, r.Status);

            dynamic cancelJoin = new ExpandoObject();
            cancelJoin.UserToken = newUser.UserToken;

            r = client.DoPutAsync(cancelJoin, "games").Result;
            Assert.AreEqual(OK, r.Status);
        }

        /// <summary>
        /// Cancel a join request in failure.
        /// </summary>
        [TestMethod]
        public void CancelJoin02()
        {
            dynamic user = new ExpandoObject();
            user.UserToken = "128";
            Response cancel = client.DoPutAsync(user, "games").Result;

            // Check the returned UserToken
            Assert.AreEqual(Forbidden, cancel.Status);
        }

        /// <summary>
        /// Creates a game and returns a valid usertoken for the game and a gameId
        /// </summary>
        /// <returns>Returns a dictionary with the keys userToken and gameId</returns>
        private Dictionary<string, string> simulateGame(int timeLimit)
        {
            // Make a user - Player 1
            dynamic user = new ExpandoObject();
            user.Nickname = "Pikachu";
            Response r = client.DoPostAsync("users", user).Result;
            Assert.AreEqual(Created, r.Status);
            
            // Check the returned UserToken
            Assert.IsTrue(r.Data.ToString() != "");
            Assert.AreEqual(36, r.Data.UserToken.ToString().Length);

            // Join Pikachu in a game
            dynamic joinGame = new ExpandoObject();
            joinGame.UserToken = r.Data.UserToken;
            joinGame.TimeLimit = timeLimit;

            Response r1 = client.DoPostAsync("games", joinGame).Result;
            Assert.AreEqual(Accepted, r1.Status);

            // Pikachu is waiting for another player

            // Create a new Player2 - Balbasaur
            user = new ExpandoObject();
            user.Nickname = "Balbasaur";
            r = client.DoPostAsync("users", user).Result;
            Assert.AreEqual(Created, r.Status);
            // Check the returned UserToken
            Assert.IsTrue(r.Data.ToString() != "");
            Assert.AreEqual(36, r.Data.UserToken.ToString().Length);


            // Join Balbasaur in a game
            joinGame = new ExpandoObject();
            joinGame.UserToken = r.Data.UserToken;
            joinGame.TimeLimit = timeLimit;

            // Both players in a game
            r1 = client.DoPostAsync("games", joinGame).Result;
            Assert.AreEqual(Created, r1.Status);

            Dictionary<string, string> pairs = new Dictionary<string, string>();

            pairs.Add("userToken", (string) r.Data.UserToken.ToString()); 
            pairs.Add("gameId", r1.Data.GameID.ToString());

            return pairs;
        }

        /// <summary>
        /// Play a word that can't be formed.
        /// </summary>
        [TestMethod]
        public void PlayWord01()
        {
            Dictionary<string, string> gameInfo = simulateGame(30);

            dynamic word = new ExpandoObject();
            word.UserToken = gameInfo["userToken"];
            word.Word = "THIS CAN'T BE FORMED";
            Response r = client.DoPutAsync(word, "games/" + gameInfo["gameId"]).Result;
            Assert.AreEqual(OK, r.Status);
            int score;
            Assert.IsTrue(Int32.TryParse(r.Data.Score.ToString(), out score));
            Assert.IsTrue(score == -1);
        }

        /// <summary>
        /// Play a word which is empty when trimmed
        /// </summary>
        [TestMethod]
        public void PlayWord02()
        {
            Dictionary<string, string> gameInfo = simulateGame(30);

            dynamic word = new ExpandoObject();
            word.UserToken = gameInfo["userToken"];
            word.Word = "         ";
            Response r = client.DoPutAsync(word, "games/" + gameInfo["gameId"]).Result;
            Assert.AreEqual(Forbidden, r.Status);
        }

        /// <summary>
        /// Play a word when using bad game ID
        /// </summary>
        [TestMethod]
        public void PlayWord03()
        {
            Dictionary<string, string> gameInfo = simulateGame(30);

            dynamic word = new ExpandoObject();
            word.UserToken = gameInfo["userToken"];
            word.Word = "word";
            Response r = client.DoPutAsync(word, "games/badGameId").Result;
            Assert.AreEqual(Forbidden, r.Status);
        }

        /// <summary>
        /// Play a word when missing the usertoken
        /// </summary>
        [TestMethod]
        public void PlayWord04()
        {
            Dictionary<string, string> gameInfo = simulateGame(30);

            dynamic word = new ExpandoObject();
            word.UserToken = "";
            word.Word = "test";
            Response r = client.DoPutAsync(word, "games/" + gameInfo["gameId"]).Result;
            Assert.AreEqual(Forbidden, r.Status);
        }

        /// <summary>
        /// Play a word when using an invalid usertoken, not in the game
        /// </summary>
        [TestMethod]
        public void PlayWord05()
        {
            Dictionary<string, string> gameInfo = simulateGame(30);

            dynamic word = new ExpandoObject();
            word.UserToken = "badtokenyo";
            word.Word = "test";
            Response r = client.DoPutAsync(word, "games/" + gameInfo["gameId"]).Result;
            Assert.AreEqual(Forbidden, r.Status);
        }

        /// <summary>
        /// Play a word when the game is pending
        /// </summary>
        [TestMethod]
        public void PlayWord06()
        {
            dynamic user = new ExpandoObject();
            user.Nickname = "Pikachu";
            Response r = client.DoPostAsync("users", user).Result;
            Assert.AreEqual(Created, r.Status);
            // Check the returned UserToken
            Assert.IsTrue(r.Data.ToString() != "");
            Assert.AreEqual(36, r.Data.UserToken.ToString().Length);

            // Make a valid join game request
            dynamic joinGame = new ExpandoObject();
            joinGame.UserToken = r.Data.UserToken;
            joinGame.TimeLimit = 30;

            Response r1 = client.DoPostAsync("games", joinGame).Result;
            Assert.AreEqual(Accepted, r1.Status);

            // Pikachu is waiting for another player

            dynamic word = new ExpandoObject();
            word.UserToken = r.Data.UserToken;
            word.Word = "test";
            r = client.DoPutAsync(word, "games/" + r1.Data.GameID).Result;
            Assert.AreEqual(Conflict, r.Status);

            // Cancel the join request from the pending game
            dynamic cancelJoin = new ExpandoObject();
            cancelJoin.UserToken = joinGame.UserToken;

            r = client.DoPutAsync(cancelJoin, "games").Result;
            Assert.AreEqual(OK, r.Status);
        }

        /// <summary>
        /// Find a word that is valid on the board and play it
        /// </summary>
        [TestMethod]
        public void PlayWord08()
        {
            Dictionary<string, string> gameInfo = simulateGame(120); 
            string[] words = File.ReadAllLines("dictionary.txt");

            Response gameRequest = client.DoGetAsync("games/{0}?Brief={1}", gameInfo["gameId"], "asdf").Result;
            dynamic game = gameRequest.Data;

            BoggleBoard board = new BoggleBoard(game.Board.ToString());

            string word = "";
            foreach(string s in words)
            {
                if(board.CanBeFormed(s))
                {
                    // Found a valid word
                    word = s;
                    break;
                }
            }

            dynamic play = new ExpandoObject();
            play.UserToken = gameInfo["userToken"];
            play.Word = word;

            Response r = client.DoPutAsync(play, "games/" + gameInfo["gameId"]).Result;
            Assert.AreEqual(OK, r.Status);
            dynamic result = r.Data;
            int score;
            Assert.IsTrue(Int32.TryParse(result.Score.ToString(), out score));
            Assert.IsTrue(score >= 0);
        }

        /// <summary>
        /// Try playing the same word twice
        /// </summary>
        [TestMethod]
        public void PlayWord09()
        {
            Dictionary<string, string> gameInfo = simulateGame(30);
            string[] words = File.ReadAllLines("dictionary.txt");
            Response gameRequest = client.DoGetAsync("games/{0}?Brief={1}", gameInfo["gameId"], "asdf").Result;
            dynamic game = gameRequest.Data;

            BoggleBoard board = new BoggleBoard(game.Board.ToString());

            string word = "";
            foreach (string s in words)
            {
                if (board.CanBeFormed(s))
                {
                    // Found a valid word
                    word = s;
                    break;
                }
            }

            dynamic play = new ExpandoObject();
            play.UserToken = gameInfo["userToken"];
            play.Word = word;

            Response r = client.DoPutAsync(play, "games/" + gameInfo["gameId"]).Result;
            Assert.AreEqual(OK, r.Status);
            dynamic result = r.Data;
            int score;
            Assert.IsTrue(Int32.TryParse(result.Score.ToString(), out score));
            Assert.IsTrue(score >= 0);

            r = client.DoPutAsync(play, "games/" + gameInfo["gameId"]).Result;
            Assert.AreEqual(OK, r.Status);
            result = r.Data;
            Assert.IsTrue(Int32.TryParse(result.Score.ToString(), out score));
            Assert.IsTrue(score == 0);
        }

        /// <summary>
        /// Find and play a valid 6 letter word
        /// </summary>
        [TestMethod]
        public void PlayWord10()
        {
            Dictionary<string, string> gameInfo = simulateGame(120);
            string[] words = File.ReadAllLines("dictionary.txt");

            Response gameRequest = client.DoGetAsync("games/{0}?Brief={1}", gameInfo["gameId"], "asdf").Result;
            dynamic game = gameRequest.Data;

            BoggleBoard board = new BoggleBoard(game.Board.ToString());

            string word = "";
            foreach (string s in words)
            {
                if (s.Length == 6 && board.CanBeFormed(s))
                {
                    // Found a valid word
                    word = s;
                    break;
                }
            }

            // Repeat the test with new boards until a 6 letter word can be formed.
            if(word != "")
            {
                dynamic play = new ExpandoObject();
                play.UserToken = gameInfo["userToken"];
                play.Word = word;

                Response r = client.DoPutAsync(play, "games/" + gameInfo["gameId"]).Result;
                Assert.AreEqual(OK, r.Status);
                dynamic result = r.Data;
                int score;
                Assert.IsTrue(Int32.TryParse(result.Score.ToString(), out score));
                Assert.AreEqual(3, score);
            }
            else
            {
                PlayWord10();
            }
            
        }

        /// <summary>
        /// Find and play a valid 7 letter word
        /// </summary>
        [TestMethod]
        public void PlayWord11()
        {
            Dictionary<string, string> gameInfo = simulateGame(30);
            string[] words = File.ReadAllLines("dictionary.txt");

            Response gameRequest = client.DoGetAsync("games/{0}?Brief={1}", gameInfo["gameId"], "asdf").Result;
            dynamic game = gameRequest.Data;

            BoggleBoard board = new BoggleBoard(game.Board.ToString());

            string word = "";
            foreach (string s in words)
            {
                if (s.Length == 7 && board.CanBeFormed(s))
                {
                    // Found a valid word
                    word = s;
                    break;
                }
            }

            // Repeat the test with new boards until a 7 letter word can be formed.
            if (word != "")
            {
                dynamic play = new ExpandoObject();
                play.UserToken = gameInfo["userToken"];
                play.Word = word;

                Response r = client.DoPutAsync(play, "games/" + gameInfo["gameId"]).Result;
                Assert.AreEqual(OK, r.Status);
                dynamic result = r.Data;
                int score;
                Assert.IsTrue(Int32.TryParse(result.Score.ToString(), out score));
                Assert.AreEqual(5, score);
            }
            else
            {
                PlayWord11();
            }
        }

        /// <summary>
        /// Find and play a valid word over 7 letters
        /// </summary>
        [TestMethod]
        public void PlayWord12()
        {
            Dictionary<string, string> gameInfo = simulateGame(30);
            string[] words = File.ReadAllLines("dictionary.txt");

            Response gameRequest = client.DoGetAsync("games/{0}?Brief={1}", gameInfo["gameId"], "asdf").Result;
            dynamic game = gameRequest.Data;

            BoggleBoard board = new BoggleBoard(game.Board.ToString());

            string word = "";
            foreach (string s in words)
            {
                if (s.Length > 7 && board.CanBeFormed(s))
                {
                    // Found a valid word
                    word = s;
                    break;
                }
            }

            // Repeat the test with new boards until a word > 7 letters can be formed.
            if (word != "")
            {
                dynamic play = new ExpandoObject();
                play.UserToken = gameInfo["userToken"];
                play.Word = word;

                Response r = client.DoPutAsync(play, "games/" + gameInfo["gameId"]).Result;
                Assert.AreEqual(OK, r.Status);
                dynamic result = r.Data;
                int score;
                Assert.IsTrue(Int32.TryParse(result.Score.ToString(), out score));
                Assert.AreEqual(11, score);
            }
            else
            {
                PlayWord12();
            }   
        }

        /// <summary>
        /// Find and play a valid word with 5 letters
        /// </summary>
        [TestMethod]
        public void PlayWord13()
        {
            Dictionary<string, string> gameInfo = simulateGame(30);
            string[] words = File.ReadAllLines("dictionary.txt");

            Response gameRequest = client.DoGetAsync("games/{0}?Brief={1}", gameInfo["gameId"], "asdf").Result;
            dynamic game = gameRequest.Data;

            BoggleBoard board = new BoggleBoard(game.Board.ToString());

            string word = "";
            foreach (string s in words)
            {
                if (s.Length == 5 && board.CanBeFormed(s))
                {
                    // Found a valid word
                    word = s;
                    break;
                }
            }

            // Repeat the test with new boards until a word with 5 letters can be formed.
            if (word != "")
            {
                dynamic play = new ExpandoObject();
                play.UserToken = gameInfo["userToken"];
                play.Word = word;

                Response r = client.DoPutAsync(play, "games/" + gameInfo["gameId"]).Result;
                Assert.AreEqual(OK, r.Status);
                dynamic result = r.Data;
                int score;
                Assert.IsTrue(Int32.TryParse(result.Score.ToString(), out score));
                Assert.AreEqual(2, score);
            }
            else
            {
                PlayWord13();
            }
        }

        /// <summary>
        /// Play a word not in the dictionary
        /// </summary>
        [TestMethod]
        public void PlayWord14()
        {
            Dictionary<string, string> gameInfo = simulateGame(30);
            string[] words = File.ReadAllLines("dictionary.txt");

            Response gameRequest = client.DoGetAsync("games/{0}?Brief={1}", gameInfo["gameId"], "asdf").Result;
            dynamic game = gameRequest.Data;

            BoggleBoard board = new BoggleBoard(game.Board.ToString());

            dynamic play = new ExpandoObject();
            play.UserToken = gameInfo["userToken"];
            play.Word = "jkashdjkhfakfhad";

            Response r = client.DoPutAsync(play, "games/" + gameInfo["gameId"]).Result;
            Assert.AreEqual(OK, r.Status);
            dynamic result = r.Data;
            int score;
            Assert.IsTrue(Int32.TryParse(result.Score.ToString(), out score));
            Assert.AreEqual(-1, score);
        }

        /// <summary>
        /// Stress test for the play word method
        /// </summary>
        [TestMethod]
        public void PlayWordStressTest()
        {
            int iterations = 50;
            for(int i = 0; i < iterations; i++)
            {
                PlayWord08();
            }
            for (int i = 0; i < iterations; i++)
            {
                PlayWord09();
            }
        }

        /// <summary>
        /// Play a word which is null
        /// </summary>
        [TestMethod]
        public void PlayWord07()
        {
            Dictionary<string, string> gameInfo = simulateGame(30);

            dynamic word = new ExpandoObject();
            word.UserToken = gameInfo["userToken"];
            word.Word = null;

            Response r = client.DoPutAsync(word, "games/" + gameInfo["gameId"]).Result;
            Assert.AreEqual(Forbidden, r.Status);
        }

        /// <summary>
        /// With brief != yes, game state = active
        /// </summary>
        [TestMethod]
        public void GameStatus01()
        {
            Dictionary<string, string> gameInfo = simulateGame(30);
            Response r = client.DoGetAsync("games/{0}?Brief={1}", gameInfo["gameId"], "asdf").Result;

            Assert.AreEqual(OK, r.Status);
            dynamic game = r.Data;
            Assert.AreEqual("active", game.GameState.ToString());
            Assert.AreEqual(16, game.Board.ToString().Length);
            Assert.AreEqual("30", game.TimeLimit.ToString());

            dynamic player1 = game.Player1;
            Assert.AreEqual("Pikachu", player1.Nickname.ToString());
            Assert.AreEqual("0", player1.Score.ToString());

            dynamic player2 = game.Player2;
            Assert.AreEqual("Balbasaur", player2.Nickname.ToString());
            Assert.AreEqual("0", player2.Score.ToString());
        }

        /// <summary>
        /// Omitting brief
        /// </summary>
        [TestMethod]
        public void GameStatus02()
        {
            Dictionary<string, string> gameInfo = simulateGame(30);
            Response r = client.DoGetAsync("games/" + gameInfo["gameId"]).Result;

            Assert.AreEqual(OK, r.Status);
            dynamic game = r.Data;
            Assert.AreEqual("active", game.GameState.ToString());
            Assert.AreEqual(16, game.Board.ToString().Length);
            Assert.AreEqual("30", game.TimeLimit.ToString());

            dynamic player1 = game.Player1;
            Assert.AreEqual("Pikachu", player1.Nickname.ToString());
            Assert.AreEqual("0", player1.Score.ToString());

            dynamic player2 = game.Player2;
            Assert.AreEqual("Balbasaur", player2.Nickname.ToString());
            Assert.AreEqual("0", player2.Score.ToString());
        }

        /// <summary>
        /// With breif
        /// </summary>
        [TestMethod]
        public void GameStatus03()
        {
            Dictionary<string, string> gameInfo = simulateGame(30);
            Response r = client.DoGetAsync("games/" + gameInfo["gameId"] + "?Brief=yes").Result;

            dynamic game = r.Data;
            Assert.AreEqual("active", game.GameState.ToString());
            try
            {
                string board = game.Board;
                Assert.Fail();
            } catch(Exception)
            {
            }

            try
            {
                string timeLimit = game.TimeLimit;
                Assert.Fail();
            }
            catch (Exception)
            {
            }
            try
            {
                string player1Name = game.Player2.Nickname;
                Assert.Fail();
            }
            catch (Exception)
            {
            }
            try
            {
                string player2Name = game.Player2.Nickname;
                Assert.Fail();
            }
            catch (Exception)
            {
            }

        }

        /// <summary>
        /// Invalid GameID
        /// </summary>
        [TestMethod]
        public void GameStatus04()
        {
            Dictionary<string, string> gameInfo = simulateGame(30);
            Response r = client.DoGetAsync("games/" + "-10").Result;
            Assert.AreEqual(Forbidden, r.Status);
        }

        /// <summary>
        /// Get a game which is pending.
        /// </summary>
        [TestMethod]
        public void GameStatus05()
        {
            // Must simulate a valid usertoken
            dynamic user = new ExpandoObject();
            user.Nickname = "Pikachu";
            Response r = client.DoPostAsync("users", user).Result;
            Assert.AreEqual(Created, r.Status);
            // Check the returned UserToken
            Assert.IsTrue(r.Data.ToString() != "");

            // Make a valid join game request
            dynamic joinGame = new ExpandoObject();
            joinGame.UserToken = r.Data.UserToken;
            joinGame.TimeLimit = 30;

            Response r1 = client.DoPostAsync("games", joinGame).Result;
            Assert.AreEqual(Accepted, r1.Status);

            // Pikachu is waiting for another player
            r = client.DoGetAsync("games/" + r1.Data.GameID).Result;
            Assert.AreEqual(OK, r.Status);

            // Cancel the join request from the pending game
            dynamic cancelJoin = new ExpandoObject();
            cancelJoin.UserToken = joinGame.UserToken;

            r = client.DoPutAsync(cancelJoin, "games").Result;
            Assert.AreEqual(OK, r.Status);
        }

        /// <summary>
        /// brief != yes, and game status is completed.
        /// </summary>
        [TestMethod]
        public void GameStatus06()
        {
            // Start a game with timeLimit 5 seconds and wait for 10 seconds to make 
            // sure its status changes to "completed"
            Dictionary<string, string> gameInfo = simulateGame(5);
            System.Threading.Thread.Sleep(10000);

            Response r = client.DoGetAsync("games/" + gameInfo["gameId"]).Result;

            Assert.AreEqual(OK, r.Status);
            dynamic game = r.Data;
            Assert.AreEqual("completed", game.GameState.ToString());
            Assert.AreEqual(16, game.Board.ToString().Length);
            Assert.AreEqual("5", game.TimeLimit.ToString());

            dynamic player1 = game.Player1;
            Assert.AreEqual("Pikachu", player1.Nickname.ToString());
            Assert.AreEqual("0", player1.Score.ToString());

            dynamic player2 = game.Player2;
            Assert.AreEqual("Balbasaur", player2.Nickname.ToString());
            Assert.AreEqual("0", player2.Score.ToString());
        }

        /// <summary>
        /// Didn't pass a game id
        /// </summary>
        [TestMethod]
        public void GameStatus07()
        {
            Dictionary<string, string> gameInfo = simulateGame(30);
            Response r = client.DoGetAsync("games/").Result;
            Assert.AreEqual(NotFound, r.Status);
        }

        /// <summary>
        /// Tests the api
        /// </summary>
        [TestMethod]
        public void API()
        {
            Response r = new Response();
            try
            {
                r = client.DoGetAsync("api").Result;
                
            } catch(Exception)
            {
            }
            
        }
    }
}
