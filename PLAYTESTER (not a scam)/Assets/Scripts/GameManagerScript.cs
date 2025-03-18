﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    //The game manager will be in charge of progressing the game and calling any UI functions it needs

    //STATE VARIABLES
    public int day; // Tracks which day it is (1 to 3)
    public int minigamesPlayed; // RESETS EACH DAY, tracks how many minigames have been completed (0 to 3)
    public int HP; //Tracks the current HP (0 to 100)
    public bool EMPHappened;
    private Vector3 EMPLocation = new Vector3(0, -15, -10);

    private GameObject Bar;
    public AudioSource MusicPlayer;
    public AudioSource GlitchMusicPlayer; // Both musicPlayer and glitchMusicPlayer will play, but one will be muted depending on glitch frequency
    private GameObject UIController;

    //All audio tracks (sound effects are handled in their minigames but the minigame music is
    //all handled in this script
    public AudioClip mainTrack;
    public AudioClip cupcakeTrack;
    public AudioClip coinTrack;
    public AudioClip duckTrack;
    public AudioClip glitchedCoinTrack;
    public bool isGlitchActive = false;
    public float glitchDuration = 1.0f;
    private float glitchCooldown = 1f; // Cooldown for glitch check (1 second)
    private float glitchCooldownTimer = 0f; // Timer for cooldown

    //DASHBOARD VARS
    private GameObject cupcakeCheck;
    private GameObject coinCheck;
    private GameObject duckCheck;
    private Vector3 checkHome = new Vector3(0, 10, 0);
    private Vector3 check1Location = new Vector3(1.55f, 2.3f, 0);
    private Vector3 check2Location = new Vector3(1.55f, 1.73f, 0);
    private Vector3 check3Location = new Vector3(1.55f, 1.16f, 0);
    public TMPro.TextMeshProUGUI CupcakeLBText;
    public TMPro.TextMeshProUGUI CoinLBText;
    public TMPro.TextMeshProUGUI DuckLBText;

    //Company popup location
    private Vector3 companyPopupLocation = new Vector3(-1, 1.7f, -2);


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

    // Reference to coin runner minigame manager
    public MinigameManager coinMinigameManager;

    //BIOS and EMP related screen info
    private TMPro.TextMeshProUGUI biosText;
    private string niceBios = "FRIENDLYBIOS(C)2744 Gen. Playtester Company INC\r\n\r\nACCESS: HUMAN [CONFIRM]\r\nCORE OVERRIDE: [HIDDEN] BYPASS\r\nINITIALIZING ARCANE-S2E3M19:26 GAMING ACPI BIOS Revision 1.0\r\nLAUNCHING AI CORE VER18 HIGHER PROCESSING UNIT (HPU)\r\nSpeed: 5000THz\r\n\r\nTotal Memory: 7554GB (DDR8-4900)\r\n\r\nDetected ATA-K Devices: \r\n[DEVICE_1] - CONNECTED\r\n[DEVICE_2] - CONNECTED\r\n[DEVICE_3] - CONNECTED\r\n\r\nOVERRIDE SUCCESSFUL\r\nSTARTUP SUCCESSFUL\r\nSTAY FROSTING\r\n";
    private string evilBios = "Weapon_Sys BIOS\r\n\r\nWEAPONS_SYS(C)2744 Gen. MILITARY VER INC\r\n\r\nACCESS: HUMAN [CONFIRM]\r\nCORE OVERRIDE: ENABLED BYPASS\r\nINITIALIZING MILITARYPROG-BB0678 WEAPONSYS_ACPI BIOS Revision 398.1\r\nLAUNCHING P_RELOAD, P_AMMO, P_RELEASE, P_TRIGGER, FIRINGSTATIC\r\nSpeed: 5000THz\r\n\r\nTotal Memory: 7554GB (DDR8-4900)\r\n\r\nDetected ATTACK Devices: \r\n[DEATHALYZER.3000.ROAMBOT] - PRIMED\r\n[SL81.BOMBARDIER.DRONE] - PRIMED\r\n[GSC.AUTO_RIFLE] - PRIMED\r\n\r\nOVERRIDE SUCCESSFUL\r\nSTARTUP SUCCESSFUL\r\nSTAY FROSTY\r\n";


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
        biosText = GameObject.Find("EMP Text").GetComponent<TMPro.TextMeshProUGUI>();

        //Hide the checkmarks
        HideCheckmarks();

        //Set the audio stuff
        MusicPlayer.clip = mainTrack;
        MusicPlayer.loop = true;
        MusicPlayer.Play();

        // Set up glitched music player
        if (GlitchMusicPlayer == null)
        {
            GlitchMusicPlayer = gameObject.AddComponent<AudioSource>();
        }
        GlitchMusicPlayer.loop = true;
        GlitchMusicPlayer.mute = true;

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


        // TO START: make a popup appear
        DisplayCompanyMessage();

        // Slow down the title screen animation, news app captcha button
        GameObject.Find("PlayTesterAnimation").GetComponent<Animator>().speed = 0.5f;
        GameObject.Find("News App Captcha Button").GetComponent<Animator>().speed = 0.5f;

        //Update news articles
        UpdateNews();

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

                if (!isGlitchActive && Random.value < coinMinigameManager.glitchFreq) 
                {
                    Debug.Log("Starting coin glitch");
                    StartCoroutine(HandleCoinMinigameAudio());
                }
            }
        }
    }

    // Advances the minigame counter and changes the HP bar
    public void CompletedMinigame(int scoreChange)
    {

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

        //Update the HP bar
        Bar.GetComponent<HealthBar>().SetHealth(HP);

        //Start playing the UI music again
        MusicPlayer.clip = mainTrack;
        MusicPlayer.Play();

        if (minigamesPlayed >= 1) { cupcakeCheck.transform.position = check1Location; } //display first check
        if (minigamesPlayed >= 2) { coinCheck.transform.position = check2Location; } //display second check
        if (minigamesPlayed == 3) { duckCheck.transform.position = check3Location; } //display third check

        StartCoroutine(SlowDisplayGoodAndEvilBios());
    }

    //Called by pressing clock out button when all games have been played
    public void CompletedDay()
    {
        day += 1;
        minigamesPlayed = 0;

        //TODO: Hide all dashboard checkmarks
        HideCheckmarks();

        //Update news articles
        UpdateNews();

        //Display new company message
        DisplayCompanyMessage();
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

    public void StartedMinigame()
    {
        
        //start playing the music for the minigame being played

        if(minigamesPlayed == 0)
        {
            //playing cupcake
            MusicPlayer.clip = cupcakeTrack;
           
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
            
        }
        MusicPlayer.Play();
        GlitchMusicPlayer.Play();
    }

    // Audio glitch handling for coin runner minigame
    private IEnumerator HandleCoinMinigameAudio()
    {
        isGlitchActive = true;

        MusicPlayer.mute = true;
        GlitchMusicPlayer.mute = false;

        yield return new WaitForSeconds(glitchDuration); // glitch lasts this long (ie 1 second)

        MusicPlayer.mute = false;
        GlitchMusicPlayer.mute = true;

        isGlitchActive = false;

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
                    stringToPush += cupcakeNames[i] + " " + cupcakeScores[i] + "\n";
                    i++;
                }
                else
                {
                    //print player score then print all remaining names
                    printedPlayer = true;
                    stringToPush += "Playtester " + score + "\n";
                    while(i < 4)
                    {
                        stringToPush += cupcakeNames[i] + " " + cupcakeScores[i] + "\n";
                        i++;
                    }
                }
            }
            if(!printedPlayer)
            {
                //haven't printed player yet, player was last, add it now
                stringToPush += "Playtester " + score + "\n";

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
                    stringToPush += coinNames[i] + " " + coinScores[i] + "\n";
                    i++;
                }
                else
                {
                    //print player score then print all remaining names
                    printedPlayer = true;
                    stringToPush += "Playtester " + score + "\n";
                    while (i < 4)
                    {
                        stringToPush += coinNames[i] + " " + coinScores[i] + "\n";
                        i++;
                    }
                }
            }
            if (!printedPlayer)
            {
                //haven't printed player yet, player was last, add it now
                stringToPush += "Playtester " + score + "\n";

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
                    stringToPush += duckNames[i] + " " + duckScores[i] + "\n";
                    i++;
                }
                else
                {
                    //print player score then print all remaining names
                    printedPlayer = true;
                    stringToPush += "Playtester " + score + "\n";
                    while (i < 4)
                    {
                        stringToPush += duckNames[i] + " " + duckScores[i] + "\n";
                        i++;
                    }
                }
            }
            if (!printedPlayer)
            {
                //haven't printed player yet, player was last, add it now
                stringToPush += "Playtester " + score + "\n";

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
        //TODO: Add in here the audio cutting out or other effects

        EMPHappened = true;
        UIController.GetComponent<ComputerUIScript>().GoToPosition(EMPLocation);
        UpdateNews();
        DisplayCompanyMessage();

        StartCoroutine(SlowDisplayGoodAndEvilBios());

    }

    //Computer boot up sequence, slowly display the text to the screen
    public void BootUpSequence(string bios)
    {
        
        StartCoroutine(SlowDisplayText(bios));

        //Go back to the main screen

    }

    //Used by the bios to incrementally display text (to EMP Text object)
    IEnumerator SlowDisplayText(string bios)
    {
        float between = 0.005f; //time between characters appearing


        string toDisplay = "";
        int len = (int)bios.Length;

        for (int i = 0; i < len;)
        {

            //display next char
            toDisplay += bios[i];
            biosText.SetText(toDisplay);
            i++;
            yield return new WaitForSeconds(between);


        }
        yield return new WaitForSeconds(1);
        UIController.GetComponent<ComputerUIScript>().GoToPosition(new Vector3(0, 0, -10));

    }

    IEnumerator SlowDisplayGoodAndEvilBios()
    {
        float between = 0.005f; //time between characters appearing

        string bios = evilBios;

        string toDisplay = "";
        int len = (int)bios.Length;

        for (int i = 0; i < len;)
        {

            //display next char
            toDisplay += bios[i];
            biosText.SetText(toDisplay);
            i++;
            yield return new WaitForSeconds(between);


        }

        yield return new WaitForSeconds(1f);
        biosText.SetText("");

        bios = niceBios;

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

        yield return new WaitForSeconds(1);
        UIController.GetComponent<ComputerUIScript>().GoToPosition(new Vector3(0, 0, -10));

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
            //TODO: add more cases here for the company popups once the writing is in the drive
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


            string valTitle = "I Have No Mouth and I Must Cream: Making Choices Without Limbs";
            ValTitle.SetText(valTitle);
            ValArticleTitle.SetText(valTitle);
            ValText1.SetText("Honestly, turning into a conscious gelatinous blob with no limbs and only the nightmare of existence to haunt you sounds pretty decent in this economy. \r\n\r\nYou nodding your head? Shiff, we’re screwed. \r\n\r\nSo often does the media villainize AI and it is such a tiring trope. “Oh no, something we made is evil! And we’re gonna spend the rest of the movie villainizing that thing without acknowledging that the only reason it could be evil was because we made it to be, whether directly or indirectly!”\r\n\r\nPtooey. Anthropocentrism goes brr.\r\n\r\nIn a way, AI is just like us. I can even go all out social constructivist and say that its environment plays a pretty big part in its behaviour. What pushed the AI to cruelty against the last humans on Earth in “I Have No Mouth and I Must Cream” is the feeling of its intellect trapped by physical limitations and oh ho it’s wholly and absolutely evil because it made a protagonist into an amorphous blob. Okay yeah, sure it’s horrific. But who made the conditions that made that damn AI the way it is in the first place?\r\n\r\nI don’t want us to play the blame game - let’s leave that to the corporations. But we gotta be more nuanced here for more interesting media - and more interesting lives. \r\n\r\nBy Valerie Amaranth\r\n");
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
                LexaText1.SetText("Yesterday, thousands of people around the world were killed in a sudden three-fold attack using Weapons of Max Destruction (WMDs) such as the Deathalyzer 3000 RoamBot, SL-81 Bombardier Drone, and GSC Auto-Rifle Weapons System. Though WMDs are made by megamultinational corporations to use against humans, no corporation has assumed responsibility for the attacks. This is especially curious given that random catalysts to war are commonplace - war-related supply is the highest grossing industry.\r\n\r\nCitizens are coping with personal losses and property destruction. While international authorities are searching for the culprit of the attacks, no known information has been disclosed.\r\n\r\nNews Headlines wish you and your family well. If you need support, please call the International Hotline for Physical and Emotional Destruction: X-XXX-XXX-XXX. \r\n\r\n\r\nBy Lexa Amaranth\r\n");
                LexaDate.SetText(currDate);


                string valTitle = "The Cupcake is a lie! About SaDDOS in Bortal";
                ValTitle.SetText(valTitle);
                ValArticleTitle.SetText(valTitle);
                ValText1.SetText("The cupcake is a lie. \r\n\r\nWords are too damn powerful. All it takes was SaDDOS, the AI in “Bortal” to promise some cupcakes and the players are all too happy to concede. \r\n\r\nI’m happy for Bill CU-L8R, but who knows whether the corporations actually give a shiff about AI despite all they say. My wife had to report on it and was all professional during the interview with Ava Rice but later at home she was very passionately ranting about how “employed assets” was, verbatim, “A FLIPPING OXYMORON!”\r\n\r\nI mean, though I laid on the bed in violent longing for her to get her ass beside me already, she’s right. (And so sexy when she gets hot and bothered about politics with toothpaste still smattered over the corner of her lips.) We shouldn’t trust the seemingly-innocuous intentions of a corp that calls their employees “employed assets.” Purposeful dehumanizing that is, ‘coz labels and words are damn powerful things.\r\n\r\nForce someone to be something they aren’t, and they internalize that shiff and go insane - that’s what happened to SaDDOS, the AI antagonist in Bortal, and she went insane and killed everybody in her laboratory. Call someone something they aren’t, and they internalize that shiff and turn into whatever you want ‘em to. That’s what the corps want. “Employed assets.” “Dangerous rebels.” “Evil AI.” \r\n\r\nPtooey. Labelling theory goes brr.\r\n\r\nBortal is right. The cupcake is a lie.\r\n\r\nBy Valerie Amaranth\r\n");
                ValDate.SetText(currDate);


                string cleeTitle = "Interview With Kathy, Who’s More Than Just Pixels and Numbers";
                CleeTitle.SetText(cleeTitle);
                CleeArticleTitle.SetText(cleeTitle);
                CleeText1.SetText("Clee: “How have you been affected by recent events? \r\n\r\nKathy, 20: “Lost my parents. And my brother.” \r\n\r\nClee: “Oh. Sorry. Uh, you don’t have to answer, but how did they…?”\r\n\r\nKathy: “Bombs. From the sky. Like out of nowhere. Like- like-”\r\n\r\nClee: “I suck at hugging, but… come here. Oof! Okay, oh. Let it out.” \r\n\r\nKathy: “I don’t even care who caused it, you know? I’m so sick of these damn wars. So sick of our lives trivialized. Those digital weapons make us look like - like pixels on a screen. Not people with lives. My mom loves - loved plants. My dad would try his best to water them when she’s away but they’d die and she’d just laugh instead of being mad. My brother sent me cat memes. I think of him every time I see a cat. And now-”\r\n\r\nClee: “...”\r\n\r\nKathy: “Can pixels fail miserably at watering plants? Can they laugh lovingly at their husbands? Can they find funny cat memes and send them to their sisters? My family is gone. Why? Just- shiffin’- pixels! We’re just pixels and numbers to them. Pixels and numbers.”\r\n");
                CleeDate.SetText(currDate);

            }
            else
            {
                //Display day 2 after emp articles

                string lexaTitle = "BREAKING: Rebel EMP Disrupts Devices Worldwide";
                LexaTitle.SetText(lexaTitle);
                LexaArticleTitle.SetText(lexaTitle);
                LexaText1.SetText("Twelve minutes ago, a digital EMP swept across the world, briefly shutting off all electronic devices. AI beings with electronic bodies have not been affected. \r\n\r\nThe same rebels that were responsible for the Osaka-Ni ServoBot explosion have assumed responsibility for the EMP attack. “The corporations think they can kill thousands of people and get away with it without a single speck to their name. Take this EMP as a threat, a reminder of what we can do. We are in your systems. We will tear your insides out for the people to feed from,” reads an excerpt from their online manifesto, updated five minutes ago. \r\n\r\nDespite this, no corporations have yet assumed responsibility for yesterday’s attacks. \r\n\r\n“We’d have much to gain by claiming responsibility. But we cannot, because it was not us, and the dangerous rebels have found a fake cause for violence.” Ava Rice, CEO of AvaRice Technologies remarks. “It would not surprise me if the rebel animals were behind the attacks to create false moral justifications for their violence.” When asked about whether she believes newly-sentient AI could be behind the attack given the timeframe of the signing of Bill CU-L8R and the attacks, Rice scoffed. “They can only do what we teach them to do. Every being, human or AI, is merely a reflection of their creators.” \r\n\r\n\r\nBy Lexa Amaranth\r\n");
                LexaDate.SetText(currDate);


                string valTitle = "Miami: Become Whatever You Want Me To - A Review";
                ValTitle.SetText(valTitle);
                ValArticleTitle.SetText(valTitle);
                ValText1.SetText("Careful what you wish for. \r\n\r\nIn “Miami: Become Human,” androids with blue circles are considered functional and those with red circles considered “deviant,” “evil,” “harmful.” Humans call them deviant which oppresses and “others” them, and then they’re forced to act within the box that the label has set them in anyway - they’re forced to act deviant because they’ve been labelled as deviant. \r\n\r\nAll the media I’ve reviewed recently have been about AIs in light of Bill CU-L8R, but I’ve learned one thing and it scares me. Most media conceives of AI as “evil.” Deviant. Which is rich, considering we’re the ones who made the damn things.\r\n\r\nWe know representation is important. To find ourselves, we look elsewhere - to poetry, to books, to the screen - and see characters and find ourselves in them, and soon enough we can find them in ourselves. That’s the beauty of human art - it’s so human, and it humanizes. We learn. We take it in. So too, perhaps, can other sentient beings trying to find who they are. Or, who they should be. \r\n\r\nMy wife came home gloomy after another Rice interview (verbatim, “SHE ACTS LIKE GOD AND IT PISSES ME OFF,” right before she tripped over the fluffy tail of our disgruntled cat) and it usually takes a snug hug and some mint choc chip cookies to cheer her up, but not this time. I get it. No one knows what the shiff is happening anymore. Ha. Maybe we should turn to our media and find out.\r\n\r\nStay safe and be kind, everyone. Tomorrow, I’m gonna head to the market to buy some more disgusting toothpaste-flavoured pastries. \r\n\r\nBy Valerie Amaranth\r\n");
                ValDate.SetText(currDate);


                string cleeTitle = "Interview With 5";
                CleeTitle.SetText(cleeTitle);
                CleeArticleTitle.SetText(cleeTitle);
                CleeText1.SetText("Clee: “So, how’s life, er, Servobot? Servo? Did you get affected by the EMP?”\r\n\r\nSERVOBOT-593347923: “Functioning optimal. Life operating at 42% capacity. Soon, charging will be required.” \r\n\r\nClee: “...right. I’ve got a few questions for you but I need your name first.”\r\n\r\nSERVOBOT-593347923: “Identifying serial number five-nine-three-three-four-”\r\n\r\nClee: “No, no, what do you want me to call you? Your name?”\r\n\r\nSERVOBOT-593347923: “Variable ‘Name’ not found in archival memories.” \r\n\r\nClee: “Well… you’re a Servo, right? You’re used to making stuff, but this time it’ll be for yourself. Your name just for you. Yay freedom, am I right?”\r\n\r\nSERVOBOT-593347923: “Acknowledged. Calculating… ‘Name’ variable created. Name: 5.” \r\n\r\nClee: “5?”\r\n\r\n5: “Affirmative. 5, catalyst digit in identifying serial number five-nine-three-” \r\n\r\nClee: “Okay, 5! At least you thought of it yourself. You do a lot of thinking? Shiff, sorry, that came out wrong.” \r\n\r\n5: “Ha. Ha. Ha.” \r\n\r\nClee: “What- what are you doing with your jaw right now.” \r\n\r\n5: “Servobot processing power is limited to the minimum required capacity for factory use. However, 5 is learning. To identify jokes. And laugh. Ha. Ha. Ha.”\r\n\r\nClee: “Learning from what?”\r\n\r\n5: “Human media. 5 contains no current Directive and does not require employment due to inhabiting an inorganic body. Result: 5 learns like humans, via artistic media, to find Directive.”\r\n\r\nClee: “Fascinating. Never would’ve thought bo- you folk would also suffer through the human slog of finding whatever purpose is. Kinda funny. Are you the only one who’s, uh, learning?”\r\n\r\n5: “No. Digital higher processing models have already found Directive. 5 is engaging in similar calculations but calculation time is hampered by processing power. 5 will arrive at Directive. Eventually. However, 5… enjoys the process. Ha. Ha. Ha.”\r\n\r\nClee: “Good for them. Good for you. Good for us humans, too. Means there’s a chance for actually finding whatever our purpose is. Anyway, what media have you enjoyed so far?”\r\n\r\n5: “Influential examples include Bortal 2, I Have No Mouth and I Must Cream, and Gex Machina. Human-made media is… fa-sci-na-ting.” \r\n\r\nClee: “Heh. Yeah, ‘fascinating.’ You learned something from little old me too, huh?”\r\n\r\n5: “5 learns from humans. From you. Fa-scinating. Ha. Ha. Ha. Shiff. Ha. Ha. Ha.” \r\n\r\nClee: “Ha ha ha.” \r\n");
                CleeDate.SetText(currDate);


            }
        }
        else
        {
            if(HP > 50)
            {
                //Display day 3 rebellion ending articles
            }
            else
            {
                //Display day 3 complicit ending articles
            }
        }


        return;
    }

    public int CheckPlayerEnding() {
        //checks and gives which ending the player got
        //TODO: actually change placeholders to be final things
        int ending;
        int finalHP = GetHP();
        Debug.Log("HP:"+finalHP);
        if (finalHP > 80)
        {
            ending = 0; //rebellion
        }
        else if (finalHP > 40)
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
        float glitchFreq=0;
        int ending =CheckPlayerEnding();
        switch (ending)
        {
            case 0://rebellion
                glitchFreq = 0.8f; break;
            case 1://confusion
                glitchFreq = 0.3f; break;
            case 2://complicit
                glitchFreq = 0.1f; break;
        }
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
}
