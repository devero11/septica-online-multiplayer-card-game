using System.Collections;
using System.Collections.Generic;
using Nakama;
using Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UIElements;
public class TabScript : MonoBehaviour
{
    private VisualElement[] elements = new VisualElement[4];
    private Button[] buttons = new Button[4];
    private Button errorPopup;
    public NakamaConnection nakama;
    
    private Button threePlayers;
    private Button fourPlayers;
    private Button teams;

    private Label username;
    private Label rank;
    private Label level;
    private Label coins;
    public UIDocument uiDocument;

    Button threeFriends;
    IApiNotification invitationApi;

    VisualElement invitation;
    Button InviteButton;
    Button decline;

    Label InviteText;
    public GameObject lobby;
    // Start is called before the first frame update
    void Start()
    {   
        if(nakama.Socket !=null)
        nakama.Socket.ReceivedNotification += PopUpInvite;
        // Get the root VisualElement of the UI Document
        var root = uiDocument.rootVisualElement;


        invitation = root.Q<VisualElement>("invitation");
        InviteButton = root.Q<Button>("acceptInvitation");
        decline = root.Q<Button>("declineInvitation");
        InviteText = root.Q<Label>("inviteText");
        InviteButton.clicked += JoinParty;
        decline.clicked += Decline;
        

        threeFriends = root.Q<Button>("threeFriends");
        threeFriends.clicked += createParty;




        errorPopup = root.Q<Button>("playerLeft");
        errorPopup.clicked += error;

        threePlayers = root.Q<Button>("threePlayers");
        threePlayers.clicked += ThreePlayerSearch;

        fourPlayers = root.Q<Button>("fourPlayers");
        fourPlayers.clicked += FourPlayerSearch;

        teams = root.Q<Button>("teams");
        teams.clicked += TeamsSearch;


        username = root.Q<Label>("username");
        level = root.Q<Label>("level");
        rank = root.Q<Label>("rank");
        coins = root.Q<Label>("coins");

        if(nakama.Session !=null){
            username.text = nakama.Session.Username;
            rank.text = "rank:"+nakama.userRank;
        }
        // Query and assign the VisualElement and Button
        elements[0] = root.Q<VisualElement>("Home");
        elements[1] = root.Q<VisualElement>("Friends");
        elements[2] = root.Q<VisualElement>("LeaderBoard");
        elements[3] = root.Q<VisualElement>("Shop");
        buttons[0] = root.Q<Button>("home");
        buttons[1] = root.Q<Button>("friends");
        buttons[2] = root.Q<Button>("leaderboard");
        buttons[3] = root.Q<Button>("shop");
        // Register the button click event
        buttons[0].clicked += () => OnButtonClick(0);
        buttons[1].clicked += () => OnButtonClick(1);
        buttons[2].clicked += () => OnButtonClick(2);
        buttons[3].clicked += () => OnButtonClick(3);
        

        
    }
    void Decline(){
        invitationApi=null;
    }

    async void JoinParty(){
        Debug.Log(invitationApi.Content);
        
        try{
        nakama.Socket.ReceivedParty += lobby.GetComponent<lobbyScript>().saveParty;
        await nakama.Socket.JoinPartyAsync(JsonConvert.DeserializeObject<Dictionary<string,string>>(invitationApi.Content)["message"]);
        lobby.SetActive(true);
        invitationApi = null;
        }
        catch{}
        //lobby.GetComponent<lobbyScript>().DisplayParty();
    }


    void createParty(){
        lobby.GetComponent<lobbyScript>().CreateParty();
        lobby.SetActive(true);
        
    }

    string inviteSender;
     void PopUpInvite(IApiNotification notification){
        if(notification.Code == 1){
        invitationApi=notification;
        Debug.Log(notification.Content);
        Debug.Log("1 is the code Recieved");
        }
        Debug.Log("Invitation Recieved");
        var message = JsonConvert.DeserializeObject<Dictionary<string,string>>(notification.Content);
        inviteSender = message["username"] +" has invited you!";
    }

    // Update is called once per frame
    private void OnButtonClick(int index)
    {   
        Debug.Log("Button Pressed");
        // Disable the VisualElement
        elements[index].style.display = DisplayStyle.Flex;
        for(var i = 0; i<4;i++)
            if(i !=index)
                elements[i].style.display = DisplayStyle.None;
    }

    public void error(){
        nakama.playerLeftError = false;
        errorPopup.style.display= DisplayStyle.None;
    }

    public void ThreePlayerSearch(){
        nakama.ThreePlayersMatch();
        
    }
    public void FourPlayerSearch(){
        nakama.FourPlayersMatch();
        
    }
        public void TeamsSearch(){
        nakama.TeamsMatch(); 
    }


    void Update(){
        
        if(invitationApi !=null){
            invitation.style.display = DisplayStyle.Flex;
            InviteText.text=inviteSender;
        }
        else
            invitation.style.display = DisplayStyle.None;
        if(nakama.playerLeftError)
            errorPopup.style.display = DisplayStyle.Flex;
        if(nakama.Session !=null){
            rank.text = "rank:"+nakama.userRank;
            level.text = "lvl:"+nakama.userLevel;
            coins.text = "coins:"+nakama.userCoins;
        }
    }
}
