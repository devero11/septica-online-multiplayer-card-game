using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

using UnityEngine.UI;

public class Dealer : MonoBehaviour
{
    int[] oldCardsArray = new int[4];
    public   GameObject tableCard;
    int oldTableLength;
    public TextMeshProUGUI x;
    public NakamaConnection nakama;
    public Sprite[] cardSprites;
    bool cardsToServe = true;
    public GameObject canvas;
    public GameObject[] cardsArray;
    // Start is called before the first frame update
    void Start()
    {
        nakama.winner = "";
        nakama.playerPoints=0;
        nakama.oppPoints[0] = 0;
        nakama.oppPoints[1] = 0;
    
        nakama.TableCards= new int[0];
    }

    // Update is called once per frame

    void Update()
    {

        if(nakama.match == null && nakama.winner =="")
            SceneManager.LoadScene(1);

        if(nakama.winner ==""){
        initializeCards();
        oldCardsArray = nakama.PlayerCards; 


        // for(int i = 0; i < nakama.PlayerCards.Length; i++){
        //     if(Array.Exists(nakama.TableCards, element => element == nakama.PlayerCards[i])){
        //        cardsArray[i].SetActive(false) ;
        //     }
        // }

        if(oldTableLength+1 == nakama.TableCards.Length){
            var x = Instantiate(tableCard);
            x.transform.localScale = new Vector2(0.20f,0.20f);
            x.transform.position = new Vector3( -1.5f+0.5f*(oldTableLength%7), 1.5f*math.floor(oldTableLength/7),+10-0.1f*oldTableLength);
            x.GetComponent<SpriteRenderer>().sprite = cardSprites[nakama.TableCards[nakama.TableCards.Length-1]];
        }
        if(nakama.TableCards.Length ==0)
        {
            GameObject[] objectsToDestroy = GameObject.FindGameObjectsWithTag("TableCard");

            // Loop through and destroy each object
            foreach (GameObject obj in objectsToDestroy)
            {
                Destroy(obj);
            }
        }
        oldTableLength = nakama.TableCards.Length;
        }
    }



    void initializeCards(){
        
        if(nakama.PlayerCards != oldCardsArray){
            Debug.Log("Initializing cards");
            for(int i = 0; i < nakama.PlayerCards.Length; i++){
                if(nakama.PlayerCards[i] !=-1){
                    cardsArray[i].SetActive(true);
                    cardsArray[i].GetComponent<SpriteRenderer>().sprite = cardSprites[nakama.PlayerCards[i]];
                    cardsArray[i].GetComponent<DragCard>().cardIndex = nakama.PlayerCards[i];
                }else cardsArray[i].SetActive(false) ;
                
            }
            
        }
    }
}
