﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Kino;

public class DuckGameManager : MonoBehaviour
{
    public TMP_Text scoreText;// Reference to TextMeshPro to show the score
    
    private int score = 0;
    public bool gameOver;
    private int timesPlayed;
    public ComputerUIScript UIController;
    private GameObject MainGameManager;

    float timeRemaining;
    public float gameDuration = 30;
    public TMP_Text timerText; //TMP to show timer

    // Glitch variables
    private float glitchFreq;
    private float glitchWaitTime = 1f;
    public bool isGlitch = false;

    // kino effect
    public DigitalGlitch GlitchEffect;
    public AnalogGlitch AnalogGlitchEffect;

    // Audio management
    public AudioSource[] audioSources;
    public AudioClip normalSFX;
    public AudioClip glitchedSFX;

    //leaderboard text
    public TMP_Text LeaderboardText;

    //Tutorial freeze management (Neila note: The code for it is bad but it works)
    //private bool isTutorialPlaying; //checks if there is a tutorial on screen
    //ublic GameObject popup;
    //Vector3 popupHome;
    //private bool hasInitialized; //checks if the game started game elements yet 
    //public GameObject freezeOverlay;


    void Start()
    {
        gameOver = true;
        score = 0;
        UpdateScoreText();
        timesPlayed = 0;
        UIController = GameObject.Find("UI Controller").GetComponent<ComputerUIScript>();
        MainGameManager = GameObject.Find("Game Manager");

        // SFX
        audioSources = GetComponents<AudioSource>();
        audioSources[0].clip = normalSFX;
//      failureSFX = GetComponent<AudioSource>();
 //     failureSFX.clip = failureClipSFX;

        //hasInitialized = false;
        //popup = GameObject.Find("Popup");
        //popupHome = new Vector3(0, 10, 0);
    }

    void Update()
    {
        if (!gameOver)
        {

            //if (popup.transform.position == popupHome&&!hasInitialized) //if tutorial left screen
           // {
                //isTutorialPlaying = false;
                //hasInitialized = true;
            //InitializeDuckGame();
            //}
            timeRemaining -= Time.deltaTime;
            timerText.text = "Time Left: " + Mathf.RoundToInt(timeRemaining);

            if (timeRemaining <= 0)
            {
                GameOver();
            }
        }
            /*
            else if (!isTutorialPlaying)
            {
                hasInitialized = true;
                InitializeCoinrunnerGame();
            }
            */

            //if (!isTutorialPlaying)
            //{
                
            //}
    }


    //This function is called by the AppScript and should handle setting up everything for the duck minigame
    public void StartDuckMinigame()
    {
        gameOver = false;
        score = 0;
        ++timesPlayed;
        timeRemaining = gameDuration;
        timerText.text = "Time Left: " + gameDuration.ToString();


        //StartCoroutine(GlitchCheckRoutine());

        if (timesPlayed == 1)
        {
            //isTutorialPlaying = true;
            glitchFreq = 0;
            UIController.TriggerPopup(new Vector3(48f, 22.5f, -5), "Use the mouse to aim and left click to shoot.Take down all the ducks!");
        } else if (timesPlayed == 2)
        {
            //isTutorialPlaying = false;
            glitchFreq = 0.3f;
            //InitializeDuckGame();
        } else // day 3; need a variable to determine which route to take
        {
            //isTutorialPlaying = false;
            //InitializeDuckGame();

        }
    }

    IEnumerator GlitchCheckRoutine()
    {
        while (!gameOver)
        {
            yield return new WaitForSeconds(glitchWaitTime); // Check every second

            if (glitchFreq > 0 && Random.value < glitchFreq) // If we get a random value less than the glitch frequency, the glitches will occur
            {
                StartCoroutine(ActivateGlitch());
            }
        }
    }

    IEnumerator ActivateGlitch()
    {
        // Apply glitch effects
        isGlitch = true;
        // Kino effect
        GlitchEffect.intensity = Random.Range(0f, 0.3f); // can adjust
        AnalogGlitchEffect.colorDrift = Random.Range(0f, 0.3f);
        audioSources[0].clip = glitchedSFX;

        yield return new WaitForSeconds(1f); // Glitch lasts 1 second

        // Revert to normal if the glitch frequency isn't 100%
        if (glitchFreq != 1)
        {
            isGlitch = false;
            audioSources[0].clip = normalSFX;
            GlitchEffect.intensity = 0;
            AnalogGlitchEffect.colorDrift = 0;
        }

    }
    public void GameOver()
    {
        gameOver = true;

        int scoreChange = CalculateScore(score);
        timerText.text = "Time's up!";
        timerText.text = "Time's up!";

        //Tell the game manager that the game is over
        MainGameManager.GetComponent<GameManagerScript>().CompletedMinigame(scoreChange);
        MainGameManager.GetComponent<GameManagerScript>().RefreshLeaderboard(3, score);


        //Update leaderboard text
        LeaderboardText.SetText(score + " Points!");

        //Go to the leaderboard
        UIController.GetComponent<ComputerUIScript>().GoToPosition(new Vector3(90, 20, -10)); //go to the leaderboard

        score = 0;
        UpdateScoreText();
    }

    private int CalculateScore(int pointsEarned)
    {
        float score = pointsEarned * -11;
        //Let 18 be a reasonably good score to achieve
        score = score / 18;

        if (score < -12) //dont let the score go beyond -12, since this is the max
        {
            score = -12;
        }

        return (int)score;
    }


    // Method to add or subtract points
    public void AddPoints(int points)
    {
        score += points;
        UpdateScoreText();
    }

    // Update the score display
    void UpdateScoreText()
    {
        scoreText.text = "Points: " + score;

    }

    void InitializeDuckGame() {
        
        GameObject.Find("SpawnManager").GetComponent<SpawnManager>().SpawnStart();
        GameObject.Find("SpawnManagerFast").GetComponent<SpawnManagerFast>().SpawnStart();
}
}
