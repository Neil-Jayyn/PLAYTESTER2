using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputerUIScript : MonoBehaviour
{
    //DECLARE VARIABLES
    public static GameObject camera;
    private static GameObject popup;

    // Start is called before the first frame update
    void Start()
    {
        // GET ALL OBJECTS
        camera = GameObject.Find("Main Camera");
        popup = GameObject.Find("Popup");

        //SET VARIABLES
        //initialize camera to the first position in the game (title screen)
        camera.GetComponent<Transform>().position = new Vector3(-20,15,-10);
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
        popup.GetComponent<Transform>().position = pos;
        popup.transform.GetChild(1).transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().SetText(text);
        
    }
}
