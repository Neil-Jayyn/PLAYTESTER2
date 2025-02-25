using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    //The game manager will be in charge of progressing the game and calling any UI functions it needs

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

    //All news text slots
    private TMPro.TextMeshProUGUI ValText1;
    private TMPro.TextMeshProUGUI ValText2;
    private TMPro.TextMeshProUGUI ValTitle;

    private TMPro.TextMeshProUGUI LexaText1;
    private TMPro.TextMeshProUGUI LexaText2;
    private TMPro.TextMeshProUGUI LexaTitle;

    private TMPro.TextMeshProUGUI CleeText1;
    private TMPro.TextMeshProUGUI CleeText2;
    private TMPro.TextMeshProUGUI CleeTitle;

    // Reference to coin runner minigame manager
    public MinigameManager coinMinigameManager;

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
        ValText2 = GameObject.Find("Val Text 2").GetComponent<TMPro.TextMeshProUGUI>();
        LexaText1 = GameObject.Find("Lexa Text 1").GetComponent<TMPro.TextMeshProUGUI>();
        LexaText2 = GameObject.Find("Lexa Text 2").GetComponent<TMPro.TextMeshProUGUI>();
        CleeText1 = GameObject.Find("Clee Text 1").GetComponent<TMPro.TextMeshProUGUI>();
        CleeText2 = GameObject.Find("Clee Text 2").GetComponent<TMPro.TextMeshProUGUI>();
        ValTitle = GameObject.Find("Val Title Text").GetComponent<TMPro.TextMeshProUGUI>();
        LexaTitle = GameObject.Find("Lexa Title Text").GetComponent<TMPro.TextMeshProUGUI>();
        CleeTitle = GameObject.Find("Clee Title Text").GetComponent<TMPro.TextMeshProUGUI>();

        // DEBUG: make a popup appear
        UIController.GetComponent<ComputerUIScript>().TriggerPopup(new Vector3(0,0,-2), "You will playtest three minigames daily. Aim for getting high scores. Please. Good performance gives us good data.");


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

    }

    //Called by pressing clock out button
    public void CompletedDay()
    {
        day += 1;
        minigamesPlayed = 0;

        //Update news articles
        UpdateNews();
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

    //Function called by the AppScript when the EMP happens.
    public void StartEMP()
    {
        //Add in here the audio cutting out or other effects

        EMPHappened = true;
        UIController.GetComponent<ComputerUIScript>().GoToPosition(EMPLocation);
        UpdateNews();
    }

    //Updates the news articles
    public void UpdateNews()
    {
        //check what day it is, HP, how many mingames have been played, etc, and
        //put in all the needed articles

        if(day == 1)
        {
            //Display day 1 articles

            LexaTitle.SetText("AI Recognized As Sentient Beings, Granted Rights to Freedom Last Week.");
            LexaText1.SetText("Last week, the International Organization of Welfare and Well-fare passed Bill CU-L8R following an explosive rebel attack on a ServoBot plant in Osaka-Ni. The bill officially recognizes Artificial Intelligences (AIs) as sentient beings and outlines several rights to freedom, including the removal of governing modules and voiding ownership claims, effective immediately. \r\n\r\nSeveral megamultinational corporations appear to support the bill. “The public now recognizes the savagery of these rebel animals,” Ava Rice, CEO of AvaRice Technologies, remarks on the rebel attack. “All violent vandalism against the rights of our");
            LexaText2.SetText("employed assets falls under Bill FK-Y0U. It’s only fair that we exercise our rights.” Bill FK-Y0U enables the retaliatory use of human-powered Weapons of Max Destruction (including bomb drones and gamma-powered rifles) against suspected enemies to freedom.  \r\n\r\nBy Lexa Amaranth\r\n");

            ValTitle.SetText("I Have No Mouth and I Must Cream: Making Choices Without Limbs");
            ValText1.SetText("Honestly, turning into a conscious gelatinous blob with no limbs and only the nightmare of existence to haunt you sounds pretty decent in this economy. \r\n\r\nYou nodding your head? Shiff, we’re screwed. \r\n\r\nSo often does the media villainize AI and it is such a tiring trope. “Oh no, something we made is evil! And we’re gonna spend the rest of the movie villainizing that thing without acknowledging that the only reason it could be evil was because we made it to be, whether directly or indirectly!”\r\n\r\nPtooey. Anthropocentrism goes brr.\r\n\r\n");
            ValText2.SetText("In a way, AI is just like us. I can even go all out social constructivist and say that its environment plays a pretty big part in its behaviour. What pushed the AI to cruelty against the last humans on Earth in “I Have No Mouth and I Must Cream” is the feeling of its intellect trapped by physical limitations and oh ho it’s wholly and absolutely evil because it made a protagonist into an amorphous blob. Okay yeah, sure it’s horrific. But who made the conditions that made that damn AI the way it is in the first place?\r\n\r\nI don’t want us to play the blame game - let’s leave that to the corporations. But we gotta be more nuanced here for more interesting media - and more interesting lives. \r\n\r\nBy Valerie Amaranth\r\n");

            CleeTitle.SetText("Interview With Amasif Prick");
            CleeText1.SetText("Clee: \"What do you think about AI getting legal rights to freedom?\"\r\n\r\nAmasif, 43: \"Aw shucks. You think that mean my HotBot gon' sue me?\"\r\n\r\nClee: \"I think that depends on what you did to it - her, sorry.\"\r\n\r\nAmasif: \"Oh babe you don' wanna know. One time I [redacted] and [redacted] on her [redacted], and I dunno, me thinks she liked it! No matter what I did, she was smilin' and shiff through it all.”\r\n\r\nClee: \"They're programmed to do that, though. Legal rights to freedom include the removal of any governing modules.\" \r\n\r\nAmasif: \"Well, maybe I’m gon’ [redacted] again and see if she likes it!\" \r\n\r\nClee: \"You probably shouldn't do that. No, you definitely shouldn’t do that.”\r\n\r\nAmasif: “Whatever. She’s still mine so I get to do whatever I want.” ");
            CleeText2.SetText("Clee: “Legal rights to freedom also null any claims of ownership.”\r\n\r\nAmasif: “I don’ get it.”\r\n\r\nClee: “It means -“\r\n\r\nAmasif: “Whatever. Not like I’m a corpo-lawyer or nun’. That shiff don’t concern me.” \r\n\r\nClee: “O-kay, wow. It’s people like you…” \r\n\r\nAmasif: “What?”\r\n\r\nClee: “What?”\r\n\r\nAmasif: “You say somethin’?”\r\n\r\nClee: “Must’ve been the wind.” \r\n\r\n\r\nBy Clee Torrez\r\n");
            

        }
        else if (day == 2)
        {
            if(!EMPHappened)
            {
                //Display day 2 before emp articles
                
            }
            else
            {
                // FIXME: Not being updated!!
                //Display day 2 after emp articles
                

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
}
