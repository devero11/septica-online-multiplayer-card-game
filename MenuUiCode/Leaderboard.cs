using System.Collections;
using System.Collections.Generic;
using Nakama;
using UnityEngine;
using UnityEngine.UIElements;

public class Leaderboard : MonoBehaviour
{
    public UIDocument uiDocument;

    public NakamaConnection nakama;
    public VisualTreeAsset userAssetTempate;


    Button leaderboard;
    ScrollView list;

    Button global;
    Button yours;
    // Start is called before the first frame update
    void Start()
    {
        var root = uiDocument.rootVisualElement;

        leaderboard = root.Q<Button>("leaderboard");
        global = root.Q<Button>("global");
        yours = root.Q<Button>("yours");
        list = root.Q<ScrollView>("leaderboardList");
        global.clicked += fetchGlobal;
        yours.clicked += fetchLocal;
        fetchGlobal();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    async void fetchGlobal(){
        list.Clear();
       var ranks = await nakama.Client.ListLeaderboardRecordsAsync(nakama.Session, "ranks",null,null,20);
       Debug.Log(ranks);
        
        foreach(var user in ranks.Records)
        {
            var newElement = userAssetTempate.CloneTree(); 

            Label username = newElement.Q<Label>("username");
            Label rank = newElement.Q<Label>("rank");
            username.text = user.Username;
            rank.text = user.Score;
            Debug.Log(user.Username);
            list.Add(newElement);
        }
    }
    async void fetchLocal(){
         list.Clear();
       var ranks = await nakama.Client.ListLeaderboardRecordsAroundOwnerAsync(nakama.Session,"ranks",nakama.Session.UserId,null,20);
       Debug.Log(ranks);
        foreach(var user in ranks.Records)
        {
            var newElement = userAssetTempate.CloneTree(); 

            Label username = newElement.Q<Label>("username");
            Label rank = newElement.Q<Label>("rank");
            username.text = user.Username;
            rank.text = user.Score;
            Debug.Log(user.Username);
            list.Add(newElement);
        }
    }
}
