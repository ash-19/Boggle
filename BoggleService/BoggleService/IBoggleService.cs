// Skeleton provided by Joe Zachary.
//
// Extended by Snehashish Mishra on 29th March for CS 3500 
// offered by The University of Utah, Spring 2016.

using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Boggle
{
    /// <summary>
    /// This interface defines a collection of operations provided by 
    /// BoggleService.svc. Each method that is annotated as with [WebGet] 
    /// or [WebInvoke] will be exposed by the service. As such, it implements 
    /// the BoggleAPI used by the boggle game clients to match players and 
    /// play Boggle games.
    /// </summary>
    [ServiceContract]
    public interface IBoggleService
    {
        /// <summary>
        /// Sends back index.html as the response body.
        /// </summary>
        [WebGet(UriTemplate = "/api")]
        Stream API();

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
        [WebInvoke(Method = "POST", UriTemplate = "/users")]
        CreateUserReturnInfo CreateUser(PlayerInfo user);

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
        [WebInvoke(Method = "POST", UriTemplate = "/games")]
        JoinGameReturnInfo JoinGame(JoinGameInfo game);

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
        [WebInvoke(Method = "PUT", UriTemplate = "/games")]
        void CancelJoin(PlayerInfo player);

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
        /// <param name="GameID">Contains the requested game id</param>
        /// <param name="wordPlayed">Contains the body of the request</param>
        /// <returns>The score of the played word</returns>
        [WebInvoke(Method = "PUT", UriTemplate = "/games/{GameID}")]
        PlayWordReturnInfo PlayWord(string GameID, WordInfo wordPlayed);

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
        [WebGet(UriTemplate = "/games/{GameID}?Brief={brief}")]
        GameStatusReturnInfo GameStatus(string GameID, string brief);
    }
}
