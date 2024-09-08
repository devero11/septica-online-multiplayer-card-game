using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class playerUsernames : MonoBehaviour
{
    public TextMeshPro[] usernames;
    public NakamaConnection nakama;
    // Start is called before the first frame update
    void Start()
    {
        usernames[0].text = nakama.Session.Username + ":" + nakama.playerPoints;
        usernames[1].text = nakama.oppUsernames[0]+ ":" + nakama.oppPoints[0];
        usernames[2].text = nakama.oppUsernames[1] + ":" + nakama.oppPoints[1];

        
    }   

    // Update is called once per frame
    void Update()
    
    {   for(int i = 0; i<usernames.Length; i++)
        if(usernames[i].text == nakama.turnUsername)
            usernames[i].color = new Color(1,0,0);
        else usernames[i].color = new Color(1,1,1);



        usernames[0].text = nakama.Session.Username + ":" + nakama.playerPoints;
        usernames[1].text = nakama.oppUsernames[0]+ ":" + nakama.oppPoints[0];
        usernames[2].text = nakama.oppUsernames[1] + ":" + nakama.oppPoints[1];
    }
}
