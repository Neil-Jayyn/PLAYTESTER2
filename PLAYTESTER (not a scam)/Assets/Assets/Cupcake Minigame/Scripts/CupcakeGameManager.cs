using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Kino;

public class CupcakeGameManager : MonoBehaviour
{
    [SerializeField]public float gameDuration = 60f;

    //score handling
    public TMP_Text scoreText;
    public int scorePoints;

    //timer handling
    public TMP_Text timerText;
    public float timeRemaining = 0f;
    
    //game states
    private bool gameOver;
    public bool gamePlaying;
    private bool tutorialPlaying;

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

    // Oven variables
    public GameObject oven;
    public Sprite ovenNormal;
    public Sprite ovenGlitched;

    //sound effect
    public AudioSource sfx;
    public AudioClip sfxNormalYay;
    public AudioClip sfxGlitchedYay;

    //tutorial freeze
    public GameObject popup;
    Vector3 popupHome;
    GameObject freezeOverlay;

    EnemySpawner spawner1;
    EnemySpawner spawner2;

    // Leaderboard variables
    public TMP_Text LeaderboardText;

    void Start()
    {
        gamePlaying = false;
        timesPlayed = 0;

        UIController = GameObject.Find("UI Controller").GetComponent<ComputerUIScript>();
        MainGameManager = GameObject.Find("Game Manager");
        sfx = GetComponent<AudioSource>();

        popup = GameObject.Find("Popup");
        popupHome = new Vector3(0, 10, 0);
        freezeOverlay = GameObject.Find("FreezeOverlay");
        GameObject.Find("cupcakeHolder").GetComponent<Animator>().enabled = false;

        spawner1 = GameObject.Find("EnemySpawner").GetComponent<EnemySpawner>();
        spawner2 = GameObject.Find("EnemySpawner (1)").GetComponent<EnemySpawner>();


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

        timerText.text = "Time Left: " + gameDuration.ToString();
        
        //set the timer for the game
        timeRemaining = gameDuration;

        // If it is the first time playing, tutorial popup
        if (timesPlayed == 1)
        {
            tutorialPlaying = true;
            glitchFrequency = 0;
            UIController.TriggerPopup(new Vector3(50, 50, -9.1f), "Use the left and right arrow keys to move and spacebar to drop.\r\nGive cupcakes to everyone!\r\n");
            freezeOverlay.SetActive(true);
 
        } else if (timesPlayed == 2)
        {
            tutorialPlaying=false;
            glitchFrequency = 0.3f;
            freezeOverlay.SetActive(false);
            spawner1.SetNumOfEnemies(25);
            spawner2.SetNumOfEnemies(20);
            
        } else // day 3; need a variable to determine which route to take
        {
            glitchFrequency=MainGameManager.GetComponent<GameManagerScript>().GlitchFreqFromEnding();
            tutorialPlaying=false;
            freezeOverlay.SetActive(false);
            spawner1.SetNumOfEnemies(27);
            spawner2.SetNumOfEnemies(18);

        }

        // Start glitch checking
        StartCoroutine(GlitchCheckRoutine());
    }

    // Update is called once per frame
    void Update()
    {

        if (gamePlaying) //camera moved over to minigame
        {
            if (tutorialPlaying == false)
            {
                InitializeCupcakeGame();
            } 
            else if (popup.transform.position == popupHome) //if tutorial left screen
            {
                tutorialPlaying = false;
                InitializeCupcakeGame();
            }

            if (!tutorialPlaying) //tutorial left screen or no tutorial
            {
                //InitializeCupcakeGame();
                freezeOverlay.SetActive(false);
                timeRemaining -= Time.deltaTime;
                timerText.text = "Time Left: " + Mathf.RoundToInt(timeRemaining);

                if (timeRemaining <= 0)
                {
                    GameOver();
                }
            }
        }


    }

    void InitializeCupcakeGame() {
        GameObject.Find("EnemySpawner").GetComponent<EnemySpawner>().playCupcakeMinigame = true;
        GameObject.Find("EnemySpawner (1)").GetComponent<EnemySpawner>().playCupcakeMinigame = true;
        GameObject.Find("cupcakeHolder").GetComponent<playerMovement>().playCupcakeMinigame = true;
        GameObject.Find("cupcakeHolder").GetComponent<Animator>().enabled = true;
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

        SpriteRenderer ovenSpriteR=oven.GetComponent<SpriteRenderer>(); // init to change oven
        ovenSpriteR.sprite = ovenGlitched;

        yield return new WaitForSeconds(glitchLength); // Glitch lasts 1 second
        
        // turn off glitches unless glitches are supposed to happen 100% 
        if (glitchFrequency != 1)
        {
            isGlitch = false;
            GlitchEffect.intensity = 0;
            AnalogGlitchEffect.colorDrift = 0;
            spriteRenderer.sprite = bgNormal;
            ovenSpriteR.sprite = ovenNormal;
        } 
    }

    private void GameOver()
    {
        gamePlaying = false;
        gameOver = true;
        tutorialPlaying = true;
        timerText.text = "Game Stop!";
        //Time.timeScale = 0f; // Stop everything

        //set the enemy spawners script to false
        GameObject.Find("EnemySpawner").GetComponent<EnemySpawner>().playCupcakeMinigame = false;
        GameObject.Find("EnemySpawner (1)").GetComponent<EnemySpawner>().playCupcakeMinigame = false;

        //TODO: calculate score to send to the hp based on the points earned in the round

        int score = ScoreCalculation(scorePoints);
        LeaderboardText.SetText(scorePoints + " Points!");
        MainGameManager.GetComponent<GameManagerScript>().RefreshLeaderboard(1, scorePoints);

        //Go back to the main script and say that the game is done:
        GameObject.Find("Game Manager").GetComponent<GameManagerScript>().CompletedMinigame(score);

        //disable oven
        GameObject.Find("cupcakeHolder").GetComponent<playerMovement>().playCupcakeMinigame = false;
        GameObject.Find("cupcakeHolder").GetComponent<Animator>().enabled = false;


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

    private int ScoreCalculation(int points) {
        float score;

        //these calculation are from LILY, based on the high score I got of 17 (so let 16 be a very good score)
        score = points * -11;
        score /= 16;
        
        //dont let score go beyond -12 (max)
        if (score < -12) 
        {
            score = -12;
        }

        return (int)score;
    }
}
