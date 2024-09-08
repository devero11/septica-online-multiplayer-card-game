using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogIn : MonoBehaviour
{
    public GameObject x;
    public GameObject y;
    public NakamaConnection nakamaConnection;
    public void Update(){
        x.SetActive(nakamaConnection.errorTrigger);
        y.SetActive(nakamaConnection.usernameExists);
    }

    public void resetError(){
        nakamaConnection.errorTrigger = false;
        nakamaConnection.usernameExists = false;
    }
    public async void logIn(){
        await nakamaConnection.ConnectAsGuest();
    }
        public async void logInRandom(){
        await nakamaConnection.RandomId();
    }
}
