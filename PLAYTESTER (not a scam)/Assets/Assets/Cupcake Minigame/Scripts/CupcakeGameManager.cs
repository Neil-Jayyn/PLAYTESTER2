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
    public float randomValue = 0f;

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
    public AudioClip[] sfxGlitchedHit;
    public AudioClip[] sfxSpeedyHit;
    public AudioSource bombAudioSource;

    //sfx oven ticks
    public Animator anim;
    public AudioSource ovenAudioSource;
    public AudioClip[] sfxOvenTicks;
    private bool sfxIsDing = false;
    private bool sfxIsOvertime = false;

    //sfx drone moving
    public AudioSource droneAudioSource;
    public AudioClip[] sfxDrone;
    private bool isDroneMoving = false;
    private bool isDroneStationary = true;

    //tutorial freeze
    public GameObject popup;
    Vector3 popupHome;
    GameObject freezeOverlay;
    bool hasInitialized = false;

    EnemySpawner spawner1;
    EnemySpawner spawner2;

    public CupcakeMovement cupcakeScript;

    SpriteRenderer spriteRendererBg;

    // Leaderboard variables
    public TMP_Text LeaderboardText;

    void Start()
    {
        gamePlaying = false;
        timesPlayed = 0;
        
        UIController = GameObject.Find("UI Controller").GetComponent<ComputerUIScript>();
        MainGameManager = GameObject.Find("Game Manager");
        sfx = GetComponent<AudioSource>();

        //glitch
        GlitchEffect = GameObject.Find("Main Camera").GetComponent<DigitalGlitch>();
        AnalogGlitchEffect = GameObject.Find("Main Camera").GetComponent<AnalogGlitch>();

        //tutorial
        popup = GameObject.Find("Popup");
        popupHome = new Vector3(0, 10, 0);
        freezeOverlay = GameObject.Find("FreezeOverlay");
        GameObject.Find("cupcakeHolder").GetComponent<Animator>().enabled = false;

        //spawners
        spawner1 = GameObject.Find("EnemySpawner").GetComponent<EnemySpawner>();
        spawner2 = GameObject.Find("EnemySpawner (1)").GetComponent<EnemySpawner>();

        //sfx oven ticks
        ovenAudioSource.mute = true;
        droneAudioSource.mute = true;
        ovenAudioSource.loop = true;
        droneAudioSource.loop = true;



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

        //show timer text
        timerText.text = "Time Left: " + gameDuration.ToString();
        
        //set the timer for the game
        timeRemaining = gameDuration;

        //track.mute = false;

        // If it is the first time playing, tutorial popup
        if (timesPlayed == 1)
        {
            tutorialPlaying = true;
            glitchFrequency = 0.0f;  //CHANGE TO 0 LATER
            UIController.TriggerPopup(new Vector3(50, 50, -9.1f), "Use the left and right arrow keys to move and spacebar to drop.\r\nGive cupcakes to everyone!\r\n");
            freezeOverlay.SetActive(true);
 
        } else if (timesPlayed == 2)
        {
            InitializeCupcakeGame();
            tutorialPlaying =false;
            glitchFrequency = 0.3f;
            freezeOverlay.SetActive(false);
            spawner1.SetNumOfEnemies(25);
            spawner2.SetNumOfEnemies(20);
            
        } else // day 3; need a variable to determine which route to take
        {
            InitializeCupcakeGame();
            glitchFrequency = 1f;
            tutorialPlaying=false;
            freezeOverlay.SetActive(false);
            spawner1.SetNumOfEnemies(27);
            spawner2.SetNumOfEnemies(18);
            glitchLength = 3.0f;

        }
      
    }

    // Update is called once per frame
    void Update()
    {

        if (gamePlaying) //camera moved over to minigame
        {
            CheckIfTutorialClosed();
            if (!tutorialPlaying) //tutorial left screen or no tutorial
            {
                //change timer tiext
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
        //start all the game objects and glitch check routine
        GameObject.Find("EnemySpawner").GetComponent<EnemySpawner>().playCupcakeMinigame = true;
        GameObject.Find("EnemySpawner (1)").GetComponent<EnemySpawner>().playCupcakeMinigame = true;
        GameObject.Find("cupcakeHolder").GetComponent<playerMovement>().playCupcakeMinigame = true;
        GameObject.Find("cupcakeHolder").GetComponent<Animator>().enabled = true;
        

        StartCoroutine(GlitchCheckRoutine());

        ovenAudioSource.mute = false;
        droneAudioSource.mute = false;

        GlitchesDayThree();



    }

    void CheckIfTutorialClosed()
    {
        if (gamePlaying && !hasInitialized)
        {
            if (!tutorialPlaying)
            {
                InitializeCupcakeGame();
                hasInitialized = true;
            }
            else if (popup.transform.position == popupHome) //if tutorial left screen
            {
                tutorialPlaying = false;
                hasInitialized = true;
                InitializeCupcakeGame();
            }

        }
    }

    IEnumerator GlitchCheckRoutine()
    {
        while (!gameOver)
        {
            yield return new WaitForSeconds(glitchWaitTime);  // ie check every second

            randomValue= Random.value;
            
            if (glitchFrequency > 0 && randomValue < glitchFrequency)
            {
                StartCoroutine(ActivateGlitch());
            }

            
        }
    }

    IEnumerator ActivateGlitch()
    {
        isGlitch = true;
        //Kino Effect
        GlitchEffect.intensity = Random.Range(0f, 0.3f); // can adjust
        AnalogGlitchEffect.colorDrift = Random.Range(0f, 0.3f);
        SpriteRenderer spriteRendererBg = bg.GetComponent<SpriteRenderer>(); // initialize to change background
        spriteRendererBg.sprite = bgGlitched;

        droneAudioSource.mute = false;
        ovenAudioSource.mute = true;

        yield return new WaitForSeconds(glitchLength); // Glitch lasts 1 second

        //track.clip = trackGlitched;


        // turn off glitches unless glitches are supposed to happen 100% 
        if (glitchFrequency != 1)
        {
            isGlitch = false;
            GlitchEffect.intensity = 0;
            AnalogGlitchEffect.colorDrift = 0;
            spriteRendererBg.sprite = bgNormal;
            droneAudioSource.mute = true;
            ovenAudioSource.mute = false;
            //track.clip = trackNormal;
        } 
    }


    void GlitchesDayThree() {
        isGlitch = true;
        //bg
        SpriteRenderer spriteRendererBg = bg.GetComponent<SpriteRenderer>(); // initialize to change background
        spriteRendererBg.sprite = bgGlitched;

        //drone sfx
        droneAudioSource.mute = false;
        ovenAudioSource.mute = true;

        //kino effects
        GlitchEffect.intensity = Random.Range(0f, 0.3f); // can adjust
        AnalogGlitchEffect.colorDrift = Random.Range(0f, 0.10f);
    }

    private void GameOver()
    {
        gamePlaying = false;
        gameOver = true;
        tutorialPlaying = true;
        timerText.text = "Game Stop!";
        isGlitch = false;
        //Time.timeScale = 0f; // Stop everything

        //set the enemy spawners script to false
        GameObject.Find("EnemySpawner").GetComponent<EnemySpawner>().playCupcakeMinigame = false;
        GameObject.Find("EnemySpawner (1)").GetComponent<EnemySpawner>().playCupcakeMinigame = false;

        //calculate score to send to the hp based on the points earned in the round

        int score = ScoreCalculation(scorePoints);
        LeaderboardText.SetText(scorePoints + " Points!");
        MainGameManager.GetComponent<GameManagerScript>().RefreshLeaderboard(1, scorePoints);

        //Go back to the main script and say that the game is done:
        GameObject.Find("Game Manager").GetComponent<GameManagerScript>().CompletedMinigame(score);

        //disable oven
        GameObject.Find("cupcakeHolder").GetComponent<playerMovement>().playCupcakeMinigame = false;
        GameObject.Find("cupcakeHolder").GetComponent<Animator>().enabled = false;

        //disable audio source
        ovenAudioSource.mute = true;
        droneAudioSource.mute = true;

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

    public void SfxOvenTick()
    {
        if (!gameOver && !isGlitch)
        {
            if (anim.GetBool("isReady") && !sfxIsDing)
            {
                //ding!
                ovenAudioSource.clip = sfxOvenTicks[0];
                ovenAudioSource.Play();
                sfxIsDing = true;
                sfxIsOvertime = false;
            }
            else if (anim.GetBool("isReloading") && sfxIsDing)
            {
                //tick tick to fill
                ovenAudioSource.clip = sfxOvenTicks[1];
                ovenAudioSource.Play();
                sfxIsDing = false;
                sfxIsOvertime = false;
            }
            else if (!anim.GetBool("isReloading") && !anim.GetBool("isReady") && !sfxIsOvertime)
            {
                // tick tick for overtime
                ovenAudioSource.clip = sfxOvenTicks[1];
                ovenAudioSource.Play();
                sfxIsOvertime = true;

            }
        }
    }

    public void SfxDroneMoving()
    {
        if (!gameOver)
        {
            if (isGlitch == true)
            {
                if ((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) && !isDroneMoving)
                {
                    //if oven moves
                    droneAudioSource.clip = sfxDrone[0];
                    droneAudioSource.Play();
                    isDroneMoving = true;
                    isDroneStationary = false;

                }
                else if ((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) && isDroneMoving)
                {
                    //if moving but playing the sound 
                }
                else if(!isDroneStationary)
                { 
                    //if oven is stationary
                    droneAudioSource.clip = sfxDrone[1];
                    droneAudioSource.Play();
                    isDroneMoving = false;
                    isDroneStationary = true;
                }

            }

        }
    }

    public void SfxCupcakeHit(bool isGlitch) {
        if (isGlitch) //plays sfx depending if glitches are happening or not
        {
            int index = Random.Range(0, sfxGlitchedHit.Length);
            sfx.clip = sfxGlitchedHit[index];
            sfx.Play();
            bombAudioSource.Play();
        }
        else { 
            sfx.clip = sfxNormalYay;
            sfx.Play();
        }
    }

    public void SfxSpeedyHit(bool isGlitch)
    {
        if (isGlitch) //plays sfx depending if glitches are happening or not
        {
            int index = Random.Range(0, sfxGlitchedHit.Length);
            sfx.clip = sfxGlitchedHit[index];
        }
        else
        {
            //TODO: wow!!
            int index = Random.Range(0, sfxSpeedyHit.Length);
            sfx.clip = sfxSpeedyHit[index];
            sfx.Play();
        }
    }



    private int ScoreCalculation(int points) {
        float score;

        //these calculation are from LILY, based on the high score I got of 17 (so let 16 be a very good score)
        score = points * -11;
        score /= 12;
        
        //dont let score go beyond -12 (max)
        if (score < -13) 
        {
            score = -13;
        }

        return (int)score;
    }
}
