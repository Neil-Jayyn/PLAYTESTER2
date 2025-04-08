using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ComplicitEndingManager : MonoBehaviour
{

    public bool gameOver;
    public GameObject crosshair;
    public GameObject player;

    public bool hasShotThemselves;
    
    public TMP_Text timerText;

    AudioSource sfx;
    public AudioClip gunshot;

    // Start is called before the first frame update
    void Start()
    {
        gameOver = true;
        hasShotThemselves = false;

        //set objects
        //crosshair = GameObject.Find("complicit player");
        //player = GameObject.Find("complayer Target");

        crosshair.SetActive(false);
        player.SetActive(false);
        sfx=GetComponent<AudioSource>();

        
    }

    public void StartComplicitMinigame()
    {
        Debug.Log("Started KYS mg");
        gameOver = false;
        crosshair.SetActive(true);
        player.SetActive(true);
    }

    public void Check() {
        Debug.Log("Check complicit function");
    }
    IEnumerator StartTimer() {
        float timer = 10f;
        while (timer > 0 && !gameOver)
        {
            timer -= Time.deltaTime; // Update timer
            timerText.text = "Time: " + Mathf.RoundToInt(timer); // Display timer (rounded)
            yield return null;
        }

        GameOver();
    }



    public void PlayerShotThemself() {
        hasShotThemselves = true;
        //TODO: sfx
        sfx.clip = gunshot;
        sfx.Play();
    }


    void GameOver() { 

    }
    // Update is called once per frame
    void Update()
    {
        
    }

}
