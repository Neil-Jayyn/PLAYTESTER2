using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ComplicitEndingManager : MonoBehaviour
{
    //complicit ending cutscene vars
    ComputerUIScript UIController;
    private Vector3 tempEndingLocation = new Vector3(150, 50, -10);
    private Vector3 complicitEndingLocation = new Vector3(120, 50, -8);

    public bool gameOver;
    public GameObject crosshair;
    public GameObject player;
    GameObject KYSScreen;

    //bool check ending variation
    public bool hasShotThemselves;
    
    //timer handling
    public TMP_Text timerText;
    [SerializeField] float timerLength = 10f;

    AudioSource sfx;
    public AudioClip gunshot;

    //tutorial
    bool isTutorialPlaying;
    public GameObject popup;
    Vector3 popupHome;

    // Start is called before the first frame update
    void Start()
    {
        //set vars
        gameOver = true;
        hasShotThemselves = false;

        crosshair.SetActive(false);
        player.SetActive(false);
        sfx=GetComponent<AudioSource>();

        UIController = GameObject.Find("UI Controller").GetComponent<ComputerUIScript>();
        KYSScreen = GameObject.Find("KYS Screen");

        //tut handling
        popup = GameObject.Find("Popup");
        popupHome = new Vector3(0, 10, 0);
    }

    public void StartComplicitMinigame()
    {
        Debug.Log("Started KYS mg");
        gameOver = false;
        isTutorialPlaying = true;

        //trigger tutorial popup
        UIController.TriggerPopup(complicitEndingLocation, "You know what to do.");
    }

    public void InitializeComplicitMinigame() {
        crosshair.SetActive(true);
        player.SetActive(true);
        StartCoroutine(StartTimer());
        KYSScreen.GetComponent<Animator>().enabled = true;
    }

    public void DisableComplicitMinigame() {
        gameOver = true;
        crosshair.SetActive(false);
        player.SetActive(false);
    }

    IEnumerator StartTimer() {

        float timer = timerLength;
        while (timer > 0 && !gameOver)
        {
            timer -= Time.deltaTime; // Update timer
            timerText.text = "Time: " + Mathf.RoundToInt(timer); // Display timer (rounded)
            yield return null;
        }

        GameOver();
    }



    public void PlayerShotThemself() {
        //called when player shoots themselves no problem
        hasShotThemselves = true;
        sfx.clip = gunshot;
        sfx.Play();
    }

    public bool DidPlayerShoot() {
        return hasShotThemselves;
    }

    void GameOver() {
        gameOver = true;
        timerText.text = "Time's up.";
        DisableComplicitMinigame();

        //TODO: Go to cutscene location for complicit endings
        //For now, it will go to temporary bin ending location
        UIController.GoToPosition(tempEndingLocation);
    }
    // Update is called once per frame
    void Update()
    {
        if (!gameOver) {
            if (popup.transform.position == popupHome) {
                isTutorialPlaying = false;
            }

            if (!isTutorialPlaying) {
                InitializeComplicitMinigame();
            }
        }

    }

}
