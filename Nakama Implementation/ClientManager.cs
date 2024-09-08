using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;



public class ClientManager : MonoBehaviour
{


    
    public NakamaConnection nakamaConnection;
    // Start is called before the first frame update
    async void Start()
    {
        nakamaConnection.errorTrigger = false;
        DontDestroyOnLoad(gameObject);
        Debug.Log("Start method called");
        if (PlayerPrefs.HasKey(NakamaConnection.DeviceIdentifierPrefName))
        {
            Debug.Log("Log In ID exists");
            await nakamaConnection.ConnectAsGuest();
        }
    }

    void Update()
    {   
        if(nakamaConnection.errorTrigger && SceneManager.GetActiveScene().buildIndex != 0)
            SceneManager.LoadScene(0);
        if (Input.GetKeyDown(KeyCode.L))
            PlayerPrefs.DeleteAll();
    }

    public void updateUsername(string Value){
        if(Value.Length>3)
        nakamaConnection.username = Value;
    }
}
