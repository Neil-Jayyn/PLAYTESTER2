using System.Collections;
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
    private bool isTutorialPlaying; //checks if there is a tutorial on screen
    public GameObject popup;
    Vector3 popupHome;
    private bool hasInitialized; //checks if the game started game elements yet 
    GameObject freezeOverlay;
    GameObject player;


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

        //freeze variables initialized
        hasInitialized = false;
        popup = GameObject.Find("Popup");
        popupHome = new Vector3(0, 10, 0);
        freezeOverlay = GameObject.Find("FreezeOverlay (2)");
        GameObject.Find("DuckPlayer").GetComponent<SpriteRenderer>().enabled = false;
        

    }

    void Update()
    {
        if (!gameOver) //camera moved over to duck game
        {

            if (!hasInitialized) //if game didnt start yet
             {
                if (popup.transform.position == popupHome)
                { //if popup closed
                    isTutorialPlaying = false;
                    hasInitialized = true;
                    InitializeDuckGame();
                }
                else if (!isTutorialPlaying) { 
                    InitializeDuckGame(); 
                    hasInitialized=true; 
                }
            }

            if (!isTutorialPlaying) //start game when tutorial is closed or when no tutorial
            {
                timeRemaining -= Time.deltaTime;
                timerText.text = "Time Left: " + Mathf.RoundToInt(timeRemaining);

                if (timeRemaining <= 0)
                {
                    GameOver();
                }
            }
        }
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
           
            isTutorialPlaying = true;
            Debug.Log("first time playing");
            glitchFreq = 0;
            UIController.TriggerPopup(new Vector3(48f, 22.5f, -9.1f), "Use the mouse to aim and left click to shoot.Take down all the ducks!");
        } else if (timesPlayed == 2)
        {
            isTutorialPlaying = false;
            glitchFreq = 0.3f;
            //InitializeDuckGame();
            
        } else // day 3; need a variable to determine which route to take
        {
            isTutorialPlaying = false;
            glitchFreq=MainGameManager.GetComponent<GameManagerScript>().GlitchFreqFromEnding();
            //GlitchFreqFromEnding();
            //InitializeDuckGame();

        }
    }

    private void InitializeDuckGame() {
        //initialize game objects for game
        freezeOverlay.SetActive(false);
        GameObject.Find("SpawnManager").GetComponent<SpawnManager>().SpawnStart();
        GameObject.Find("SpawnManagerFast").GetComponent<SpawnManagerFast>().SpawnStart();
        GameObject.Find("DuckPlayer").GetComponent<SpriteRenderer>().enabled = true;
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

        //disable crosshair
        GameObject.Find("DuckPlayer").GetComponent<SpriteRenderer>().enabled = false;

        //Update leaderboard text
        LeaderboardText.SetText(score + " Points!");

        //Go to the leaderboard
        UIController.GetComponent<ComputerUIScript>().GoToPosition(new Vector3(90, 20, -10)); //go to the leaderboard

        freezeOverlay.SetActive(true);
        hasInitialized = false;


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

}
