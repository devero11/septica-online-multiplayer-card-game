using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TerrainUtils;
using UnityEngine.UIElements;

public class WinnerMenu : MonoBehaviour
{   
    public UIDocument uiDocument;
    Label text;
    public NakamaConnection nakama;
    Button exit;
    VisualElement menu;
    // Start is called before the first frame update
    void Start()
    {
        var root = uiDocument.rootVisualElement;
        exit = root.Q<Button>("Exit");
        menu = root.Q<VisualElement>("Menu");
        text = root.Q<Label>("Winner");

        exit.clicked += Quit;
    }

    // Update is called once per frame
    void Update()
    {
        if(nakama.winner != ""){
            menu.style.display = DisplayStyle.Flex;
            if(nakama.Session.Username == nakama.winner)
                text.text = "You Won!";
            else text.text = "You lost!";
        }
    }
    async void Quit()
    {   try{
        await nakama.Socket.LeaveMatchAsync(nakama.match);
        }
        catch{}
        nakama.fetchRank();
        nakama.match = null;
        nakama.gameMode = null;
        SceneManager.LoadScene(1);
    }
}
