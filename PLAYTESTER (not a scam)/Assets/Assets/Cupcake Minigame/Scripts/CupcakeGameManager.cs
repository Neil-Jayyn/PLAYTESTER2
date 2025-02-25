using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Kino;

public class CupcakeGameManager : MonoBehaviour
{
    public float gameDuration = 30f;

    public TMP_Text scoreText;
    public int scorePoints;

    //int currentDay;

    //timer text
    public TMP_Text timerText;
    public float timeRemaining = 0;
    
    private bool gameOver;
    public bool gamePlaying;

    private int timesPlayed;

    public ComputerUIScript UIController;
    GameObject MainGameManager;

    // Kino effect
    public DigitalGlitch GlitchEffect;
    public AnalogGlitch AnalogGlitchEffect;

    // Glitch variables
    public float glitchWaitTime = 1f;
    public float glitchFrequency; //
    public float glitchLength = 1f;
    public bool isGlitch = false;

    // Background variables
    public GameObject bg;
    public Sprite bgNormal;
    public Sprite bgGlitched;

    //sound effect
    public AudioSource sfx;
    public AudioClip sfxNormalYay;
    public AudioClip sfxGlitchedYay;

    void Start()
    {
        gamePlaying = false;
        timesPlayed = 0;

        UIController = GameObject.Find("UI Controller").GetComponent<ComputerUIScript>();
        //MainGameManager = GameObject.Find("Game Manager");
        sfx=GetComponent<AudioSource>();
       
    }

    //This is the function that will be called by the AppScript script. It should contain
    //everything needed to start the cupcake minigame
    public void StartCupcakeMinigame()
    {

        gameOver = false;
        gamePlaying = true;
        timesPlayed++;

        scorePoints = 0;
        UpdateScoreText();

        //normalcupcakeColor = cupcakeRenderer.color;
        timerText.text = "Time Left: " + gameDuration.ToString();
        //StartCoroutine(GlitchCheckRoutine());


        //set the playCupcakeMinigame in the EnemySpawner script so the game plays
        GameObject.Find("EnemySpawner").GetComponent<EnemySpawner>().playCupcakeMinigame = true;
        GameObject.Find("cupcakeHolder").GetComponent<playerMovement>().playCupcakeMinigame = true;

        
        //set the timer for the game
        timeRemaining = gameDuration;

        // If it is the first time playing, tutorial popup
        if (timesPlayed == 1)
        {
            glitchFrequency = 0;
            UIController.TriggerPopup(new Vector3(50, 50, -5), "Use the left and right arrow keys to move and spacebar to drop.\r\nGive cupcakes to everyone!\r\n");
        } else if (timesPlayed == 2)
        {
            glitchFrequency = 0.3f;
        } else // day 3; need a variable to determine which route to take
        {
           
        }

        // Start glitch checking
        StartCoroutine(GlitchCheckRoutine());
    }

    // Update is called once per frame
    void Update()
    {

        if(gamePlaying)
        {
            timeRemaining -= Time.deltaTime;
            timerText.text = "Time Left: " + Mathf.RoundToInt(timeRemaining);

            if(timeRemaining <= 0)
            {
                GameOver();
            }
        }


    }

    IEnumerator GlitchCheckRoutine()
    {
        while (!gameOver)
        {
            yield return new WaitForSeconds(glitchWaitTime);  // ie check every second

            if (glitchFrequency > 0 && Random.value < glitchFrequency)
            {
                StartCoroutine(ActivateGlitch());
            }
        }
    }

    IEnumerator ActivateGlitch()
    {
        isGlitch = true;
        GlitchEffect.intensity = Random.Range(0f, 0.3f); // can adjust
        AnalogGlitchEffect.colorDrift = Random.Range(0f, 0.3f);
        SpriteRenderer spriteRenderer = bg.GetComponent<SpriteRenderer>(); // initialize to change background
        spriteRenderer.sprite = bgGlitched;

        yield return new WaitForSeconds(glitchLength); // Glitch lasts 1 second
        
        // turn off glitches unless glitches are supposed to happen 100% 
        if (glitchFrequency != 1)
        {
            isGlitch = false;
            GlitchEffect.intensity = 0;
            AnalogGlitchEffect.colorDrift = 0;
            spriteRenderer.sprite = bgNormal;
        } 
    }

    private void GameOver()
    {
        gamePlaying = false;
        gameOver = true;
        timerText.text = "Game Stop!";
        //Time.timeScale = 0f; // Stop everything

        //set the enemy spawner script to false
        GameObject.Find("EnemySpawner").GetComponent<EnemySpawner>().playCupcakeMinigame = false;

        //TODO: calculate score to send to the hp based on the points earned in the round
        int score = -12; //for now just let this be the max

        //Go back to the main script and say that the game is done:
        GameObject.Find("Game Manager").GetComponent<GameManagerScript>().CompletedMinigame(score);

        GameObject.Find("cupcakeHolder").GetComponent<playerMovement>().playCupcakeMinigame = false;

        //TODO: add a wait a second before go to the leaderboard screen.
        GameObject.Find("UI Controller").GetComponent<ComputerUIScript>().GoToPosition(new Vector3(90, 50, -10)); //go to the leaderboard

    }

    void UpdateScoreText() {
        scoreText.text = "SCORE: " + scorePoints;
    }

    public void AddPoints(int points) { 
        scorePoints+=points;
        UpdateScoreText();
    }
   
    public void SfxCupcakeHit(bool isGlitch) {
        if (isGlitch) //plays sfx depending if glitches are happening or not
        {
            sfx.clip = sfxGlitchedYay;
            sfx.Play();
        }
        else { 
            sfx.clip = sfxNormalYay;
            sfx.Play();
        }
    }





    /*
     aaah
    IEnumerator GlitchCheckRoutine()
    {
        while (!gameOver)
        {
            yield return new WaitForSeconds(glitchRestTime); // Check every second

            if (glitchFrequency > 0 && Random.value < glitchFrequency) // If we get a random value less than the glitch frequency, the glitches will occur
            {
                StartCoroutine(ActivateGlitch());
            }
        }
    }
    IEnumerator ActivateGlitch()
    {
        // Apply glitch effects
        cupcakeRenderer.color = glitchColor;

        yield return new WaitForSeconds(1f); // Glitch lasts 1 second

        // Revert to normal if the glitch frequency isn't 100%
        if (glitchFrequency != 1)
        {
            cupcakeRenderer.color = normalcupcakeColor;
        }

    }*/
}
