using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Kino;
using JetBrains.Annotations;

public class MinigameManager : MonoBehaviour
{
    public ComputerUIScript UIController;
    GameObject SpawnMg;
    GameObject MainGameManager;

    //stupid fix for camera going back to main desktop
    private Vector3 coinGameLocation = new Vector3(50, 35, -10);

    // Manages glitches 
    public float glitchFreq; // Set based on the day and situation
    public float glitchWaitTime = 1f;
    public float gameDur = 5f;
    public bool isGlitch;
    public float randomValue = 0f; // saves this variable so it can be also accessed for audio glitches in GameManagerScript

    // kino effect
    public DigitalGlitch GlitchEffect;
    public AnalogGlitch AnalogGlitchEffect;

    // Timer management 
    public TMP_Text timerText;
    private bool isGameOver;

    //Tutorial freeze management (Neila note: The code for it is bad but it works)
    private bool isTutorialPlaying; //checks if there is a tutorial on screen
    public GameObject popup;
    Vector3 popupHome;
    private bool hasInitialized; //checks if the game started game elements yet 
    public GameObject freezeOverlay;

    //Score management
    public TMP_Text scoreText;
    public int points;

    // Audio management
    public AudioSource goodCoinSFX;
    //public AudioClip normalSFX;
    public AudioClip[] goodCoinObtainedSFX;
    public AudioClip[] screamsSFX;
    public AudioSource badCoinSFX;
    public AudioClip badCoinObtainedSFX;
    public AudioClip[] trashHitSFX;
   

    // spawnables
    
    // General variables
    private int timesPlayed;
    // bg1, bg2 
    // public SpriteRenderer bg1Renderer;
    // public SpriteRenderer bg2Renderer;
    // Good spawnable
  
    // player color can change
    public SpriteRenderer playerRenderer;
    //public Color glitchPlayerColor = new Color(0f, 0f, 0f, 1f); // Black
    //private Color normalPlayerColor;
    public Animator playerAnimator;

    // player sprite can change
    //public Sprite pirateSprite;
    //public Sprite robotSprite;
    public AnimationClip pirateAnimation;
    public AnimationClip robotAnimation;

    //leaderboard text
    public TMP_Text LeaderboardText;

    // Speed variables
    public float scrollSpeed = 2f;
    public float speedIncreaseRate = 0.1f; // +0.1 speed per second
    public float maxSpeed = 6f;
    public float minSpeed = 1f;
    public float slowDownAmount = 1f;

    void Start()
    {
        UIController = GameObject.Find("UI Controller").GetComponent<ComputerUIScript>(); 
        MainGameManager = GameObject.Find("Game Manager");
        SpawnMg = GameObject.Find("SpawnMg");

        GameObject.Find("Player").GetComponent<Animator>().enabled = false;


        // SFX
        goodCoinSFX = GetComponent<AudioSource>();
        int index = Random.Range(0, goodCoinObtainedSFX.Length);
        goodCoinSFX.clip = goodCoinObtainedSFX[index]; // first sound by default

        badCoinSFX.clip = badCoinObtainedSFX;
    //  playerRenderer.sprite = pirateSprite; // Set starting sprite as pirate
        // normalPlayerColor = playerRenderer.color; // Set starting color as player's uncorrupted color

        isGameOver = true;
        points = 0;
        timesPlayed = 0;
        isGlitch = false;
        hasInitialized = false;

        //tutorial freeze
        popup = GameObject.Find("Popup");
        popupHome = new Vector3(0, 10, 0);

    }

    //This function is called by the AppScript and should start all the setup needed for the minigame
    public void StartCoinMinigame()
    {
        isGameOver = false;
        //InitializeCoinrunnerGame();

        points = 0;
        ++timesPlayed;

        //If it is the first time playing, tutorial popup amd no glitches
        if (timesPlayed == 1)
        {
            Debug.Log("First Time Playing CoinRunner");
            glitchFreq = 0.5f; // NEED TO CHANGE TO ZERO LATER!!
            UIController.TriggerPopup(new Vector3(50, 36, -8), "Use the up and down arrows to move lanes. Grab all the gold coins!");
            isTutorialPlaying = true;
            freezeOverlay.SetActive(true);
        } else if (timesPlayed == 2)
        {
            glitchFreq = 0.3f;
            isTutorialPlaying = false;
            InitializeCoinrunnerGame();
            
        } else // day 3; need a variable to determine which route to take (not anymore if glitching regardless)
        {
            glitchFreq = 1f;
            isTutorialPlaying = false;
            InitializeCoinrunnerGame();
            glitchFreq=MainGameManager.GetComponent<GameManagerScript>().GlitchFreqFromEnding();


        }


    }

    void Update() {
        Debug.Log("CoiNRunner " + isGameOver);
        if (!isGameOver) {
   
            CheckIfTutorialClosed();
            // Increase speed over time
            if (hasInitialized) { 
            scrollSpeed += speedIncreaseRate * Time.deltaTime;
            scrollSpeed = Mathf.Clamp(scrollSpeed, minSpeed, maxSpeed); // make sure speed is between set bounds
            }
        }
    }

    IEnumerator GlitchCheckRoutine()
    {
        while (!isGameOver)
        {
            yield return new WaitForSeconds(glitchWaitTime); // Check every second

            randomValue = Random.value;

            if (glitchFreq > 0 && randomValue < glitchFreq) // If we get a random value less than the glitch frequency, the glitches will occur
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

        Debug.Log("Robot");
        // playerRenderer.sprite = robotSprite;
        //playerRenderer.color = glitchPlayerColor;
        playerAnimator.Play("RobotAnimation");
        int screamIndex = Random.Range(0, screamsSFX.Length); // randomly choose a scream sfx
        goodCoinSFX.clip = screamsSFX[screamIndex];

        int trashIndex = Random.Range(0, trashHitSFX.Length); // randomly choose a trash hit sfx
        badCoinSFX.clip = trashHitSFX[trashIndex];
        // goodCoinSFX.clip = glitchedSFX;

        yield return new WaitForSeconds(1f); // Glitch lasts 1 second
        
        // Revert to normal if the glitch frequency isn't 100%
        if (glitchFreq != 1)
        {
           isGlitch = false;
            //     playerRenderer.sprite = pirateSprite;
           playerAnimator.Play("PirateAnimation");

           int goodCoinIndex = Random.Range(0, goodCoinObtainedSFX.Length); // randomly choose a good coin sfx
           goodCoinSFX.clip = goodCoinObtainedSFX[goodCoinIndex];
           
           badCoinSFX.clip = badCoinObtainedSFX;

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

        DisableCoinRunnerGame();

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
        //Let 16 be a reasonably good score to achieve -> update this once speed is added !!
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

    void CheckIfTutorialClosed()
    {
        //check if game initialised or in a tutorial
        if (!isGameOver&&!hasInitialized)
        {
            if (!isTutorialPlaying)
            {
                InitializeCoinrunnerGame();
                hasInitialized = true;
            }
            else if (popup.transform.position == popupHome) //if tutorial left screen
            {
                isTutorialPlaying = false;
                hasInitialized = true;
                InitializeCoinrunnerGame();
            }

        }
    }

    void InitializeCoinrunnerGame() {
        //start all game components to start game
        freezeOverlay.SetActive(false);
        // SpawnMg.GetComponent<spawnMg>().SpawnMgStart();
        StartCoroutine(GlitchCheckRoutine());
        StartCoroutine(StartMinigameTimer());
        GameObject.Find("SpawnMg").GetComponent<spawnMg>().startCoinMinigame = true;
        GameObject.Find("bg1").GetComponent<scrollBg>().startCoinMinigame = true;
        GameObject.Find("bg2").GetComponent<scrollBg>().startCoinMinigame = true;
        GameObject.Find("Player").GetComponent<Animator>().enabled = true;
    }

    void DisableCoinRunnerGame() {
        //disable all game components
        //freezeOverlay.SetActive(true);
        GameObject.Find("SpawnMg").GetComponent<spawnMg>().startCoinMinigame = false;
        GameObject.Find("bg1").GetComponent<scrollBg>().startCoinMinigame = false;
        GameObject.Find("bg2").GetComponent<scrollBg>().startCoinMinigame = false;
        GameObject.Find("Player").GetComponent<Animator>().enabled = false;
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

