using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Kino;

public class MinigameManager : MonoBehaviour
{
    public ComputerUIScript UIController;
    GameObject SpawnMg;
    GameObject MainGameManager;

    // Manages glitches 
    public float glitchFreq; // Set based on the day and situation
    public float glitchWaitTime = 1f;
    public float gameDur = 5f;
    public bool isGlitch;

    // kino effect
    public DigitalGlitch GlitchEffect;
    public AnalogGlitch AnalogGlitchEffect;

    // Timer management 
    public TMP_Text timerText;
    private bool isGameOver;

    //Score management
    public TMP_Text scoreText;
    public int points;

    // Audio management
    public AudioSource sfx;
    public AudioClip normalSFX;
    public AudioClip glitchedSFX;


    // General variables
    private int timesPlayed;
    // bg1, bg2 
    // public SpriteRenderer bg1Renderer;
    // public SpriteRenderer bg2Renderer;
    // Good spawnable
  
    // player color can change
    public SpriteRenderer playerRenderer;
    public Color glitchPlayerColor = new Color(0f, 0f, 0f, 1f); // Black
    private Color normalPlayerColor;

    //leaderboard text
    public TMP_Text LeaderboardText;

    

    void Start()
    {
        UIController = GameObject.Find("UI Controller").GetComponent<ComputerUIScript>(); 
        MainGameManager = GameObject.Find("Game Manager");
        SpawnMg = GameObject.Find("SpawnMg");

        // SFX
        sfx = GetComponent<AudioSource>();
        sfx.clip = normalSFX;

        normalPlayerColor = playerRenderer.color; // Set starting color as player's uncorrupted color

        isGameOver = true;
        points = 0;
        timesPlayed = 0;
        isGlitch = false;

    }

    //This function is called by the AppScript and should start all the setup needed for the minigame
    public void StartCoinMinigame()
    {
        isGameOver = false;
        SpawnMg.GetComponent<spawnMg>().SpawnMgStart();
        StartCoroutine(GlitchCheckRoutine());
        StartCoroutine(StartMinigameTimer());

        points = 0;
        ++timesPlayed;

        //If it is the first time playing, tutorial popup amd no glitches
        if (timesPlayed == 1)
        {
            glitchFreq = 0f;
            UIController.TriggerPopup(new Vector3(50, 36, -5), "Use the up and down arrows to move lanes. Grab all the gold coins!");
        } else if (timesPlayed == 2)
        {
            glitchFreq = 0.3f;
        } else // day 3; need a variable to determine which route to take
        {

        }


    }

    IEnumerator GlitchCheckRoutine()
    {
        while (!isGameOver)
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

        playerRenderer.color = glitchPlayerColor;
        sfx.clip = glitchedSFX;

        yield return new WaitForSeconds(1f); // Glitch lasts 1 second
        
        // Revert to normal if the glitch frequency isn't 100%
        if (glitchFreq != 1)
        {
           isGlitch = false;
           playerRenderer.color = normalPlayerColor;
           sfx.clip = normalSFX;
           GlitchEffect.intensity = 0;
           AnalogGlitchEffect.colorDrift = 0;

        }

    }

    IEnumerator StartMinigameTimer()
    {
        float timer = gameDur; // Set timer

        while (timer > 0 && !isGameOver)
        {
            timer -= Time.deltaTime; // Update timer
            timerText.text = "Time: " + Mathf.RoundToInt(timer); // Display timer (rounded)
            yield return null;
        }

        GameOver(); // When time is up, call game over function
    }

    void GameOver()
    {
        isGameOver = true;
        timerText.text = "Time's Up!";

        LeaderboardText.SetText(points + " Points!");

        UIController.GetComponent<ComputerUIScript>().GoToPosition(new Vector3(90, 35, -10)); //go to the leaderboard

        int score = CalculateScore(points); 

        //update the dashboard leaderboards
        MainGameManager.GetComponent<GameManagerScript>().RefreshLeaderboard(2, points);

        //tell the game manager that the minigame is done
        MainGameManager.GetComponent<GameManagerScript>().CompletedMinigame(score);

        
        
    }

    private int CalculateScore(int pointsEarned)
    {
        float score = pointsEarned * -11;
        //Let 16 be a reasonably good score to achieve -> update this once speed is added
        score = score / 16;

        if (score < -12) //dont let the score go beyond -12, since this is the max
        {
            score = -12;
        }

        return (int)score;
    }

    //Get the status of the game
    public bool IsGameOver()
    {
        return isGameOver;
    }

    public void AddPoints(int point)
    {
        points += point;
        UpdateScore();
    }

    void UpdateScore()
    {
        scoreText.text = "Points: "+points.ToString();
    }
    

    /* This function was supposed to delete all the spawnables after the game ends but it's creating an
     * infinite loop somehow
    
    private void DeleteSpawnables()
    {
        GameObject spawnable = GameObject.Find("Spawnable(Clone)");
        while (spawnable != null)
        {
            Destroy(spawnable);
            spawnable = GameObject.Find("Spawnable(Clone)");
        }

        return;
    }

    */
}

