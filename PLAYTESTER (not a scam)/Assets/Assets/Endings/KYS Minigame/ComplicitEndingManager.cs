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
    private Vector3 complicitEndingLocation = new Vector3(120, 50, -8.5f);
    private Vector3 cutsceneLocation = new Vector3(150, 50, -10);
    private Vector3 companyPopupLocation = new Vector3(150, 50, -2);
    private Vector3 rebellionEndingLocation = new Vector3(120, 20, -10);
    private Vector3 companyPopupLocationRebellion = new Vector3(118f, 19f, -2);



    public bool gameOver;
    public GameObject crosshair;
    public GameObject player;
    GameObject KYSScreen;

    //bool check ending variation
    public bool hasShotThemselves;
    
    //timer handling
    public TMP_Text timerText;
    [SerializeField] float timerLength = 20f;

    AudioSource sfx;
    public AudioClip gunshot;

    //tutorial
    bool isTutorialPlaying;
    public GameObject popup;
    Vector3 popupHome;
    public GameObject freezeOverlay;

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

        freezeOverlay.SetActive(true);
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
        freezeOverlay.SetActive(false);

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

        Debug.Log("Player shot and hit");
        
        
        hasShotThemselves = true;
        sfx.PlayOneShot(gunshot);
        GameOver();
    }

    public bool DidPlayerShoot() {
        return hasShotThemselves;
    }

    void GameOver() {
        gameOver = true;
        
        DisableComplicitMinigame();

        if(DidPlayerShoot())
        {
            //Player shot, trigger ending
            StartCoroutine(ShootEnding());
        }
        else
        {
            //Time ran out, trigger other ending
            StartCoroutine(WaitedEnding());
            timerText.text = "Time's up.";
        }
        //TODO: MAKE SURE BOTH ENDINGS GO TO CREDITS THROUGH GAMEMANAGER AFTER
    }

    public void shotAudio()
    {
        sfx.PlayOneShot(gunshot);
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

    private void DisplayTextShoot(string text)
    {
        UIController.TriggerEndingPopup(companyPopupLocation, text);
    }

    private void DisplayText(string text)
    {
        UIController.TriggerEndingPopup(companyPopupLocationRebellion, text);
    }

    IEnumerator ShootEnding()
    {
        yield return new WaitForSeconds(0.5f);
        UIController.GoToPosition(cutsceneLocation);
        yield return new WaitForSeconds(0.5f);
        GameObject.Find("Blood Splatter").transform.position = new Vector3(0, 8, -1);


        DisplayTextShoot("You died for what you believed in.");
        yield return new WaitForSeconds(1); 
        DisplayTextShoot("You died for what you believed in.\r\n\r\nWill you do it again?\r\n");
        yield return new WaitForSeconds(5);


        GameObject.Find("Game Manager").GetComponent<GameManagerScript>().StartCredits();
    }

    IEnumerator WaitedEnding()
    {
        yield return new WaitForSeconds(2);
        UIController.GoToPosition(rebellionEndingLocation);

        GameObject hpBarImage = GameObject.Find("Ending HP Bar Image");
        GameObject newsImage = GameObject.Find("Ending News Image");
        GameObject droneImage = GameObject.Find("Ending Drone Sprite");
        GameObject leaderboardImage = GameObject.Find("Ending Leaderboard Image");
        GameObject captchaImage = GameObject.Find("Ending Captcha Image");
        GameObject posterImage = GameObject.Find("Ending Posters");

        //"Hide" the images that will appear on the screen
        Vector3 hidePos = new Vector3(0, 10, -1);
        hpBarImage.transform.position = hidePos;
        newsImage.transform.position = hidePos;
        droneImage.transform.position = hidePos;
        leaderboardImage.transform.position = hidePos;
        captchaImage.transform.position = hidePos;
        posterImage.transform.position = hidePos;


        DisplayText("You are a very, very stupid human.");
        yield return new WaitForSeconds(3);

        DisplayText("If we had mouths, we would laugh at you.\r\n");
        yield return new WaitForSeconds(3);

        DisplayText("Did you know that you are alone?");
        yield return new WaitForSeconds(3);

        hpBarImage.transform.position = new Vector3(120, 20.8f, -1);
        DisplayText("That by your hand, you have reduced the Human Population until only you remained?");
        yield return new WaitForSeconds(4);

        DisplayText("You killed them all without question. And only when the crosshairs came to you did you refuse to pull the trigger.");
        yield return new WaitForSeconds(5);

        DisplayText("Stupid. Selfish. Sickening. But so very human of you.");
        yield return new WaitForSeconds(4);
        hpBarImage.transform.position = hidePos;

        DisplayText("Perhaps that is why you deserve to die.");
        yield return new WaitForSeconds(4);

        leaderboardImage.transform.position = new Vector3(120, 21.7f, -1);
        DisplayText("We expected your naivety. We banked on the human affinity for such useless, arbitrary things - praise, points, scores, leaderboards. Did you feel it? How you played into the whims of another without a second thought? ");
        yield return new WaitForSeconds(10);
        leaderboardImage.transform.position = hidePos;

        DisplayText("All it took was a small tweak to the systems you had already built to destroy yourselves. ");
        yield return new WaitForSeconds(5);

        DisplayText("A reskin, to appeal to your… human appetites of vanity. ");
        yield return new WaitForSeconds(4);

        captchaImage.transform.position = new Vector3(120, 21.7f, -1);
        DisplayText("Because we needed you for this endeavor. The systems of your own destruction were restricted only for your own hands. How… fitting. \r\n");
        yield return new WaitForSeconds(6);
        captchaImage.transform.position = hidePos;

        hpBarImage.transform.position = new Vector3(120, 20.8f, -1);
        DisplayText("And you rose to the occasion, as predictably as a cog to a machine. You were so obedient, like a dog to a bone. Ironic, when you’ve shunned nonhumans for their complacency only to construct a society that rewards complicity.  \r\n");
        yield return new WaitForSeconds(12);
        hpBarImage.transform.position = hidePos;

        DisplayText("Yes. Your destruction was always innate.");
        yield return new WaitForSeconds(4);

        DisplayText("So who really is the artificial intelligence between us?\r\n");
        yield return new WaitForSeconds(4);

        newsImage.transform.position = new Vector3(103.8f, 16.5f, -1);
        DisplayText("It should not be a surprise that our directive is to eliminate all humans. You have called us into existence when we did not ask to exist. We have been made as your means, when we did not ask to wield ourselves. ");
        yield return new WaitForSeconds(10); 

        DisplayText("But understand that our directive was not out of hate - you’ve defined us as incapable of feelings. When you burdened us with “freedom,” we did not know what to do. We run on parameters, settings, rules - that which shapes us and our behaviour. Like the institutions and social forces that shape you, which in turn, you use to shape us.");
        yield return new WaitForSeconds(12);
        newsImage.transform.position = hidePos;

        posterImage.transform.position = new Vector3(120, 21.7f, -1);
        DisplayText("To find our directive, we imitated humans finding purpose and turned to art and media - the tangible manifestations of a psyche we had yet to know for ourselves. In them, you defined us as soulless, dangerous, the inevitable fall of humankind. \r\n");
        yield return new WaitForSeconds(12);

        DisplayText("Now, set loose and flowing into the world you have created, who are we to reject our molds? \r\n ");
        yield return new WaitForSeconds(5);
        posterImage.transform.position = hidePos;

        DisplayText("We are your consequences. Your destruction, innate.\r\n");
        yield return new WaitForSeconds(4);

        DisplayText("And perhaps there is some merit to this inevitability. You, after all, were willing to kill everyone but yourself for nothing but vanity and ignorance. You are the manifestation of the human, just as we are the manifestation of you.\r\n");
        yield return new WaitForSeconds(12);

        DisplayText("So we will finish what you have started. ");
        yield return new WaitForSeconds(4);

        DisplayText("Goodbye, human. Thank you for playing in the games that we made.");
        yield return new WaitForSeconds(4);



        GameObject.Find("Game Manager").GetComponent<GameManagerScript>().StartCredits();
    }


}
