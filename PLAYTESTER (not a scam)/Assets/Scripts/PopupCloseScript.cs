using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupCloseScript : MonoBehaviour
{
    private GameObject popup;
    private GameObject bigPopup;
    public Vector3 popupHome; //Where the popup should go when it isn't being displayed
    public AudioClip clickSFX;
    private AudioSource audio;
    
    
    // Start is called before the first frame update
    void Start()
    {
        //Get the popup
        popup = GameObject.Find("Popup");
        bigPopup = GameObject.Find("Company Popup");
        popupHome = new Vector3(0, 10, 0);

        audio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Triggered when the popup is clicked and should close
    void OnMouseUp()
    {
        ClosePopup();
    }

    // Closes ALL popups
    public void ClosePopup()
    {
        popup.GetComponent<Transform>().position = popupHome;
        bigPopup.GetComponent<Transform>().position = popupHome;
        audio.PlayOneShot(clickSFX);

        return;
    }
}
