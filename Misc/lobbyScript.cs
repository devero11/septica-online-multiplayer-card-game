using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Nakama;
using Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class lobbyScript : MonoBehaviour
{
    public NakamaConnection nakama;
    public UIDocument document;
    public VisualTreeAsset addFriendTemplate;
    public IParty party;
    VisualElement joinedFriends;
    VisualElement friendList;
    Button exitButton;

    DropdownField dropdown;

    IUserPresence partyLeader;

    Button findMatch;
    public IPartyPresenceEvent presences;
    bool onPartyRecieved = false;
    // Start is called before the first frame update
    void OnEnable()
    {   //CreateParty();
        var root = document.rootVisualElement;
        joinedFriends = root.Q<VisualElement>("joined");
        friendList = root.Q<ScrollView>("friends");
        exitButton = root.Q<Button>("exitButton");
        findMatch = root.Q<Button>("FindMatch");

        dropdown = root.Q<DropdownField>("gamemode");



        exitButton.clicked += exitParty;

        findMatch.clicked += lobbyMatchSearch;

        if (nakama)
            fetchFriends();
        try
        {
            nakama.Socket.ReceivedParty += saveParty;
            nakama.Socket.ReceivedPartyLeader += updateLeader;
            
        }
        catch
        {
            nakama.errorTrigger = true;
            SceneManager.LoadScene(0);
        }
        nakama.Socket.ReceivedPartyPresence += TriggerDisplayParty;
        nakama.Socket.ReceivedPartyData += nakama.JoinPartyMatch;
        if (partyLeader.Username != nakama.Session.Username)
            findMatch.style.display = DisplayStyle.None;



    }


    void updateLeader(IPartyLeader leader){
        partyLeader = leader.Presence;
    }


    async void lobbyMatchSearch()
    {

        if ((party.Presences.Count() < 3 && dropdown.index == 0) ||  (party.Presences.Count() <4 && dropdown.index > 0)){
            nakama.LobbyMatchmakerPlayer(party.Id , dropdown.index);
            return;
            }
        if( (party.Presences.Count() > 3 && dropdown.index == 0) || party.Presences.Count() > 4 )
            return;




            var response = (party.Presences.Count() == 3 && dropdown.index == 0)?  await nakama.Client.RpcAsync(nakama.Session, "threePlayerLobby")  
        : dropdown.index == 1?  await nakama.Client.RpcAsync(nakama.Session, "fourPlayerLobby") :await nakama.Client.RpcAsync(nakama.Session, "teamLobby");

            string matchId= JsonConvert.DeserializeObject<Dictionary<string,string>>(response.Payload)["match_id"];
            Debug.Log("Match created with id:" + response.Payload);


            await nakama.Socket.SendPartyDataAsync(party.Id, 110, System.Text.Encoding.UTF8.GetBytes(matchId));
            // Optionally, join the created match
            nakama.endTurnButton = false;
            nakama.oppUsernames = new string[2];
            nakama.match = await nakama.Socket.JoinMatchAsync(matchId);

        

    }
    void exitParty()
    {
        nakama.Socket.LeavePartyAsync(party.Id);
        party = null;
        gameObject.SetActive(false);
        partyLeader= null;
    }


    // Update is called once per frame
    void Update()
    {  
        if (partyLeader.Username != nakama.Session.Username){
            findMatch.style.display = DisplayStyle.None;
            dropdown.style.display = DisplayStyle.None;
        }
        else    {
            findMatch.style.display = DisplayStyle.Flex;
            dropdown.style.display = DisplayStyle.Flex;}

        if (onPartyRecieved)
            DisplayParty(presences);



    }

     void TriggerDisplayParty(IPartyPresenceEvent partyPresenceEvent = null)
    {
        onPartyRecieved = true;
        presences = partyPresenceEvent;
    }


    public void saveParty(IParty partyData)
    {
        Debug.Log("party Received");
        party = partyData;
        partyLeader = party.Leader;
    }

    public void DisplayParty(IPartyPresenceEvent partyPresenceEvent = null)
    {
        joinedFriends.Clear();
        if (partyPresenceEvent != null)
        {
            party.UpdatePresences(partyPresenceEvent);
            foreach (var f in party.Presences)
            {
                var newElement = addFriendTemplate.CloneTree();
                var username = newElement.Q<Label>("username");
                var button = newElement.Q<Button>();
                username.text = f.Username;
                button.style.display = DisplayStyle.None;
                joinedFriends.Add(newElement);
            }
        }
        onPartyRecieved = false;
    }

    public async void fetchFriends()
    {
        friendList.Clear();
        var result = await nakama.Client.ListFriendsAsync(nakama.Session, 0, 100);
        foreach (var f in result.Friends)
        {
            var newElement = addFriendTemplate.CloneTree();
            var username = newElement.Q<Label>("username");
            var button = newElement.Q<Button>();
            username.text = f.User.Username;
            button.text = "Invite Friend";
            button.clicked += () => InviteFriend(f.User.Id);
            friendList.Add(newElement);

        }
    }

    public async void CreateParty()
    {
        try
        {
            party = await nakama.Socket.CreatePartyAsync(true, 4);
            partyLeader = party.Leader;
        }
        catch
        {
            nakama.errorTrigger = true;
            SceneManager.LoadScene(0);
        }

        DisplayParty();
    }

    async void InviteFriend(string id)
    {
        var message = new Dictionary<string, string>
        {
            { "receiver_id", id },
            { "content", party.Id }
        };
        Debug.Log(Newtonsoft.Json.JsonConvert.SerializeObject(message));
        if(party.Presences.Count()<4)
        await nakama.Client.RpcAsync(nakama.Session, "send_invite", Newtonsoft.Json.JsonConvert.SerializeObject(message));
    }


}
