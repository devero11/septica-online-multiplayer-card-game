using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using UnityEngine;
using UnityEngine.UIElements;
using Newtonsoft.Json;
using UnityEngine.SocialPlatforms;
using Unity.VisualScripting;
using Nakama.Snippets;
using System.Linq;

public class FriendsTabScript : MonoBehaviour
{   
    public UIDocument uiDocument;


    public VisualTreeAsset addFriendTemplate;
    public VisualElement addFriendsList;
    Button[] tab= new Button[2];
    User[] users;


    VisualElement pendingList;
    VisualElement friendsList;
    VisualElement waitingList;
    TextField searchUser;
    public NakamaConnection nakama;



    VisualElement[] canvas = new VisualElement[2];
    // Start is called before the first frame update
    void Start()
    {

        var root = uiDocument.rootVisualElement;
        tab[0] = root.Q<Button>("FriendsList");
        tab[1] = root.Q<Button>("AddFriends");

        pendingList = root.Q<VisualElement>("pendingList");
        friendsList = root.Q<VisualElement>("friendsList");
        waitingList = root.Q<VisualElement>("waitingList");
        addFriendsList = root.Q<VisualElement>("addFriendsList");

        canvas[0] = root.Q<VisualElement>("existingFriends");
        canvas[1] = root.Q<VisualElement>("addFriend");


        tab[0].clicked += () => OnButtonClick(0);
        tab[1].clicked += () => OnButtonClick(1);



        searchUser = root.Q<TextField>("searchUser");
        searchUser.RegisterValueChangedCallback (evt =>
        {
            QuerySearch(evt.newValue);
        });
    if(nakama)
        returnFriendsList();
    }
    // Update is called once per frame
    void OnButtonClick(int x)
    {

        if(x == 0){

            returnFriendsList();
        }

        canvas[x].style.display = DisplayStyle.Flex;
        canvas[(x+1)%2].style.display = DisplayStyle.None;
    }


    async void  QuerySearch(string payload){
    addFriendsList.Clear();
    if(payload.Length > 3){
    var result = await nakama.Client.RpcAsync(nakama.Session, "search_users", payload);
    users = JsonConvert.DeserializeObject<User[]>( result.Payload );
        Debug.Log("Searching...");
        // Now you can access each user
        foreach (var user in users)
        {
            Debug.Log($"Username: {user.username}, ID: {user.id}");
            
            var newElement = addFriendTemplate.CloneTree();
            var username = newElement.Q<Label>("username");
            var button = newElement.Q<Button>();
            button.clicked += () => addFriendButton(user.username);
            username.text= user.username;
            addFriendsList.Add(newElement);
        }

    }else{ users = new User[]{};
    addFriendsList.Clear();
    }
    }



    async void returnFriendsList(){
        var result = await nakama.Client.ListFriendsAsync(nakama.Session, null, 100);
        Debug.Log("Kidnapping Friends");
        pendingList.Clear();
        waitingList.Clear();
        friendsList.Clear();
        Debug.Log(result.Friends.Count());
        foreach (var f in result.Friends)
        {
            if(f.State == 2){
                var newElement = addFriendTemplate.CloneTree();
                var username = newElement.Q<Label>("username");
                var button = newElement.Q<Button>();
                button.clicked += () => addFriendButton(f.User.Username);
                username.text= f.User.Username;
                pendingList.Add(newElement);
            }
            if(f.State == 0){
                var newElement = addFriendTemplate.CloneTree();
                var username = newElement.Q<Label>("username");
                var button = newElement.Q<Button>();
                button.clicked += () => removeFriend(f.User.Username);
                button.text = "Remove Friend";
                username.text= f.User.Username;
                friendsList.Add(newElement);
            }
            if(f.State == 1){
                var newElement = addFriendTemplate.CloneTree();
                var username = newElement.Q<Label>("username");
                var button = newElement.Q<Button>();
                button.clicked += () => removeFriend(f.User.Username);
                button.text = "Remove Invite";
                username.text= f.User.Username;
                waitingList.Add(newElement);
            }
            

        }
    
    }
    async void removeFriend(string username){
        var usernames = new[] {username};
        await nakama.Client.DeleteFriendsAsync(nakama.Session, null, usernames);
        returnFriendsList();
    }
    
    async void addFriendButton(string username){
        var usernames = new[] {username};
        await nakama.Client.AddFriendsAsync(nakama.Session, null, usernames);
        returnFriendsList();
    }

}




[System.Serializable]
public class User
{
    public long create_time;
    public long disable_time;
    public int edge_count;
    public string id;
    public string lang_tag;
    public string metadata;
    public long update_time;
    public string username;
    public long verify_time;
    public string wallet;
}
