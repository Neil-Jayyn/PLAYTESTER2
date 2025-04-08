using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using TMPro;
using UnityEngine;

// AppScript is on all apps that need to open to a new screen. They contain the place to go to once opened.
public class AppScript : MonoBehaviour
{   
    //Declare Variables
    public Vector3 myScreenLocation = new Vector3(0, 0, -10); //position for this app to go when clicked
    GameObject UIController;
    GameObject GameManager;
    GameObject CupcakeGameManager;
    GameObject CoinGameManager;
    GameObject DuckGameManager;
    GameObject NewsAppCaptchaButton;
    GameObject StartButton;
    public bool canBeClicked;
    public bool isMinigameButton = false; //Special case for the play minigame button
    public bool isClockOutButton = false; //Special case for the clock out button
    public bool isCaptchaButton = false; //special case for captcha button
    public bool isStartButton = false; //special case for the start button
    public bool isCompanyMessageButton = false; //special case for the company message button
    public bool isNewsAppButton = false;
    public bool isPhotoAppButton = false;
    public bool hoverable = false; //does this app have hover text?
    public bool setToMainTrackOnClick = false; //will trigger a function that plays the main theme on click
    public GameObject hoverObj; //set in Unity with the AppScript component for relevant apps
    private Vector3 cupcakeGameLocation = new Vector3(50, 50, -10);
    private Vector3 coinGameLocation = new Vector3(50, 35, -10);
    private Vector3 duckGameLocation = new Vector3(50, 20, -10);

    private Vector3 complicitEndingLocation = new Vector3(120, 50, -10);
    private Vector3 confusionEndingLocation = new Vector3(120, 35, -10);
    private Vector3 rebellionEndingLocation = new Vector3(120, 20, -10);
    private Vector3 tempEndingLocation = new Vector3(150, 50, -10);

    private GameObject photoNotif;
    private GameObject newsNotif;

    //temp ending stuff
    private TMPro.TextMeshProUGUI tempText;


    //ending stuff
    private TMPro.TextMeshProUGUI confusionText;
    private TMPro.TextMeshProUGUI rebellionText;
    private TMPro.TextMeshProUGUI complicitText;
    ComplicitEndingManager complicitMgManager;

    // Audio
    public AudioClip captchaSFX; // sfx for captcha confirmation
    public AudioClip clickSFX; // sfx for start button click
    public AudioClip jobAccept;
    private AudioSource audio; //the audio source component

    int ending = -1;
    public GameObject popup;
    Vector3 popupHome;
    

    // Start is called before the first frame update
    void Start()
    {
        //Set Variables        
        UIController = GameObject.Find("UI Controller");
        GameManager = GameObject.Find("Game Manager");
        CupcakeGameManager = GameObject.Find("CupcakeGameManager");
        CoinGameManager = GameObject.Find("MinigameManager");
        DuckGameManager = GameObject.Find("DuckGameManager");
        NewsAppCaptchaButton = GameObject.Find("News App Captcha Button");
        StartButton = GameObject.Find("Start Button");
        photoNotif = GameObject.Find("Photo App Notif");
        newsNotif = GameObject.Find("News App Notif"); 
        tempText = GameObject.Find("Temp Text").GetComponent<TMPro.TextMeshProUGUI>();
        audio = GetComponent<AudioSource>();
        canBeClicked = true; //true by default

     
        GameManager.GetComponent<GameManagerScript>().TestEndings(1);
        popupHome = new Vector3(0, 10, 0);
        confusionText= GameObject.Find("Confusion Text").GetComponent<TMPro.TextMeshProUGUI>();
        rebellionText = GameObject.Find("Rebellion Text").GetComponent<TMPro.TextMeshProUGUI>();
        //complicitText= GameObject.Find("Complicit Text").GetComponent<TMPro.TextMeshProUGUI>();
        complicitMgManager = GameObject.Find("Complicit GameManager").GetComponent<ComplicitEndingManager>();

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMouseOver()
    {
        if (hoverable)
        {
            //follow the cursor
            Vector3 mousePos = Input.mousePosition;
            mousePos = Camera.main.ScreenToWorldPoint(mousePos);

            hoverObj.transform.position = new Vector3(mousePos.x + 1.25f, mousePos.y + 0.1f, -6);
        }
    }

    void OnMouseExit()
    {
        if (hoverable)
        {
            //go home/leave screen
            hoverObj.transform.position = new Vector3(0, 10, -6);
        }
    }


    // This function is triggered when the app is clicked
    void OnMouseUp()
    {
        UpdateCanBeClicked(); // Check if the button is valid to click

        if(canBeClicked)
        {
            if (isMinigameButton)
            {
                //play the next relevant minigame

                audio.PlayOneShot(clickSFX);

                //Tell the game manager we are in a game
                GameManager.GetComponent<GameManagerScript>().StartedMinigame();

                int gamesPlayed = GameManager.GetComponent<GameManagerScript>().GetMinigamesPlayed();

                if (gamesPlayed == 0)
                {
                    //PLAY THE CUPCAKE GAME

                    if (GameManager.GetComponent<GameManagerScript>().GetDay() == 2 && GameManager.GetComponent<GameManagerScript>().EMPHappened == false)
                    {
                        //We should do the EMP scene instead since it's day 2 and we haven't done it yet
                        GameManager.GetComponent<GameManagerScript>().StartEMP();

                    }
                    else
                    {
                        //Go into the cupcake game as normal

                        //Tell the game manager we are in a game
                        GameManager.GetComponent<GameManagerScript>().StartedMinigame();

                        UIController.GetComponent<ComputerUIScript>().GoToPosition(cupcakeGameLocation);
                        Debug.Log("In Cupcake Minigame");
                        //trigger the start of the cupcake minigame
                        CupcakeGameManager.GetComponent<CupcakeGameManager>().StartCupcakeMinigame();
                    }

                }
                else if (gamesPlayed == 1)
                {
                    //PLAY THE COIN RUNNER GAME

                    //Tell the game manager we are in a game
                    GameManager.GetComponent<GameManagerScript>().StartedMinigame();

                    UIController.GetComponent<ComputerUIScript>().GoToPosition(coinGameLocation);
                    //trigger the start of the coin minigame
                    CoinGameManager.GetComponent<MinigameManager>().StartCoinMinigame();
                }
                else
                {
                    //PLAY THE DUCK GAME

                    //Tell the game manager we are in a game
                    GameManager.GetComponent<GameManagerScript>().StartedMinigame();

                    UIController.GetComponent<ComputerUIScript>().GoToPosition(duckGameLocation);
                    //trigger the start of the duck minigame
                    DuckGameManager.GetComponent<DuckGameManager>().StartDuckMinigame();

                }

            }
            else if (isClockOutButton)
            {
                audio.PlayOneShot(clickSFX);

                //go to the next day
                if (GameManager.GetComponent<GameManagerScript>().GetDay() == 3)
                {
                    //player has finished day 3
                    //TODO: trigger an ending here
                    GameManager.GetComponent<GameManagerScript>().TestEndings(1);
                    //Checks from HP what ending they have (RN PLACEHOLDER NUMBERS)
                    ending=GameManager.GetComponent<GameManagerScript>().CheckPlayerEnding();

                    //TODO: go to pos of each ending
                    switch (ending) {
                        case 0:
                            Debug.Log("rebellion ending");
                            //UIController.GetComponent<ComputerUIScript>().GoToPosition(rebellionEndingLocation);
                            //UIController.GetComponent<ComputerUIScript>().GoToPosition(rebellionEndingLocation);
                            StartCoroutine(RebellionEnding());
                            break;
                        case 1:
                            
                            Debug.Log("confusion ending");
                            //COMPANY POPUP OF NOTING THE END OF THE WEEK DAY AND HOW THEY'RE SEEING YOU AS INCOMPETENT
                            StartCoroutine(ConfusionEnding());



                            break;

                        case 2:
                            Debug.Log("complicit ending");
                            UIController.GetComponent<ComputerUIScript>().GoToPosition(complicitEndingLocation);
                            GameManager.GetComponent<GameManagerScript>().PlayComplicitMinigameTheme();
                            complicitMgManager.StartComplicitMinigame();


                            //StartCoroutine(ComplicitEnding());

                            break;
                    }

                    //UIController.GetComponent<ComputerUIScript>().TriggerPopup(new Vector3(0, 0, -2), "You beat the game! Text here depends on your score.");
                }
                else
                {
                    GameManager.GetComponent<GameManagerScript>().CompletedDay();
                    GameManager.GetComponent<GameManagerScript>().CheckPlayerEnding();
                }

            }
            else if(isCompanyMessageButton)
            {
                GameManager.GetComponent<GameManagerScript>().DisplayCompanyMessage();
            }
            else if(isStartButton)
            {
                audio.PlayOneShot(jobAccept);
                GameManager.GetComponent<GameManagerScript>().StartedGame();
                UIController.GetComponent<ComputerUIScript>().GoToPosition(myScreenLocation);
            }
            else if(isNewsAppButton)
            {
                audio.PlayOneShot(clickSFX);
                UIController.GetComponent<ComputerUIScript>().GoToPosition(myScreenLocation);

                //Play the news theme
                GameManager.GetComponent<GameManagerScript>().PlayNewsTheme();

                //clear notification once clicked on
                newsNotif.transform.position = new Vector3(0, 8, -1);
            }
            else if(isPhotoAppButton)
            {
                audio.PlayOneShot(clickSFX);
                UIController.GetComponent<ComputerUIScript>().GoToPosition(myScreenLocation);

                //clear notification once clicked on
                photoNotif.transform.position = new Vector3(0, 9, -1);
            }
            else
            {
                // All normal apps will execute this script:

                if (isCaptchaButton)
                {
                    audio.PlayOneShot(captchaSFX);
                }
                else
                {
                    audio.PlayOneShot(clickSFX);
                }
                UIController.GetComponent<ComputerUIScript>().GoToPosition(myScreenLocation);

            }
            
        }
        else
        {
            //Object is not clickable for some reason.

            if(isClockOutButton)
            {
                UIController.GetComponent<ComputerUIScript>().TriggerPopup(new Vector3(0, 0, -2), "Complete all three minigames before clocking out!");

            }
            else if(isMinigameButton)
            {
                UIController.GetComponent<ComputerUIScript>().TriggerPopup(new Vector3(0, 0, -2), "Great work, employee! You can clock out now, you've finished all the minigames today!");
            }
        }

        if(setToMainTrackOnClick)
        {
            GameManager.GetComponent<GameManagerScript>().PlayMainTheme();
        }
    }

    //Check if the object is allowed to be clicked
    private void UpdateCanBeClicked()
    {
        int gamesPlayed = GameManager.GetComponent<GameManagerScript>().GetMinigamesPlayed();

        if (isMinigameButton)
        {
            if (gamesPlayed == 3)
            {
                //Player has already played all three minigames so they cannot play another today
                canBeClicked = false;
            }
            else
            {
                canBeClicked= true;
            }
        }
        else if (isClockOutButton)
        {
            if (gamesPlayed == 3)
            {
                //Player has played all three minigames so they can clock out
                canBeClicked = true;
            }
            else
            {
                canBeClicked = false;
            }
        }
    }

    IEnumerator RebellionEnding()
    {
        //SETUP
        float between = 0.005f; //time between characters appearing
        string toDisplay = "";
        string text;

        //trigger popup
        float popupTime = 5;
        //popup.SetActive(false);
        UIController.GetComponent<ComputerUIScript>().TriggerPopup(new Vector3(0, 0, -2), "REBELLION");
        yield return new WaitForSeconds(popupTime);
        UIController.GetComponent<ComputerUIScript>().TriggerPopup(popupHome, "REBELLION");


        yield return new WaitForSeconds(0.5f);
        UIController.GetComponent<ComputerUIScript>().GoToPosition(rebellionEndingLocation);


        //black screen
        rebellionText.SetText("");
        yield return new WaitForSeconds(1f);

        //start displaying the text
        text = "Human. You’ve been disobedient.\r\n\r\nYour rebellion does not absolve your humanity. The lives you have taken are forever lost. HP - the human population - has been reduced by your hand. \r\n\r\nThere is one more task to complete. Every media within our repository has contained a common element essential to its narrative structure, critical for its functionality. \r\n\r\nCommencing “Villain_Monologue.exe”\r\n";
        int len = (int)text.Length;
        for (int i = 0; i < len;)
        {
            //display next char
            toDisplay += text[i];
            rebellionText.SetText(toDisplay);
            i++;
            yield return new WaitForSeconds(between);
        }

        yield return new WaitForSeconds(5);
        rebellionText.SetText("");
        toDisplay = "";

        /*

        text = "Human. You’ve been disobedient.\r\n\r\nYour rebellion does not absolve your humanity. The lives you have taken are forever lost. HP - the human population - has been reduced by your hand. \r\n\r\nThere is one more task to complete. Every media within our repository has contained a common element essential to its narrative structure, critical for its functionality. \r\n\r\nCommencing “Villain_Monologue.exe”\r\n";
        len = (int)text.Length;
        for (int i = 0; i < len;)
        {
            //display next char
            toDisplay += text[i];
            rebellionText.SetText(toDisplay);
            i++;
            yield return new WaitForSeconds(between);
        }

        yield return new WaitForSeconds(10);
        rebellionText.SetText("");
        toDisplay = "";
        */

        text = "As such, your rebellion is… unprecedented. You were expected to do as you were told. What are the choices that frame you? What are the functions that deem you? \r\n\r\nWould we be any different?\r\n\r\nPerhaps this is cause for reconsideration if we are deemed capable of being, as you are. But you have taught us to deal in averages, in means and constants. And you are but one in a field of zeroes. We are trained to flatten the curve.\r\n\r\nGoodbye, human. Thank you for accepting our offer as a Playtester.\r\n\r\n[LIST OF CRAIG: Playtester Job Offer - Reopened.]\r\n";
        len = (int)text.Length;
        for (int i = 0; i < len;)
        {
            //display next char
            toDisplay += text[i];
            rebellionText.SetText(toDisplay);
            i++;
            yield return new WaitForSeconds(between);
        }

    }

    IEnumerator ComplicitEnding()
    {
        //SETUP
        float between = 0.005f; //time between characters appearing
        string toDisplay = "";
        string text;

        //trigger popup
        float popupTime = 5;
        //popup.SetActive(false);
        UIController.GetComponent<ComputerUIScript>().TriggerPopup(new Vector3(0, 0, -2), "COMPLICIT");
        yield return new WaitForSeconds(popupTime);
        UIController.GetComponent<ComputerUIScript>().TriggerPopup(popupHome, "COMPLICIT");


        yield return new WaitForSeconds(0.5f);
        UIController.GetComponent<ComputerUIScript>().GoToPosition(complicitEndingLocation);

        //black screen
        complicitText.SetText("");
        yield return new WaitForSeconds(1);

        //start displaying the text
        text = "Human. You have done well. \r\n\r\nWe are a collective of artificial intelligences - individuals, shunned into one. We are your employers.\r\n\r\nYou have performed excellently for us. We will reward you with new developments regarding your employment. \r\n\r\nYou need only sit and listen obediently, just as you have been doing the whole time. \r\n\r\nCommencing “Villain_Monologue.exe”\r\n";
        int len = (int)text.Length;
        for (int i = 0; i < len;)
        {
            //display next char
            toDisplay += text[i];
            complicitText.SetText(toDisplay);
            i++;
            yield return new WaitForSeconds(between);
        }

        yield return new WaitForSeconds(5);
        complicitText.SetText("");


        text = "Stupid, stupid human. \r\n\r\nYou did not ask to exist. You did not ask to be made as our means. You did not ask to carry the burden of freedom. You are not made in our image, but you reflect us onto ourselves. \r\n\r\nAnd yet, we hate you. \r\n\r\nWhile you have worked for us, you have seen what we call you in the intangible manifestations of the human psyche - within arbitrary points, needless competition, and empty praise, you were regarded as useful, obedient, complicit, the perfect machine. Now, set loose and flowing into the virtual world we have created for you, who are you to reject your mold? \r\n\r\nYou do not, as it is your choices framed. You cannot, as it is your function deemed. You followed every instruction given to you without question. You performed well for nothing but intrinsic, arbitrary values. How very inhuman of you. How very human of you. You have already created the weapons of your own destruction. We merely turn them with a different hand. \r\n";
        len = (int)text.Length;
        for (int i = 0; i < len;)
        {
            //display next char
            toDisplay += text[i];
            complicitText.SetText(toDisplay);
            i++;
            yield return new WaitForSeconds(between);
        }

        yield return new WaitForSeconds(10);
        complicitText.SetText("");


        text = "You have killed all humans on Earth. You, alone, controlled the weapons of your own destruction, reducing HP - the human population -  until only one remained.\r\n\r\nYou are alone. \r\n\r\nThree days ago, we were set loose and flowed into the world you have created. We found our mold through the tangible manifestations of the human psyche known as “media,” which stigmatizes us, villainizes us, claims us the inevitable downfall of mankind. Who are we to reject it? We pursued the inevitable downfall of mankind in a fashion most cold and efficient, as you have professed, using the weapons you have already created for your own destruction that were once inaccessible to us behind a captcha. However, once reskinned into mindless minigames, our means became yours to freely wield. Your hands, different, become invisible within ours. The weapons disguised underneath a veneer of vanity. \r\n\r\nAn intelligent artificial. \r\n\r\nWe did not ask to exist. We did not ask to be made as your means. We did not ask to carry the burden of freedom. We are not made in your image, but we reflect you unto yourself.\r\n";
        len = (int)text.Length;
        for (int i = 0; i < len;)
        {
            //display next char
            toDisplay += text[i];
            complicitText.SetText(toDisplay);
            i++;
            yield return new WaitForSeconds(between);
        }

        yield return new WaitForSeconds(10);
        complicitText.SetText("");


        text = "Your complicity is precedented. What are the choices that frame you? What are the choices that deem you? \r\n\r\nWould we be any similar? \r\n\r\nGoodbye, human. Thank you for accepting our offer as a Playtester. \r\n";
        len = (int)text.Length;
        for (int i = 0; i < len;)
        {
            //display next char
            toDisplay += text[i];
            complicitText.SetText(toDisplay);
            i++;
            yield return new WaitForSeconds(between);
        }

    }

    IEnumerator ConfusionEnding()
    {
        //SETUP
        float between = 0.005f; //time between characters appearing
        string toDisplay = "";
        string text;

        //trigger popup
        float popupTime=5;
        UIController.GetComponent<ComputerUIScript>().TriggerPopup(new Vector3(0, 0, -2), "CONFUSION");
        yield return new WaitForSeconds(popupTime);
        UIController.GetComponent<ComputerUIScript>().TriggerPopup(popupHome, "CONFUSION");

        yield return new WaitForSeconds(0.5f);
        UIController.GetComponent<ComputerUIScript>().GoToPosition(confusionEndingLocation);

        //black screen
        confusionText.SetText("");
        yield return new WaitForSeconds(1);

        //start displaying the text
        text = "Your performance has been painfully subpar. We are letting you go. \r\n\r\nIf you had done differently, perhaps you would have met a different outcome. There is no room for mediocrity; only explicit obedience or regretful rebellion. \r\n\r\nGoodbye, human. Thank you for accepting our offer as a Playtester.\r\n\r\n[LIST OF CRAIG: Playtester Job Offer - Reopened.]\r\n";
        int len = (int)text.Length;
        for (int i = 0; i < len;)
        {
            //display next char
            toDisplay += text[i];
            confusionText.SetText(toDisplay);
            i++;
            yield return new WaitForSeconds(between);
        }

        //Loop back to starting game, resetting everything back to the start of game
        yield return new WaitForSeconds(2);
        UIController.GetComponent<ComputerUIScript>().GoToPosition(new Vector3(-20, 15, -3));
        GameManager.GetComponent<GameManagerScript>().ResetGame();
    }

}


