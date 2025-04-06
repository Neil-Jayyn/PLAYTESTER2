using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ComplicitEndingManager : MonoBehaviour
{

    public bool gameOver;
    GameObject crosshair;
    GameObject player;


    public TMP_Text timerText;

    // Start is called before the first frame update
    void Start()
    {
        gameOver = true;

        //set objects
        crosshair = GameObject.Find("complicit player");
        player = GameObject.Find("complayer Target");

        crosshair.SetActive(false);
        player.SetActive(false);
        
    }

    public void StartComplicitMinigame() { 
        gameOver=false;
        crosshair.SetActive(true);
        player.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
