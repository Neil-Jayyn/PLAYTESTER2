using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputerUIScript : MonoBehaviour
{
    //DECLARE VARIABLES
    public static GameObject camera;
    private static GameObject popup;
    private static GameObject bigPopup;

    private static Vector3 titleScreen = new Vector3(-20, 15, -10);

    // Start is called before the first frame update
    void Start()
    {
        // GET ALL OBJECTS
        camera = GameObject.Find("Main Camera");
        popup = GameObject.Find("Popup");
        bigPopup = GameObject.Find("Company Popup");

        GoToPosition(titleScreen);
    }


    // Update is called once per frame
    void Update()
    {
        
    }


    // This method can be called to change the position of the camera. By default the camera should be set to z=-10
    public void GoToPosition(Vector3 pos)
    {
        camera.GetComponent<Transform>().position = pos;
        return;
    }

    //This function takes a Vector3 position (for the popup to spawn) and text for the popup
    public void TriggerPopup(Vector3 pos, string text)
    {
        popup = GameObject.Find("Popup");

        //trigger close popup first so that if another popup is showing it will close it, then display the new one
        GameObject.Find("Close Popup").GetComponent<PopupCloseScript>().ClosePopup();


        popup.GetComponent<Transform>().position = pos;
        popup.transform.GetChild(1).transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().SetText(text);
        
    }

    public void TriggerCompanyPopup(Vector3 pos, string text)
    {
        bigPopup = GameObject.Find("Company Popup");

        //trigger close popup first so that if another popup is showing it will close it, then display the new one
        GameObject.Find("Close Popup").GetComponent<PopupCloseScript>().ClosePopup();

        bigPopup.GetComponent<Transform>().position = pos;
        bigPopup.transform.GetChild(1).transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().SetText(text);

    }

    public void TriggerEndingPopup(Vector3 pos, string text)
    {
        bigPopup = GameObject.Find("Ending Popup");

        bigPopup.GetComponent<Transform>().position = pos;
        bigPopup.transform.GetChild(1).transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().SetText(text);

    }

    public void ResetGame() {
        //SET VARIABLES
        //initialize camera to empty position in the game to play the video
        camera.GetComponent<Transform>().position = new Vector3(-20, 50, -10);
        //StartCoroutine(WaitForVideo());
    }
}
