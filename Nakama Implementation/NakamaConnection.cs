using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using Nakama;
using UnityEngine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using Unity.Services.Matchmaker.Models;
[Serializable]
[CreateAssetMenu]
public class NakamaConnection : ScriptableObject
{   public float userRank;
    public float userLevel;
    public int userCoins;
    public string turnUsername;
    public GameObject cardTemplate;
    public int[] PlayerCards;
    public string winner = "";
    public bool errorTrigger = false;
    public bool playerLeftError = false;
    public int playerPoints;
    public int[] oppPoints= {0,0};
    public int[] TableCards={};

    public bool endTurnButton = false;

    public string[] oppUsernames = new string[2];
     public string Scheme = "http";
    public string Host = "localhost";
    public int Port = 7350;
    public string ServerKey = "defaultkey";

    private const string SessionPrefName = "nakama.session";
    public const string DeviceIdentifierPrefName = "nakama.deviceUniqueIdentifier";

    public IClient Client;
    public ISession Session;
    public ISocket Socket;

    public bool usernameExists = false;

    public IMatch match;
    private string ticket;
    public string username;
    public User[] users;
    public string gameMode = "";
    public async Task ConnectAsGuest()
    {
        try
        {
            // Connect to the Nakama server.
            Client = new Nakama.Client(Scheme, Host, Port, ServerKey, UnityWebRequestAdapter.Instance);

            // Attempt to restore an existing user session.
            // var authToken = PlayerPrefs.GetString(SessionPrefName);
            // if (!string.IsNullOrEmpty(authToken))
            // {
            //     var session = Nakama.Session.Restore(authToken);
            //     if (!session.IsExpired)
            //     {
            //         Session = session;
            //         Debug.Log("Session restored.");
            //     }
            // }
            
            // If we weren't able to restore an existing session, authenticate to create a new user session.
            if (Session == null)
            {
                string deviceId;
                // If we've already stored a device identifier in PlayerPrefs then use that.
                if (PlayerPrefs.HasKey(DeviceIdentifierPrefName))
                {
                    deviceId = PlayerPrefs.GetString(DeviceIdentifierPrefName);
                    Debug.Log("Using stored Device ID.");
                }
                else
                {
                    // If we've reached this point, get the device's unique identifier or generate a unique one.
                    deviceId = SystemInfo.deviceUniqueIdentifier;
                    if (deviceId == SystemInfo.unsupportedIdentifier)
                    {
                        deviceId = System.Guid.NewGuid().ToString();
                    }

                    // Store the device identifier to ensure we use the same one each time from now on.
                    PlayerPrefs.SetString(DeviceIdentifierPrefName, deviceId);
                    Debug.Log("New Device ID generated and stored.");
                }

                // Use Nakama Device authentication to create a new session using the device identifier.
                try
                {
                    Session = await Client.AuthenticateDeviceAsync(deviceId,username);
                    Debug.Log("Authenticated with Device ID");
                    PlayerPrefs.SetString(SessionPrefName, Session.AuthToken);
                }
                catch
                {
                     Debug.LogError($"Username Exists");
                    usernameExists = true;
                    return;
                }
                // Store the auth token that comes back so that we can restore the session later if necessary.
                
            }

            // Open a new Socket for real-time communication.
            try{
            Socket = Client.NewSocket();
            }
            catch{
                Debug.LogError($"Error socket in ConnectAsGuest");
                errorTrigger = true;
            }
            try
            {
                await Socket.ConnectAsync(Session, true);
                
                Debug.Log("Socket connected.");
                SceneManager.LoadScene(1);
                Socket.ReceivedMatchmakerMatched += OnRecievedMatchamakerMatch;
                Socket.ReceivedMatchState += OnRecievedMatchState;
                Socket.ReceivedMatchPresence += OnMatchPresence;
                Socket.ReceivedPartyData += JoinPartyMatch;
                
            }
            catch
            {
                Debug.LogError($"Error in ConnectAsGuest");
                errorTrigger = true;
            }

            
        }
        catch (ApiResponseException ex)
        {
            Debug.LogError($"Error in ConnectAsGuest: {ex.Message}");
            errorTrigger = true;
        }


    // var payload = "Pula"; // Replace with the actual search term
    // var result = await Client.RpcAsync(Session, "search_users", payload);
    // users = JsonConvert.DeserializeObject<User[]>( result.Payload );

    //     // Now you can access each user
    //     foreach (var user in users)
    //     {
    //         Debug.Log($"Username: {user.username}, ID: {user.id}");
    //     }


    fetchRank();



    }

    public async void fetchRank()
    {
        var account = await Client.GetAccountAsync(Session);
        if(JsonConvert.DeserializeObject<Dictionary<string, int>>(account.Wallet).ContainsKey("coins"))
        userCoins = JsonConvert.DeserializeObject<Dictionary<string, int>>(account.Wallet)["coins"];
        else userCoins= 0;

        IEnumerable<string> enumerableString = new List<string> { Session.UserId };
        foreach (var user in enumerableString)
            Debug.Log(user);
        var userData = await Client.GetUsersAsync(Session, enumerableString);
        foreach (var user in userData.Users)
        {
            Debug.Log("metadata: " + user.Metadata);
            var metadata = JsonConvert.DeserializeObject<Dictionary<string, float>>(user.Metadata);
            if (metadata.ContainsKey("rank"))
            {
                userRank = metadata["rank"];
                Debug.Log("user rank is : " + userRank);
                userLevel = metadata["lvl"];
            }
            else
            {
                var result = await Client.RpcAsync(Session, "InitializeRank");
                fetchRank();
               
            }
             await Client.RpcAsync(Session, "updateLeaderboard");
        }


    }

    public async void JoinPartyMatch(IPartyData data){
        Debug.Log(data.Data);
        string matchId= System.Text.Encoding.UTF8.GetString(data.Data);
         endTurnButton = false;
        oppUsernames = new string[2];
        try
        {
            match = await Socket.JoinMatchAsync(matchId);

            Debug.Log("Self ID:" + match.Self.SessionId);
            int x=0;
            foreach (var user in match.Presences)
            {
                if(user.Username != Session.Username){
                Debug.Log("Player ID:" + user.SessionId);
                oppUsernames[x] = user.Username;
                x++;
                }
            }


        }
        catch
        {
            Debug.Log("Error Joining Match");
        }
    }


    public async Task RandomId()
    {
        try
        {
            // Connect to the Nakama server.
            Client = new Nakama.Client(Scheme, Host, Port, ServerKey, UnityWebRequestAdapter.Instance);


                var randomId = System.Guid.NewGuid().ToString();
                    

                    // Store the device identifier to ensure we use the same one each time from now on.
                Debug.Log("New Random ID generated and stored.");
                

                // Use Nakama Device authentication to create a new session using the device identifier.
                try
                {
 
                    Session = await Client.AuthenticateDeviceAsync(randomId,username);
                    Debug.Log("Authenticated with Device ID");                
                    

                    
                    
                    }
                catch
                {
                    Debug.LogError($"Username Exists");
                    usernameExists = true;
                    return;
                }
                // Store the auth token that comes back so that we can restore the session later if necessary.
                
            

            // Open a new Socket for real-time communication.
            try{
            Socket = Client.NewSocket();
            }
            catch{
                Debug.LogError($"Error in RandomId");
                errorTrigger = true;
            }
            try
            {
                await Socket.ConnectAsync(Session, true);
                
                Debug.Log("Socket connected.");
                SceneManager.LoadScene(1);
                Socket.ReceivedMatchmakerMatched += OnRecievedMatchamakerMatch;
                Socket.ReceivedMatchState += OnRecievedMatchState;
                Socket.ReceivedMatchPresence += OnMatchPresence;
            }
            catch
            {
                Debug.LogError($"Error in RandomId");
                errorTrigger = true;
            }

            
        }
        catch (ApiResponseException ex)
        {
            Debug.LogError($"Error in RandomId: {ex.Message}");
            errorTrigger = true;
        }
        fetchRank();
    }

















    //Start 1v1 match
    public async void ThreePlayersMatch()
    {
        var matchmakerQuery = "+properties.mode:threePlayers";
        var matchmakerProperties = new Dictionary<string, string>{{ "mode", "threePlayers" }};
        Debug.Log("Finding 3 Match");
        try{
        var matchmakingTicket = await Socket.AddMatchmakerAsync(matchmakerQuery, 3,3,matchmakerProperties);
        ticket = matchmakingTicket.Ticket;}
        catch{
            errorTrigger = true;
            SceneManager.LoadScene(0);
        }
    }

        public async void FourPlayersMatch()
    {
        var matchmakerQuery = "+properties.mode:fourPlayers";
        var matchmakerProperties = new Dictionary<string, string>{{ "mode", "fourPlayers" }};
        Debug.Log("Finding 4 Match");
        try{
        var matchmakingTicket = await Socket.AddMatchmakerAsync(matchmakerQuery, 4,4, matchmakerProperties);
        ticket = matchmakingTicket.Ticket;}
        catch{
            errorTrigger = true;
            SceneManager.LoadScene(0);
        }
    }

        public async void TeamsMatch()
    {
        Debug.Log("Finding teams");
        try{
        var matchmakerQuery = "+properties.mode:teams";
        var matchmakerProperties = new Dictionary<string, string>{{ "mode", "teams" }};
        var matchmakingTicket = await Socket.AddMatchmakerAsync(matchmakerQuery, 4,4,matchmakerProperties);
        ticket = matchmakingTicket.Ticket;}
        catch{
            errorTrigger = true;
            SceneManager.LoadScene(0);
        }
    }









    public async void LobbyMatchmakerPlayer(string partyId, int index){
        Debug.Log("Finding 3 Match");
        try{


        if(index == 0){
            var matchmakerQuery = "+properties.mode:threePlayers";
            var matchmakerProperties = new Dictionary<string, string>{{ "mode", "threePlayers" }};
            var matchmakingTicket = await Socket.AddMatchmakerPartyAsync(partyId,matchmakerQuery, 3,3,matchmakerProperties);
            ticket = matchmakingTicket.Ticket;
        }
        else if(index == 1){
            var matchmakerQuery = "+properties.mode:fourPlayers";
            var matchmakerProperties = new Dictionary<string, string>{{ "mode", "fourPlayers" }};
            var matchmakingTicket = await Socket.AddMatchmakerPartyAsync(partyId,matchmakerQuery, 4,4,matchmakerProperties);
            ticket = matchmakingTicket.Ticket;}
        else {
            var matchmakerQuery = "+properties.mode:teams";
            var matchmakerProperties = new Dictionary<string, string>{{ "mode", "teams" }};
            var matchmakingTicket = await Socket.AddMatchmakerPartyAsync(partyId,matchmakerQuery, 4,4,matchmakerProperties);
            ticket = matchmakingTicket.Ticket;
        }
}
        catch{
            errorTrigger = true;
            SceneManager.LoadScene(0);
        }
    }


    private async void OnRecievedMatchamakerMatch(IMatchmakerMatched matchmakerMatched)
    {
        endTurnButton = false;
        oppUsernames = new string[2];
        try
        {
            match = await Socket.JoinMatchAsync(matchmakerMatched);

            Debug.Log("Self ID:" + match.Self.SessionId);
            int x=0;
            foreach (var user in match.Presences)
            {   
                if(user.Username != Session.Username){
                Debug.Log("Player ID:" + user.SessionId);
                oppUsernames[x] = user.Username;
                x++;
                }
            }


        }
        catch
        {
            Debug.Log("Error Joining Match");
        }


    }


    void OnRecievedMatchState(IMatchState state){
       
        var message = System.Text.Encoding.UTF8.GetString(state.State);
        Debug.Log("Received broadcast message: " + message);

        //Player left game code 101
        if(state.OpCode ==101){
            match = null;
            gameMode=null;
            playerLeftError = true;
        }


        //Match ended 100
        if(state.OpCode == 100)
          { match = null;
            gameMode= null;
          }


        //Cards Recieved 102
        if(state.OpCode == 102)
            {

        PlayerCards = JsonConvert.DeserializeObject<int[]>(message);
        Debug.Log(PlayerCards[0]);
            }


 
        //Table cards Recieved  104
        if(state.OpCode == 104 ){
            if(message != "{}"){
            TableCards = JsonConvert.DeserializeObject<int[]>(message);
            endTurnButton =false;
            Debug.Log(TableCards);
            }
            else TableCards = new int[0];
        }

        if(state.OpCode == 105){
            turnUsername = JsonConvert.DeserializeObject<string>(message);
            Debug.Log("It is the turn of :" + turnUsername);
        }
        

        if(state.OpCode == 106){
            endTurnButton =true;
        }
         if(state.OpCode == 109){
            winner = JsonConvert.DeserializeObject<string[]>(message)[0];
            fetchRank();
        }
        if(state.OpCode == 108){
            string[] data = JsonConvert.DeserializeObject<string[]>(message);
            if(data[0] == Session.Username)
                playerPoints = int.Parse(data[1]);
            if(oppUsernames[0] == data[0])
                oppPoints[0] = int.Parse(data[1]);
            if(oppUsernames[1] == data[0])
                oppPoints[1] = int.Parse(data[1]);
        }


        if(state.OpCode == 111){
            gameMode = message;
        }
    }

 private void OnMatchPresence(IMatchPresenceEvent presenceEvent)
    {
        // Log the players who joined
        foreach (var join in presenceEvent.Joins)
        {
            Debug.LogFormat("Player joined: {0}", join.Username);
        }

        // Log the players who left
        foreach (var leave in presenceEvent.Leaves)
        {
            Debug.LogFormat("Player left: {0}", leave.Username);
        }

        // Update the match presences with the new event
        match.UpdatePresences(presenceEvent);


        int x=0;
        foreach (var user in match.Presences)
            {
                Debug.Log("Player ID:" + user.SessionId);
                if(user.Username != Session.Username){
                    oppUsernames[x] = user.Username;
                    x++;
                }
        }
    }
    













}