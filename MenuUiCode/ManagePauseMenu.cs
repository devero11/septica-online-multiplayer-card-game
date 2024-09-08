using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class ManagePauseMenu : MonoBehaviour
{
    public UIDocument uiDocument;

    Button pause;
    Button resume;
    Button endTurn;
    Button quit;
    VisualElement menu;
    public NakamaConnection nakama;
    // Start is called before the first frame update
    void Start()
    {
        var root = uiDocument.rootVisualElement;
        pause = root.Q<Button>("pause");
        resume = root.Q<Button>("resume");
        quit = root.Q<Button>("quit");
        menu = root.Q<VisualElement>("menu");
        endTurn = root.Q<Button>("EndTurn");

        pause.clicked += Pause;
        resume.clicked += Resume;
        quit.clicked += Quit;

        endTurn.clicked +=EndTurn;
    }

    void Update(){
        if(nakama.endTurnButton)
            endTurn.style.display = DisplayStyle.Flex;
        else endTurn.style.display = DisplayStyle.None;
    }
    void Quit()
    {
        nakama.Socket.LeaveMatchAsync(nakama.match);
        nakama.match = null;
        nakama.gameMode = null;
        SceneManager.LoadScene(1);
    }
    void Resume(){
        menu.style.display = DisplayStyle.None;
    }
    void Pause(){
        if(nakama.winner == "")
            menu.style.display = DisplayStyle.Flex;
    }

    void EndTurn(){
        nakama.Socket.SendMatchStateAsync(nakama.match.Id, 107, "");
        nakama.endTurnButton = false;
    }
}
