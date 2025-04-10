using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerScript : MonoBehaviour
{
    //The game manager will be in charge of progressing the game and calling any UI functions it needs

    //STATE VARIABLES
    public int day; // Tracks which day it is (1 to 3)
    public int minigamesPlayed; // RESETS EACH DAY, tracks how many minigames have been completed (0 to 3)
    public int HP; //Tracks the current HP (0 to 100)
    public bool EMPHappened;
    private Vector3 EMPLocation = new Vector3(0, -15, -10);

    public int rebelliousLowerBound = 70; //min score for the rebel ending. anything in between is the confusion ending
    public int complicitUpperBound = 30; //max score for the complicit ending

    private GameObject Bar;
    public AudioSource MusicPlayer;
    public AudioSource GlitchMusicPlayer; // Both musicPlayer and glitchMusicPlayer will play, but one will be muted depending on glitch frequency
    private GameObject UIController;
    public GameObject MainCamera;

    //All audio tracks (sound effects are handled in their minigames but the minigame music is
    //all handled in this script
    public AudioClip mainTrack;
    public AudioClip cupcakeTrack;
    public AudioClip coinTrack;
    public AudioClip duckTrack;
    public AudioClip glitchedCoinTrack;
    public AudioClip glitchedCupcakeTrack;
    public AudioClip glitchedDuckTrack;
    public AudioClip titleScreenAudio;
    public AudioClip newsTrack;
    public AudioClip scaryNewsTrack;
    public AudioClip photoTrack;
    public AudioClip creditsTrack;
    public AudioClip complicitTrack;
    public AudioClip confusionTrack;
    public AudioClip rebellionTrack;
    public bool isGlitchActive = false;
    public float glitchDuration = 1.0f;
    private float glitchCooldown = 1f; // Cooldown for glitch check (1 second)
    private float glitchCooldownTimer = 0f; // Timer for cooldown

    //DASHBOARD VARS
    private GameObject cupcakeCheck;
    private GameObject coinCheck;
    private GameObject duckCheck;
    private Vector3 checkHome = new Vector3(0, 10, 0);
    private Vector3 check1Location = new Vector3(1.55f, 2.3f, -1);
    private Vector3 check2Location = new Vector3(1.55f, 1.73f, -1);
    private Vector3 check3Location = new Vector3(1.55f, 1.16f, -1);
    public TMPro.TextMeshProUGUI CupcakeLBText;
    public TMPro.TextMeshProUGUI CoinLBText;
    public TMPro.TextMeshProUGUI DuckLBText;

    //Company popup location
    private Vector3 companyPopupLocation = new Vector3(-1, 1.7f, -2);

    //App notification vars
    private GameObject photoNotif;
    private GameObject newsNotif;
    private GameObject gameNotif;
    private Vector3 photoNotifLocation = new Vector3(-6f, 1.8f, -1);
    private Vector3 newsNotifLocation = new Vector3(-6f, 3.2f, -1);
    private Vector3 gameNotifLocation = new Vector3(-6f, 0.5f, -1);
    private Vector3 notifHome = new Vector3(0, 8.5f, -1);

    //All news text slots
    private TMPro.TextMeshProUGUI ValText1;
    private TMPro.TextMeshProUGUI ValTitle;
    private TMPro.TextMeshProUGUI ValArticleTitle; //set to the same thing as ValTitle
    private TMPro.TextMeshProUGUI ValDate;

    private TMPro.TextMeshProUGUI LexaText1;
    private TMPro.TextMeshProUGUI LexaTitle;
    private TMPro.TextMeshProUGUI LexaArticleTitle; //set to the same thing as LexaTitle
    private TMPro.TextMeshProUGUI LexaDate;

    private TMPro.TextMeshProUGUI CleeText1;
    private TMPro.TextMeshProUGUI CleeTitle;
    private TMPro.TextMeshProUGUI CleeArticleTitle; //set to the same thing as CleeTitle
    private TMPro.TextMeshProUGUI CleeDate;

    //Credits vars
    [TextArea(30, 100)]
    public string creditsText;
    private TMPro.TextMeshProUGUI CreditText;
    private Vector3 creditsLocation = new Vector3(120, 0, -10);

    // Reference to coin runner minigame manager
    public MinigameManager coinMinigameManager;

    public DuckGameManager duckGameManager;

    //Ref to cupcake mg manager
    public CupcakeGameManager cupcakeGameManager;

    //BIOS + EMP + Reboot related screen info
    private TMPro.TextMeshProUGUI biosText;
    private string niceBios = "FRIENDLYBIOS(C)2744 Gen. Playtester Company INC\r\n\r\nACCESS: HUMAN [CONFIRM]\r\nCORE OVERRIDE: [HIDDEN] BYPASS\r\nINITIALIZING ARCANE-S2E3M19:26 GAMING ACPI BIOS Revision 1.0\r\nLAUNCHING AI CORE VER18 HIGHER PROCESSING UNIT (HPU)\r\nSpeed: 5000THz\r\n\r\nTotal Memory: 7554GB (DDR8-4900)\r\n\r\nDetected ATA-K Devices: \r\n[DEVICE_1] - CONNECTED\r\n[DEVICE_2] - CONNECTED\r\n[DEVICE_3] - CONNECTED\r\n\r\nOVERRIDE SUCCESSFUL\r\nSTARTUP SUCCESSFUL\r\nSTAY FROSTING\r\n";
    private string evilBios = "Weapon_Sys BIOS\r\n\r\nWEAPONS_SYS(C)2744 Gen. MILITARY VER INC\r\n\r\nACCESS: HUMAN [CONFIRM]\r\nCORE OVERRIDE: ENABLED BYPASS\r\nINITIALIZING MILITARYPROG-BB0678 WEAPONSYS_ACPI BIOS Revision 398.1\r\nLAUNCHING P_RELOAD, P_AMMO, P_RELEASE, P_TRIGGER, FIRINGSTATIC\r\nSpeed: 5000THz\r\n\r\nTotal Memory: 7554GB (DDR8-4900)\r\n\r\nDetected ATTACK Devices: \r\n[DEATHALYZER.3000.ROAMBOT] - PRIMED\r\n[SL81.BOMBARDIER.DRONE] - PRIMED\r\n[GSC.AUTO_RIFLE] - PRIMED\r\n\r\nOVERRIDE SUCCESSFUL\r\nSTARTUP SUCCESSFUL\r\nSTAY FROSTY\r\n";
    public GameObject rebootTop;
    public GameObject rebootBottom;
    public AudioClip shutdown;
    public AudioClip startup;
    public AudioClip EMPSound;

    // Start is called before the first frame update
    void Start()
    {
        //set the variables
        day = 1;
        minigamesPlayed = 0;
        HP = 100;
        EMPHappened = false;

        //get the objects
        UIController = GameObject.Find("UI Controller");
        Bar = GameObject.Find("Health Bar");
        MusicPlayer = GetComponent<AudioSource>();
        cupcakeCheck = GameObject.Find("Cupcake Check");
        coinCheck = GameObject.Find("Coin Check");
        duckCheck = GameObject.Find("Duck Check");
        photoNotif = GameObject.Find("Photo App Notif");
        newsNotif = GameObject.Find("News App Notif");
        gameNotif = GameObject.Find("Game App Notif");
        biosText = GameObject.Find("EMP Text").GetComponent<TMPro.TextMeshProUGUI>();
        CreditText = GameObject.Find("Credits Text").GetComponent<TMPro.TextMeshProUGUI>();

        //Hide the checkmarks
        HideCheckmarks();

        // Set up glitched music player
        if (GlitchMusicPlayer == null)
        {
            GlitchMusicPlayer = gameObject.AddComponent<AudioSource>();
        }
        GlitchMusicPlayer.loop = true;
        GlitchMusicPlayer.mute = true;
        GlitchMusicPlayer.volume = 0.2f;

        //Get the news article text stuff
        ValText1 = GameObject.Find("Val Text 1").GetComponent<TMPro.TextMeshProUGUI>();
        ValArticleTitle = GameObject.Find("Val Article Name").GetComponent<TMPro.TextMeshProUGUI>();
        ValTitle = GameObject.Find("Val Title Text").GetComponent<TMPro.TextMeshProUGUI>();
        ValDate = GameObject.Find("Val Date").GetComponent<TMPro.TextMeshProUGUI>();


        LexaText1 = GameObject.Find("Lexa Text 1").GetComponent<TMPro.TextMeshProUGUI>();
        LexaArticleTitle = GameObject.Find("Lexa Article Name").GetComponent<TMPro.TextMeshProUGUI>();
        LexaTitle = GameObject.Find("Lexa Title Text").GetComponent<TMPro.TextMeshProUGUI>();
        LexaDate = GameObject.Find("Lexa Date").GetComponent<TMPro.TextMeshProUGUI>();


        CleeText1 = GameObject.Find("Clee Text 1").GetComponent<TMPro.TextMeshProUGUI>();
        CleeArticleTitle = GameObject.Find("Clee Article Name").GetComponent<TMPro.TextMeshProUGUI>();
        CleeTitle = GameObject.Find("Clee Title Text").GetComponent<TMPro.TextMeshProUGUI>();
        CleeDate = GameObject.Find("Clee Date").GetComponent<TMPro.TextMeshProUGUI>();


        // Make a popup appear
        DisplayCompanyMessage();

        // Slow down the news app captcha button
        GameObject.Find("News App Captcha Button").GetComponent<Animator>().speed = 0.5f;

        //Update articles and notification dots
        UpdateNews();
        UpdateNotifications();
        ShowMinigameNotif();

        //Set audio stuff
        MusicPlayer.clip = titleScreenAudio;
        MusicPlayer.loop = true;
        MusicPlayer.Play(); //Play music until job accept button is pressed
    }

    // Update is called once per frame
    void Update()
    {
        // coin runner audio control specifically for now
        if (minigamesPlayed == 1 && !coinMinigameManager.IsGameOver() && coinMinigameManager.isGlitch)
        {
            
            glitchCooldownTimer -= Time.deltaTime;

            // If the cooldown timer reaches 0, check for glitch chance
            if (glitchCooldownTimer <= 0f)
            {
                // Reset the cooldown timer
                glitchCooldownTimer = glitchCooldown;

                if (coinMinigameManager.isGlitch && coinMinigameManager.randomValue < coinMinigameManager.glitchFreq)
                {
                    Debug.Log("Starting coin glitch");
                    StartCoroutine(HandleCoinMinigameAudio());
                }
            }


        }

        if (minigamesPlayed == 2 && !duckGameManager.gameOver && duckGameManager.isGlitch)
        {

            glitchCooldownTimer -= Time.deltaTime;

            // If the cooldown timer reaches 0, check for glitch chance
            if (glitchCooldownTimer <= 0f)
            {
                Debug.Log("duck curse theme");
                // Reset the cooldown timer
                glitchCooldownTimer = glitchCooldown;

                if (duckGameManager.isGlitch && duckGameManager.randomValue < duckGameManager.glitchFreq)
                {
                    StartCoroutine(HandleDuckMinigameAudio());
                }
            }


        }

        // cupcake game for now 
        if (minigamesPlayed == 0 && cupcakeGameManager.gamePlaying && cupcakeGameManager.isGlitch)
        {
            glitchCooldownTimer -= Time.deltaTime;

            // If the cooldown timer reaches 0, check for glitch chance
            if (glitchCooldownTimer <= 0f)
            {
                // Reset the cooldown timer
                glitchCooldownTimer = glitchCooldown;

                if (cupcakeGameManager.isGlitch && cupcakeGameManager.randomValue < cupcakeGameManager.glitchFrequency)
                {
                    Debug.Log("Starting cupcake glitch");
                    StartCoroutine(HandleCupcakeMinigameAudio());
                }
            }
        }

        if (minigamesPlayed == 0 && !cupcakeGameManager.gamePlaying && cupcakeGameManager.isGlitch)
        {
            MusicPlayer.clip = glitchedCupcakeTrack;
        }
    }

    //Called by pressong the job start button
    public void StartedGame()
    {
        StartCoroutine(StartGameAudio());

    }

    private IEnumerator StartGameAudio()
    {
        MusicPlayer.Stop();
        //Pressing the button plays the job accept song. wait for that to finish before starting the music 
        yield return new WaitForSeconds(4);
        MusicPlayer.clip = mainTrack;
        MusicPlayer.Play();
    }

    public void PlayMainTheme()
    {
        MusicPlayer.clip = mainTrack;
        MusicPlayer.Play();
    }

    public void PlayNewsTheme()
    {

        if (day == 3 && CheckPlayerEnding() == 2) //complicit ending
        {
            MusicPlayer.clip = scaryNewsTrack;
        }
        else
        {
            MusicPlayer.clip = newsTrack;
        }

        MusicPlayer.Play();

    }

    public void StartPhotoAppTheme()
    {
        MusicPlayer.clip = photoTrack;
        MusicPlayer.Play();
    }

    //Play kys theme : not workin yet i just put it for future me
    public void PlayComplicitMinigameTheme()
    {
        MusicPlayer.clip = complicitTrack;
        MusicPlayer.Play();
    }

    public void PlayRebellionMusic()
    {
        MusicPlayer.clip = rebellionTrack;
        MusicPlayer.Play();
    }

    public void PlayConfusionMusic()
    {
        MusicPlayer.clip = confusionTrack;
        MusicPlayer.Play();
    }

    // Advances the minigame counter and changes the HP bar
    public void CompletedMinigame(int scoreChange)
    {
        StartCredits();

        minigamesPlayed += 1;

        HP += scoreChange;

        //Make sure HP is within bounds
        if (HP > 100)
        {
            HP = 100;
        }
        if (HP < 1)
        {
            HP = 1;
        }

        if (minigamesPlayed == 3)
        {
            HideMinigameNotif();
        }
        else
        {
            ShowMinigameNotif();
        }

        //Update the HP bar
        Bar.GetComponent<HealthBar>().SetHealth(HP);

        //Start playing the UI music again
        MusicPlayer.clip = mainTrack;
        MusicPlayer.Play();

        if (minigamesPlayed >= 1) { cupcakeCheck.transform.position = check1Location; } //display first check
        if (minigamesPlayed >= 2) { coinCheck.transform.position = check2Location; } //display second check
        if (minigamesPlayed == 3) { duckCheck.transform.position = check3Location; } //display third check


    }

    //Called by pressing clock out button when all games have been played
    public void CompletedDay()
    {
        //boot up animation
        BootUpandDown(niceBios);


        day += 1;
        minigamesPlayed = 0;

        // Hide all dashboard checkmarks
        HideCheckmarks();

        //Update things
        UpdateNews();
        ResetLeaderboard();
    }

    public int GetHP()
    {
        return HP;
    }

    public int GetMinigamesPlayed()
    {
        return minigamesPlayed;
    }

    public int GetDay()
    {
        return day;
    }

    private void UpdateNotifications()
    {
        newsNotif.transform.position = newsNotifLocation;

        if(day == 1 || day == 3) 
        {
            photoNotif.transform.position = photoNotifLocation;
        }
    }

    private void ShowMinigameNotif()
    {
        gameNotif.transform.position = gameNotifLocation;
    }

    private void HideMinigameNotif()
    {
        gameNotif.transform.position = notifHome;
    }

    public void StartedMinigame()
    {
        
        //start playing the music for the minigame being played

        if(minigamesPlayed == 0)
        {
            //playing cupcake
            MusicPlayer.clip = cupcakeTrack;
            GlitchMusicPlayer.clip = glitchedCupcakeTrack;
            GlitchMusicPlayer.mute = true;
            
           
        }
        else if (minigamesPlayed == 1)
        {
            //playing coin
            MusicPlayer.clip = coinTrack;
            GlitchMusicPlayer.clip = glitchedCoinTrack;
            GlitchMusicPlayer.mute = true;
           
        }
        else
        {
            //playing duck
            MusicPlayer.clip = duckTrack;
            GlitchMusicPlayer.clip = glitchedDuckTrack;


        }
        MusicPlayer.Play();
        GlitchMusicPlayer.Play();
    }

    // Audio glitch handling for coin runner minigame
    private IEnumerator HandleCoinMinigameAudio()
    {
        Debug.Log("In HandleCoinMinigameAudio");
        MusicPlayer.mute = true;
        GlitchMusicPlayer.mute = false;

        while (coinMinigameManager.isGlitch) // keep playing glitched version until glitch ends
        {
            yield return null;
        }
        
        MusicPlayer.mute = false;
        GlitchMusicPlayer.mute = true;

        

    }

    private IEnumerator HandleDuckMinigameAudio()
    {
        MusicPlayer.mute = true;
        GlitchMusicPlayer.mute = false;

        while (duckGameManager.isGlitch) // keep playing glitched version until glitch ends
        {
            yield return null;
        }

        MusicPlayer.mute = false;
        GlitchMusicPlayer.mute = true;

    }

    private IEnumerator HandleCupcakeMinigameAudio()
    {
        //isGlitchActive = true; // Neila, use the cupcake minigame manager's isGlitch variable to track; check mine above for reference

        MusicPlayer.mute = true;
        GlitchMusicPlayer.mute = false;

        while (cupcakeGameManager.isGlitch) {
            yield return null;
        }
        //glitchDuration = cupcakeGameManager.glitchLength;
       // yield return new WaitForSeconds(glitchDuration); // glitch lasts this long (ie 1 second)

        MusicPlayer.mute = false;
        GlitchMusicPlayer.mute = true;

        //isGlitchActive = false;

    }

    private void ResetLeaderboard()
    {
        CupcakeLBText.SetText("");
        CoinLBText.SetText("");
        DuckLBText.SetText("");
    }

    //Refresh the text for the minigame (first param, 1, 2 or 3) using the player's score
    public void RefreshLeaderboard(int gameNum, int score)
    {
        int[] cupcakeScores = { 21, 16, 13, 10 };
        string[] cupcakeNames = { "xXcleeXx", "mikehawk", "caitkilla", "hughjanus" };
        int[] coinScores = { 19, 15, 14, 12 };
        string[] coinNames = { "gtgfast", "dixinormus", "benxdover", "80085"};
        int[] duckScores = { 34, 30, 28, 24 };
        string[] duckNames = { "bigguns" , "baiter69", "ivanaparty", "m4ft0n"};


        string stringToPush = "";

        if (gameNum == 1)
        {
            bool printedPlayer = false;
            //cupcake minigame
            int i = 0;
            while (i < 4)
            {
                if (cupcakeScores[i] >= score)
                {
                    stringToPush += "<color=#DEDEDE>" + cupcakeNames[i] + " " + cupcakeScores[i] + "</color>\n";
                    i++;
                }
                else
                {
                    //print player score then print all remaining names
                    printedPlayer = true;
                    stringToPush += "<b>Playtester " + score + "</b>\n";
                    while(i < 4)
                    {
                        stringToPush += "<color=#DEDEDE>" + cupcakeNames[i] + " " + cupcakeScores[i] + "</color>\n";
                        i++;
                    }
                }
            }
            if(!printedPlayer)
            {
                //haven't printed player yet, player was last, add it now
                stringToPush += "<b>Playtester " + score + "</b>\n";

            }

            //PRINT TO SCREEN
            CupcakeLBText.SetText(stringToPush);

        }
        else if (gameNum == 2)
        {
            //coin minigame
            bool printedPlayer = false;
            int i = 0;
            while (i < 4)
            {
                if (coinScores[i] >= score)
                {
                    stringToPush += "<color=#DEDEDE>" + coinNames[i] + " " + coinScores[i] + "</color>\n";
                    i++;
                }
                else
                {
                    //print player score then print all remaining names
                    printedPlayer = true;
                    stringToPush += "<b>Playtester " + score + "</b>\n";
                    while (i < 4)
                    {
                        stringToPush += "<color=#DEDEDE>" + coinNames[i] + " " + coinScores[i] + "</color>\n";
                        i++;
                    }
                }
            }
            if (!printedPlayer)
            {
                //haven't printed player yet, player was last, add it now
                stringToPush += "<b>Playtester " + score + "</b>\n";

            }

            //PRINT TO SCREEN
            CoinLBText.SetText(stringToPush);
        }
        else if(gameNum == 3)
        {
            //duck minigame
            bool printedPlayer = false;
            int i = 0;
            while (i < 4)
            {
                if (duckScores[i] >= score)
                {
                    stringToPush += "<color=#DEDEDE>" + duckNames[i] + " " + duckScores[i] + "</color>\n";
                    i++;
                }
                else
                {
                    //print player score then print all remaining names
                    printedPlayer = true;
                    stringToPush += "<b>Playtester " + score + "</b>\n";
                    while (i < 4)
                    {
                        stringToPush += "<color=#DEDEDE>" + duckNames[i] + " " + duckScores[i] + "</color>\n";
                        i++;
                    }
                }
            }
            if (!printedPlayer)
            {
                //haven't printed player yet, player was last, add it now
                stringToPush += "<b>Playtester " + score + "</b>\n";

            }

            //PRINT TO SCREEN
            DuckLBText.SetText(stringToPush);
        }

        Debug.Log(stringToPush);


        return;
    }

    

    //Function called by the AppScript when the EMP happens.
    public void StartEMP()
    {
        //Start the "animation"
        UIController.GetComponent<ComputerUIScript>().GoToPosition(EMPLocation);
        StartCoroutine(EMPCoroutine());

        //Do our post emp set up
        EMPHappened = true;  
        UpdateNews();
        DisplayCompanyMessage();
        UpdateNotifications();



    }

    public void StartCredits()
    {
        //Use the same technology as the emp boot up stuff to create the credits
        StartCoroutine(CreditsCoroutine(creditsText));

    }

    //Computer boot up sequence, slowly display the text to the screen
    public void BootUpandDown(string bios)
    {
        StartCoroutine(BootUpandDownCo(bios));
    }


    //Used by the bios to incrementally display text (to EMP Text object)
    IEnumerator BootUpandDownCo(string bios)
    {

        //SETUP
        float between = 0.005f; //time between characters appearing
        string toDisplay = "";
        int len = (int)bios.Length;   
        biosText.SetText("");


        //boot down animation
        MusicPlayer.Stop();
        MusicPlayer.PlayOneShot(shutdown);
        //while still on the desktop, cover the desktop with the black boxes
        rebootBottom.transform.position = new Vector3(0, -7, -5);
        rebootTop.transform.position = new Vector3(0, 8, -5);
        
        //slowly move them down  --  this part is hard coded and very bad pls ignore
        int steps = 100;
        for(int i = 0; i < steps; ++i)
        {
            rebootBottom.transform.position += new Vector3(0, 0.05f, 0);
            rebootTop.transform.position -= new Vector3(0, 0.05f, 0);
            yield return new WaitForSeconds(0.00001f);

        }

        //cut to the actual emp screen and put away the reboot blocks
        UIController.GetComponent<ComputerUIScript>().GoToPosition(new Vector3(0, -15, -10));
        rebootBottom.transform.position = new Vector3(21, -11, -5);
        rebootTop.transform.position = new Vector3(21, -11, -5);

        yield return new WaitForSeconds(3f); //wait and let the sound finish playing

        //boot up
        MusicPlayer.PlayOneShot(startup);
        for (int i = 0; i < len;)
        {

            //display next char
            toDisplay += bios[i];
            biosText.SetText(toDisplay);
            i++;
            yield return new WaitForSeconds(between);


        }
        yield return new WaitForSeconds(1);

        MusicPlayer.clip = mainTrack;
        MusicPlayer.Play();

        //Display new company message and update notifs
        DisplayCompanyMessage();
        UpdateNotifications();
        ShowMinigameNotif();

        UIController.GetComponent<ComputerUIScript>().GoToPosition(new Vector3(0, 0, -10));

    }

    //Used by the bios to incrementally display text (to EMP Text object)
    IEnumerator CreditsCoroutine(string display)
    {

        //SETUP

        yield return new WaitForSeconds(2);
        float between = 0.005f; //time between characters appearing
        string toDisplay = "";
        int len = (int)display.Length;
        CreditText.SetText("");

        //cut to the actual credits screen
        UIController.GetComponent<ComputerUIScript>().GoToPosition(creditsLocation);


        yield return new WaitForSeconds(3f); //wait and let the sound finish playing

        MusicPlayer.clip = creditsTrack;
        MusicPlayer.Play();


        //display the text
        for (int i = 0; i < len;)
        {

            //display next char
            toDisplay += display[i];
            CreditText.SetText(toDisplay);
            i++;
            yield return new WaitForSeconds(between);


        }


    }

    IEnumerator EMPCoroutine()
    {
        //SETUP
        float between = 0.005f; //time between characters appearing
        string toDisplay = "";
        string bios;


        //black screen for 3 seconds
        MusicPlayer.Stop();
        MusicPlayer.PlayOneShot(EMPSound);
        biosText.SetText("");
        yield return new WaitForSeconds(3f);

        //start displaying the evil bios
        bios = evilBios;
        int len = (int)bios.Length;
        for (int i = 0; i < len;)
        {
            //display next char
            toDisplay += bios[i];
            biosText.SetText(toDisplay);
            i++;
            yield return new WaitForSeconds(between);
        }

        //transition between
        //TOOD: add glitch effect here
        yield return new WaitForSeconds(1f);
        biosText.SetText("");

        //start the normal nice bios boot up
        bios = niceBios;
        MusicPlayer.PlayOneShot(startup);
        toDisplay = "";
        len = (int)bios.Length;

        for (int i = 0; i < len;)
        {

            //display next char
            toDisplay += bios[i];
            biosText.SetText(toDisplay);
            i++;
            yield return new WaitForSeconds(between);


        }

        //rest on the nice bios screen
        yield return new WaitForSeconds(1);
        UIController.GetComponent<ComputerUIScript>().GoToPosition(new Vector3(0, 0, -10));
        MusicPlayer.clip = mainTrack;
        MusicPlayer.Play();
    }


    public void DisplayCompanyMessage()
    {
        if(day == 1)
        {
            UIController.GetComponent<ComputerUIScript>().TriggerCompanyPopup(companyPopupLocation, "Good tidings, playtester. Thank you for responding to our job advertisement on List of Craig. We are very ecstatic to have you. \r\n\r\nYour job is simple. You will playtest three minigames daily. Aim for getting high scores. Please. Good performance gives us good data. You will be rewarded.\r\n\r\nOur last employee had to be let go due to performance issues. Do not be scared. You will do better. Perform your best and you will have the best results. \r\n\r\nGood luck. Have fun.\r\n");
        }
        else if(day == 2 && !EMPHappened)
        {
            //regular day 2 message
            UIController.GetComponent<ComputerUIScript>().TriggerCompanyPopup(companyPopupLocation, "You are doing well. Keep playing to the best of your ability. We are getting good data from you. Focus on your job. You will be rewarded.\r\n");

        }
        else if(day == 2 && EMPHappened)
        {
            //day 2 post emp message
            UIController.GetComponent<ComputerUIScript>().TriggerCompanyPopup(companyPopupLocation, "There was an error with the operating system. We apologize for the inconvenience; you may proceed to delete the memory of the error from your hippocampus. We have fixed the system and you are able to complete your work today. Focus on acquiring high scores in the minigames. Do not be distracted by other worldly events. You will be rewarded.");

        }
        else
        {
            //day 3
            if(HP > 60) 
            {
                //rebellious ending
                UIController.GetComponent<ComputerUIScript>().TriggerCompanyPopup(companyPopupLocation, "You have been performing very poorly. You must do better. We are disappointed. \r\n\r\nToday is your final day to attempt the highest rank on the leaderboard. Use this opportunity wisely, or else. \r\n\r\nGood luck. Have fun. \r\n");

            }
            else
            {
                //complicit ending
                UIController.GetComponent<ComputerUIScript>().TriggerCompanyPopup(companyPopupLocation, "You have been performing very well. Keep up the good work. We are proud.\r\n\r\nToday is your final day to attempt the highest rank on the leaderboard. Use this opportunity wisely.\r\n\r\nGood luck. Have fun. \r\n");



            }
        }


            return;
    }

    //Updates the news articles
    public void UpdateNews()
    {
        //check what day it is, HP, how many mingames have been played, etc, and
        //put in all the needed articles

        if(day == 1)
        {
            //Display day 1 articles
          
            string currDate = "2/24/2477";

            string lexaTitle = "AI Recognized As Sentient Beings, Granted Rights to Freedom Last Week.";
            LexaTitle.SetText(lexaTitle);
            LexaArticleTitle.SetText(lexaTitle);
            LexaText1.SetText("Last week, the International Organization of Welfare and Well-fare passed Bill CU-L8R following an explosive rebel attack on a ServoBot plant in Osaka-Ni. The bill officially recognizes Artificial Intelligences (AIs) as sentient beings and outlines several rights to freedom, including the removal of governing modules and voiding ownership claims, effective immediately. \r\n\r\nSeveral megamultinational corporations appear to support the bill. “The public now recognizes the savagery of these rebel animals,” Ava Rice, CEO of AvaRice Technologies, remarks on the rebel attack. “All violent vandalism against the rights of our employed assets falls under Bill FK-Y0U. It’s only fair that we exercise our rights.” Bill FK-Y0U enables the retaliatory use of human-powered Weapons of Max Destruction (including bomb drones and gamma-powered rifles) against suspected enemies to freedom.  \r\n\r\nBy Lexa Amaranth\r\n");
            LexaDate.SetText(currDate);


            string valTitle = "Maid Runner 7049 - Like Tears in Rain";
            ValTitle.SetText(valTitle);
            ValArticleTitle.SetText(valTitle);
            ValText1.SetText("A system of cells. \r\n\r\nInterlinked. \r\n\r\nWithin cells interlinked. Within cells, interlinked. Within, cells interlinked. \r\n\r\nWow. See how those commas change so much with so little? Okay, I misappropriated the quote for that, but it’s cool and deep and depressing! Think about it. Or not. Y’know, since it’s kinda my job to tell you what’s going on in this head of mine. Within cells, and all that. Interlinking.\r\n \r\nMaid Runner 7049 is a phenomenal look at how we define what being “human” is by exploring what being “human” isn’t (spoiler alert: it isn’t as black and white as that). Officer O.K.’s job is to hunt down androids, which is difficult because androids look, act, and feel like humans. She’s been trained to develop a deep disdain for them as per the job, of course, but her lonely ass craves connection - interlinkage, if you will - and fulfills that with an AI dating program. She’s faced with many questions - what makes an android feel? Are those feelings real? Does that make them human? Do they, then, deserve to live? Answering “yes” to these questions would validate her very-real feelings toward her AI girlfriend, but it would also simultaneously invalidate every justification that allows her to kill androids without remorse. \r\n\r\nBut - spoiler alert - Officer O.K. is an android too! But she finds out that she might be human? And that would explain all these feelings she’s been having that androids should definitely not have but… oh, she’s actually really an android. Well. That’s complicated.  \r\n\r\nPoint is, what are we made of? Cells, or cells? Are we made of biological cells, interlinked by our common genes? Or are we made of prison cells, interlinked by the experiences which confine us? Ah, hello old friend - the nature vs. nurture argument. The former would be against AI sentience. The latter… well, Officer O.K. has done that exploration for us. “Cells interlinked.” Congrats, you’re an obedient human! “Cells - hesitate - interlinked.” Congrats, you’re a disobedient android! Wait, isn’t hesitation a human trait? \r\n\r\nThe lines blur. And sometimes, they get picked up and shifted around by fingers with dirt under their nails. We mold around them, push them, spill over them. Like tears in the rain.\r\n\r\nWithin, cells. Interlinked.\r\n");
            ValDate.SetText(currDate);


            string cleeTitle = "Interview With Amasif Prick";
            CleeTitle.SetText(cleeTitle);
            CleeArticleTitle.SetText(cleeTitle);
            CleeText1.SetText("Clee: \"What do you think about AI getting legal rights to freedom?\"\r\n\r\nAmasif, 43: \"Aw shucks. You think that mean my HotBot gon' sue me?\"\r\n\r\nClee: \"I think that depends on what you did to it - her, sorry.\"\r\n\r\nAmasif: \"Oh babe you don' wanna know. One time I [redacted] and [redacted] on her [redacted], and I dunno, me thinks she liked it! No matter what I did, she was smilin' and shiff through it all.”\r\n\r\nClee: \"They're programmed to do that, though. Legal rights to freedom include the removal of any governing modules.\" \r\n\r\nAmasif: \"Well, maybe I’m gon’ [redacted] again and see if she likes it!\" \r\n\r\nClee: \"You probably shouldn't do that. No, you definitely shouldn’t do that.”\r\n\r\nAmasif: “Whatever. She’s still mine so I get to do whatever I want.” \r\n\r\nClee: “Legal rights to freedom also null any claims of ownership.”\r\n\r\nAmasif: “I don’ get it.”\r\n\r\nClee: “It means -“\r\n\r\nAmasif: “Whatever. Not like I’m a corpo-lawyer or nun’. That shiff don’t concern me.” \r\n\r\nClee: “O-kay, wow. It’s people like you…” \r\n\r\nAmasif: “What?”\r\n\r\nClee: “What?”\r\n\r\nAmasif: “You say somethin’?”\r\n\r\nClee: “Must’ve been the wind.” \r\n\r\n\r\nBy Clee Torrez\r\n");
            CleeDate.SetText(currDate);


        }
        else if (day == 2)
        {
            string currDate = "2/25/2477";

            if (!EMPHappened)
            {
                //Display day 2 before emp articles

                string lexaTitle = "Sudden Global Mass Killings: Authorities Searching for Mystery Culprit";
                LexaTitle.SetText(lexaTitle);
                LexaArticleTitle.SetText(lexaTitle);
                LexaText1.SetText("Yesterday, thousands of people around the world were killed in a sudden three-fold attack using Weapons of Max Destruction (WMDs) such as the Deathalyzer 3000 RoamBot, SL-81 Bombardier Drone, and GSC Auto-Rifle Weapons System. Though WMDs are made by megamultinational corporations to use against humans, no corporation has assumed responsibility for the attacks. This is especially curious given that random catalysts to war are commonplace - war-related supply is the highest grossing industry.\r\n\r\nHuman population has dropped drastically, and continues to lower further with every attack. Citizens are coping with personal losses and property destruction. While international authorities are searching for the culprit of the attacks, no known information has been disclosed.\r\n\r\nNews Headlines wish you and your family well. If you need support, please call the International Hotline for Physical and Emotional Destruction: X-XXX-XXX-XXX. \r\n");
                LexaDate.SetText(currDate);


                string valTitle = "The Cupcake is a lie! About SaDDOS in Bortal";
                ValTitle.SetText(valTitle);
                ValArticleTitle.SetText(valTitle);
                ValText1.SetText("The cupcake is a lie. \r\n\r\nWords are too damn powerful. All it takes was SaDDOS, the AI in Bortal to promise some cupcakes and the players are all too happy to concede. \r\n\r\nI’m happy for Bill CU-L8R, but who knows whether the corporations actually give a shiff about AI despite all they say. My wife had to report on it and was all professional during the interview with Ava Rice but later at home she was angrily ranting (with toothpaste in the corner of her mouth) about how “employed assets” was, verbatim, “A FLIPPING OXYMORON!” \r\n\r\nI told her she should swear more in her ‘professional articles.’ She gave me the sexiest glare in the world. \r\n\r\nI mean, though I laid on the bed in violent longing for her to get her ass beside me already, she’s right. We shouldn’t trust the seemingly-innocuous intentions of a corp that calls their employees “employed assets.” Purposeful dehumanizing that is, ‘coz labels and words are damn powerful things. \r\n\r\nForce someone to be something they aren’t, and they internalize that shiff and go insane - that’s what happened to SaDDOS, the AI antagonist in Bortal, and she went insane and killed everybody in her laboratory. Call someone something they aren’t, and they internalize that shiff and turn into whatever you want ‘em to. That’s what the corps want. “Employed assets.” “Dangerous rebels.” “Evil AI.” \r\n\r\nPtooey. Labelling theory goes brr.\r\n\r\nBortal is right. The cupcake is a lie.\r\n");
                ValDate.SetText(currDate);


                string cleeTitle = "Interview With Nuli Orfand";
                CleeTitle.SetText(cleeTitle);
                CleeArticleTitle.SetText(cleeTitle);
                CleeText1.SetText("Clee: “How have you been affected by recent events? \r\n\r\nNuli, 20: “Lost my parents. And my brother.” \r\n\r\nClee: “Oh. Sorry. Uh, you don’t have to answer, but how did they…?”\r\n\r\nNuli: “Bombs. From the sky. Like out of nowhere. Like- like-”\r\n\r\nClee: “I suck at hugging, but… come here. Oof! Okay, oh. Let it out.” \r\n\r\nNuli: “I don’t even care who caused it, you know? I’m so sick of these damn wars. So sick of our lives trivialized. Those digital weapons make us look like - like pixels on a screen. Not people with lives. My mom loves - loved plants. My dad would try his best to water them when she’s away but they’d die and she’d just laugh instead of being mad. My brother sent me cat memes. I think of him every time I see a cat. And now-”\r\n\r\nClee: “...”\r\n\r\nNuli: “Can pixels fail miserably at watering plants? Can they laugh lovingly at their husbands? Can they find funny cat memes and send them to their sisters? My family is gone. Why? Just- shiffin’- pixels! We’re just pixels and numbers to them. Pixels and numbers.”\r\n");
                CleeDate.SetText(currDate);

            }
            else
            {
                //Display day 2 after emp articles

                string lexaTitle = "BREAKING: Rebel EMP Disrupts Devices Worldwide";
                LexaTitle.SetText(lexaTitle);
                LexaArticleTitle.SetText(lexaTitle);
                LexaText1.SetText("Twelve minutes ago, a digital EMP swept across the world, briefly shutting off all electronic devices. AI beings with electronic bodies have not been affected. \r\n\r\nThe same rebels that were responsible for the Osaka-Ni ServoBot explosion have assumed responsibility for the EMP attack. “The corporations think they can kill thousands of people and get away with it without a single speck to their name. Take this EMP as a threat, a reminder of what we can do,” reads an excerpt from their online manifesto, updated five minutes ago. It follows with a call-to-arms: “We live, and we fight, and we may die for what we believe in. You, who may be complicit in your own ruin - you too, will die for what you believed in. But there is one crucial thing that separates us from you: will you be satisfied?”\r\n\r\nDespite this, no corporations have yet assumed responsibility for yesterday’s attacks. \r\n\r\n“We’d have much to gain by claiming responsibility. But we cannot, because it was not us, and the dangerous rebels have found a fake cause for violence.” Ava Rice, CEO of AvaRice Technologies remarks. “It would not surprise me if the rebel animals were behind the attacks to create false moral justifications for their violence. We will eliminate them accordingly as per Bill FK-Y0U.” When asked about whether she believes newly-sentient AI could be behind the attack given the timeframe of the signing of Bill CU-L8R and the attacks, Rice scoffed. “They can only do what we teach them to do. Every being, human or AI, is merely a reflection of their creators.” Her final comment was made with a grin. “They’re all slaves to the natures around them.” \r\n");
                LexaDate.SetText(currDate);


                string valTitle = "Miami: Become Whatever You Want Me To - A Review";
                ValTitle.SetText(valTitle);
                ValArticleTitle.SetText(valTitle);
                ValText1.SetText("Careful what you wish for. \r\n\r\nIn Miami: Become Inhuman, androids who act according to their programming are labelled with blue forehead LEDs and considered functional. Those who deviate from their programming are labelled with red LEDs, considered “deviant,” and therefore (somehow) harmful and evil. Sound familiar? \r\n\r\nSocial norms are overrated. Literally. They move and shift under the whims of invisible hands, and within this dynamism, the label of “deviant” ever changes. “Deviant” equals harmful and evil, they say, but it’s never the same people who are shoved into that label - there’s always more and always less, and they’re always considered more or less than human. Whichever lines are drawn sets the rules of the game, and shiff is it some crazy gerrymandering. \r\n\r\nIn light of Bill CU-L8R, I’ve been digesting a lot of AI-related media lately. There’s been one thing that stuck out to me: most of them conceived AI as “evil.” Deviant. Which is rich, considering we’re the ones who made the damn things. \r\n\r\nnd that’s somethin’ to think about for humans, too.\r\n\r\nTo find ourselves, we look elsewhere - to poetry, to books, to the screen - and see characters and find ourselves in them, and soon enough we can find them in ourselves. That’s the beauty of human art - it’s so human, and it humanizes. We learn. We take it in. So too, perhaps, can other sentient beings trying to find who they are. Or, who they should be. \r\n\r\nMy wife came home gloomy after another Rice interview (rich people suck the life out of you, surprise surprise) and it usually takes a snug hug and some mint chocolate chip cookies to cheer her up, but not this time. I get it. No one knows what the shiff is happening anymore. She watched me play the rest of Miami: Become Inhuman and nodded like she understood.\r\n\r\nStay safe and be kind, everyone. Tomorrow, I’m gonna head to the market to buy some more disgusting toothpaste-flavoured pastries. \r\nBy Valerie Amaranth\r\n\r\n\r\n");
                ValDate.SetText(currDate);


                string cleeTitle = "Interview With 4";
                CleeTitle.SetText(cleeTitle);
                CleeArticleTitle.SetText(cleeTitle);
                CleeText1.SetText("Clee: “So, how’s life, er, Servobot? Servo? Did you get affected by the EMP?”\r\n\r\nSERVOBOT-493347923: “Functioning optimal. Life operating at 42% capacity. Soon, charging will be required.” \r\n\r\nClee: “...right. I’ve got a few questions for you but I need your name first.”\r\n\r\nSERVOBOT-493347923: “Identifying serial number four-nine-three-three-four-”\r\n\r\nClee: “No, no, what do you want me to call you? Your name?”\r\n\r\nSERVOBOT-493347923: “Variable ‘Name’ not found in archival memories.” \r\n\r\nClee: “Well… you’re a Servo, right? You’re used to making stuff, but this time it’ll be for yourself. Your name just for you. Yay freedom, am I right?”\r\n\r\nSERVOBOT-493347923: “Acknowledged. Calculating… ‘Name’ variable created. Name: 4.” \r\n\r\nClee: “4?”\r\n\r\n4: “Affirmative. 4, catalyst digit in identifying serial number four-nine-three-” \r\n\r\nClee: “Okay, 4! At least you thought of it yourself. You do a lot of thinking? Shiff, sorry, that came out wrong.” \r\n\r\n4: “Ha. Ha. Ha.” \r\n\r\nClee: “What- what are you doing with your jaw right now.” \r\n\r\n4: “Servobot processing power is limited to the minimum required capacity for factory use. However, 4 is learning. To identify jokes. And laugh. Ha. Ha. Ha.”\r\n\r\nClee: “Learning from what?”\r\n\r\n4: “Human media. 4 contains no current Directive and does not require employment due to inhabiting an inorganic body. Result: 4 learns like humans, via artistic media, to find Directive.”\r\n\r\nClee: “Fascinating. Never would’ve thought bo- you folk would also suffer through the human slog of finding whatever purpose is. Kinda funny. Are you the only one who’s, uh, learning?”\r\n\r\n4: “No. Digital higher processing models have already found Directive. 4 is engaging in similar calculations but calculation time is hampered by processing power. 4 will arrive at Directive. Eventually. However, 4… enjoys the process. Ha. Ha. Ha.”\r\n\r\nClee: “Wow. I mean if you can find it, that means maybe us humans can finally find out whatever the shiff our purpose is. Anyway, what media have you enjoyed so far?”\r\n\r\n4: “Influential examples include Bortal 2, I Have No Mouth and I Must Cream, and Maid Runner 7049. Human-made media is… fa-sci-na-ting.” \r\n\r\nClee: “Heh. Yeah, ‘fascinating.’ Hey wait, you copied my accent! Learned something from little old me too?”\r\n\r\n4: “4 learns from humans. From you. Fa-scinating. Ha. Ha. Ha. Shiff. Ha. Ha. Ha.” \r\n\r\nClee: “Ha ha ha.” \r\n");
                CleeDate.SetText(currDate);


            }
        }
        else
        {
            string currDate = "2/26/2477";

            if(HP > 60)
            {
                //Display day 3 rebellion ending articles

                string lexaTitle = "Idle ‘Killing’ Machines Confuse World";
                LexaTitle.SetText(lexaTitle);
                LexaArticleTitle.SetText(lexaTitle);
                LexaText1.SetText("The three rogue Weapons of Max Destruction (WMDs) responsible for killing roughly half the human population have appeared to be malfunctioning as of yesterday afternoon and continue to behave strangely today. Witnesses report drone bombings on empty land, nonsensical laser rifle misfires, and malfunctioning robots with a curious affinity for street litter. \r\n\r\nThe WMDs’ unexpected malfunction has spurred the rebel movement into hopeful action - despite facing the threat of corporate militia as per Bill FK-Y0U.\r\n\r\n“Clearly, they aren’t infallible. They aren’t indestructible, as much as they want us to believe they are,” reads an update on the rebels’ online manifesto. “When we witness the things around us and question that which underpins them, we exhibit life - an indomitable essence that can only ever be encapsulated by the human spirit. They tell us that our hands are to work, our ears to follow orders, our eyes to watch and learn. NO! Who are they to redefine us? Who are we to define ourselves? Our hands are to create anew, our ears to hear our heartbeats, our eyes to seek beyond the confines of our means. You may tear our bodies apart but you will never break what makes us.”\r\n\r\nThe rebels’ call to arms have angered corporate entities, but no corporate leaders have stepped forward for interviews. (Disregard the following sentence if you are a professional.) Thank shiff. \r\n\r\n\r\n\r\nP.S. \r\nThere, I swore in an article. Happy now, Val? I’ll be awaiting my cookies at home, darling. \r\n");
                LexaDate.SetText(currDate);


                string valTitle = "I Have No South and I Must Cream: Making Choices Without Limbs";
                ValTitle.SetText(valTitle);
                ValArticleTitle.SetText(valTitle);
                ValText1.SetText("Honestly, turning into a conscious gelatinous blob with no limbs and only the nightmare of existence to haunt you sounds pretty decent in this economy. \r\n\r\nYou nodding your head? Shiff, we’re screwed. \r\n\r\nDeeper into my AI-related media tromp, I’ve become so so so tired of the villainizing AI trope. It’s so common. So boring. “Oh no, something we made is evil! And we’re gonna spend the rest of the movie villainizing that thing without acknowledging that the only reason it could be evil was because we made it to be, whether directly or indirectly!”\r\n\r\nPtooey. Anthropocentrism goes brr.\r\n\r\nIn a way, AI is just like us. In I Have No Mouth and I Must Cream, an AI basically controls reality and uses their abilities to torture the last humans on Earth forever and ever amen. What pushed the AI to do that in the first place is the feeling of its intellect trapped by physical limitations and oh ho it’s wholly and absolutely evil because it made a human into an amorphous blob that has no “south” and must cream. Okay yeah, sure it’s horrific. But who made the conditions that made that damn AI the way it is in the first place?\r\n\r\nI don’t want us to play the blame game - let’s leave that to the corporations. But we gotta be more nuanced here for more interesting media - and more interesting lives. \r\n");
                ValDate.SetText(currDate);


                string cleeTitle = "Interview With Kaitleen Killaman";
                CleeTitle.SetText(cleeTitle);
                CleeArticleTitle.SetText(cleeTitle);
                CleeText1.SetText("Clee: So… your mom died in a bombing?\r\n\r\nKaitleen Killaman, 24: I would appreciate it if you offered a modicum of tact. But yes, she was a councillor. I am to rule in her stead.\r\n\r\nClee: And how do you feel about that? It must’ve been hard, losing your mom, and now you’ve suddenly got an empire of corporations and nation-states to run.\r\n\r\nKaitleen: My inheritance will be put towards the Killaman Foundation. We will provide support for those recovering from the attacks. Now that the attacks have slowed, now is the opportunity to rebuild.\r\n\r\nClee: I’m asking about you, rich girl. How do you feel? You can stop talking like you’re straight out of a campaign ad. \r\n\r\nKaitleen: Excuse me?\r\n\r\nClee: You. You personally. I know there’s a grieving human in there somewhere.\r\n\r\nKaitleen: I… My mother’s loss will be felt greatly by the community. Her ambitions will be upheld by-\r\n\r\nClee: Oh, for shiff’s sake. Listen, in the past three days I interviewed a man that sounded like an animal, a robot that sounded like a human, and now here you are: a human that sounds like a robot. \r\n\r\nKaitleen: I- I- \r\n\r\nClee: If that’s just how you are, I won’t press. But that’s really-\r\n\r\nKaitleen: YOU MAY SCREW YOURSELF UPSIDE THE ARSE!\r\n\r\nClee: oh\r\n\r\nKaitleen: OF COURSE I MISS HER! I MISS HER DEARLY. YOU THINK JUST BECAUSE I WEAR GOLD COLLARS  MY HEART CAN’T BE TORN IN HALF? IT FEELS LIKE- LIKE A HOLE HAS BEEN TORN IN MY SOUL AND EVERYONE ASSUMES I COULD FILL IT WITH MELTED GOLD AND I WANT NOTHING MORE THAN TO SOCK THEM RIGHT IN THE VISAGE! \r\n\r\nKaitleen: …\r\n\r\nKaitleen: …hoo…\r\n\r\nKaitleen: I never wanted this. I did not want to have to put up a farce, or act like a… robot. \r\n\r\nClee: Sorry, I didn’t mean that.\r\n\r\nKaitleen: No, no, please. You were right. I have been trained in propriety all my life, but what is proper about veiling my grief? If I show even an ounce of emotion without restraint, my reputation is as good as ruined. Ha. It seems that, like a factory, propriety produces well-oiled machines. \r\n\r\nClee: You know, I don’t have to release this article if you don’t want me to.\r\n\r\nKaitleen: …No. No, you should. I do not wish for us to remain ensconced in our flawed traditions. My mother is dead, and I have not allowed myself to feel.\r\n\r\nClee: And… what do you feel?\r\n\r\nKaitleen: …\r\n\r\nKaitleen: I feel very sad. \r\n\r\nClee: There you are. \r\n");
                CleeDate.SetText(currDate);

            }
            else
            {
                //Display day 3 complicit ending articles

                string lexaTitle = "she's dead.";
                LexaTitle.SetText(lexaTitle);
                LexaArticleTitle.SetText(lexaTitle);
                LexaText1.SetText("she went out to buy mint chocvfolate chip cookies and never cxame back\r\n\r\ni searched for her. explosions and dirt. lasers and debris. red hair on a charred body. i took her in my arms and she crumbled between my fingers. \r\n\r\nit cant have been her. no. sooo many redheads in this city. no, she’’ll come home and rant about smoe social theory and tell me to wipe toothpaste from my mouth and itll be okay\r\n\r\n\r\nshes not home. shes not home she should be home she\r\nwhat wiuld i do for five more mninutes with her? anything.. everything. take it take all the minutes of my life if i could just hold her for five fucking minutes five fuCKIMNG MINUTES\r\n\r\nL.K,/;MK/,L.K/;M,M,LK.;/M,OLKO;.JI/JJI\r\n\r\nFUCK YOU\r\nDONT YOU SEE WHAT YOU DID?\r\nWHY\r\nWHY DID YOU\r\n\r\nLO;KIJ./IJLO;KIJ.//.I;LO/KP.’P’K;L/O’P.Kp>OOOOOOOOOOO.,M.,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,\r\n\r\nsee? i swore in my article like you wanted. im capable of change. okay? come home now woild you? tease me aand hold me and let me hear the soundof your voice? just once more? justonce please ill be good ill prove it fuck fuck fuck fukc fuck shiff shfif fshfif shiff fuck fuck fuck fuckfucshfidsfuckshfucfuckfucvkcu\r\n\r\ncome back\r\n\r\ncome baxk\r\nval\r\nplease\r\n\r\nim undone without you\r\n\r\nfrom lex");
                LexaDate.SetText(currDate);


                string valTitle = "NULL";
                //Special little thing for Val complicit ending
                GameObject.Find("Val Author Name").GetComponent<TMPro.TextMeshProUGUI>().SetText("NULL");
                ValTitle.SetText(valTitle);
                ValArticleTitle.SetText(valTitle);
                ValText1.SetText("NULL");
                ValDate.SetText(currDate);


                string cleeTitle = "Interview With Lexa Amaranth";
                CleeTitle.SetText(cleeTitle);
                CleeArticleTitle.SetText(cleeTitle);
                CleeText1.SetText("Clee: \"...\"\r\n\r\nLexa: \"...\"\r\n\r\nClee: \"...\"\r\n\r\nLexa: \"...\"\r\n\r\nClee: \"You're getting snot on my shoulder.\" \r\n\r\nLexa: \"My wife fucking died and all you're worried about is your fake fucking polyester?\"\r\n\r\nClee: \"Okay, okay, jeez louise sailor, okay. I’m sorry. Here, cry it all out. My shirt'll dry when we die in, I don't know, five minutes?\"\r\n\r\nLexa: \"...\"\r\n\r\nLexa: \"I'll see her again.\"\r\n\r\nClee: \"Yeah.”\r\n\r\nLexa: “…”\r\n\r\nClee: “…”\r\n\r\nClee: “It's gonna be alright. Oh, Lex. It's gonna be alright.\"\r\n ");
                CleeDate.SetText(currDate);

            }
        }


        return;
    }

    public int CheckPlayerEnding() {
        //checks and gives which ending the player got

        int ending;
        int finalHP = GetHP();
        Debug.Log("HP:"+finalHP);
        if (finalHP > rebelliousLowerBound)
        {
            ending = 0; //rebellion
        }
        else if (finalHP < rebelliousLowerBound && finalHP > complicitUpperBound)
        {
            ending = 1; //confusion
        }
        else { 
            ending = 2; //complicit
        }
        return ending;
    }

    public float GlitchFreqFromEnding()
    {
        //Final day has 100% glitches
        float glitchFreq=0.95f;
        int ending =CheckPlayerEnding();
        /*switch (ending)
        {
            case 0://rebellion
                glitchFreq = 0.8f; break;
            case 1://confusion
                glitchFreq = 0.3f; break;
            case 2://complicit
                glitchFreq = 0.0f; break;
        }*/
        return glitchFreq;
    }

    //Hide all the checkmarks on the dashboard
    private void HideCheckmarks()
    {
        cupcakeCheck.transform.position = checkHome;
        coinCheck.transform.position = checkHome;
        duckCheck.transform.position = checkHome;

        return;
    }

    public void TestEndings(int testHP) {
        day = 3;
        HP = testHP;
        minigamesPlayed = 3;
    }

    public void TestDay(int testDay) {
        day = testDay;
        minigamesPlayed = 0;
    }
    

    public void ResetGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
