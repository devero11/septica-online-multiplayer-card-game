using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nakama;
using UnityEngine.SceneManagement;
public class matchFinderManager : MonoBehaviour
{
    public NakamaConnection nakamaConnection;

    public GameObject errorPopup;
    public void ThreePlayerSearch(){
        nakamaConnection.ThreePlayersMatch();
        
    }

    void Update(){


 ///       if(nakamaConnection.match != null)
 ///       
        if(nakamaConnection.gameMode == "ThreePlayers")
            SceneManager.LoadScene(2);
        else if(nakamaConnection.gameMode == "FourPlayers")
            SceneManager.LoadScene(3);
        else if(nakamaConnection.gameMode == "Teams")
            SceneManager.LoadScene(4);
    }

}
