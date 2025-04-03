using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ComplicitEndingManager : MonoBehaviour
{

    public bool gameOver;

    public TMP_Text timerText;

    // Start is called before the first frame update
    void Start()
    {
        gameOver = true;
        
    }

    public void StartComplicitMinigame() { 
        gameOver=false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameOver) { 

        }
    }
}
