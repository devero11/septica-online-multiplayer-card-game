using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class FetchUsername : MonoBehaviour
{   
    public TextMeshProUGUI text;
    public NakamaConnection nakama;
    // Start is called before the first frame update
    void Start()
    {
        text.text = "user:" + nakama.Session.Username;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
