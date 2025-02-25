using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Kino;

public class DuckGameManager : MonoBehaviour
{
    public TMP_Text scoreText;  // Reference to TextMeshPro to show the score
    private int score = 0;
    public bool gameOver;
    private int timesPlayed;
    public ComputerUIScript UIController;

    float timeRemaining;
    public float gameDuration = 30;

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

    void Start()
    {
        gameOver = true;
        score = 0;
        UpdateScoreText();
        timesPlayed = 0;
        UIController = GameObject.Find("UI Controller").GetComponent<ComputerUIScript>();

        // SFX
        audioSources = GetComponents<AudioSource>();
        audioSources[0].clip = normalSFX;
//      failureSFX = GetComponent<AudioSource>();
 //     failureSFX.clip = failureClipSFX;
    }

    void Update()
    {
        if (!gameOver)
        {
            timeRemaining -= Time.deltaTime;

            if (timeRemaining <= 0)
            {
                GameOver();
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

        StartCoroutine(GlitchCheckRoutine());

        if (timesPlayed == 1)
        {
            glitchFreq = 0;
            UIController.TriggerPopup(new Vector3(48f, 22.5f, -5), "Use the mouse to aim and left click to shoot.Take down all the ducks!");
        } else if (timesPlayed == 2)
        {
            glitchFreq = 0.3f;
        } else // day 3; need a variable to determine which route to take
        {

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

        //TODO: calculate score
        int scoreChange = -12; //for now just set it to the max

        //Tell the game manager that the game is over
        GameObject.Find("Game Manager").GetComponent<GameManagerScript>().CompletedMinigame(scoreChange);

        //Go to the leaderboard
        UIController.GetComponent<ComputerUIScript>().GoToPosition(new Vector3(90, 20, -10)); //go to the leaderboard

        score = 0;
        UpdateScoreText();
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
