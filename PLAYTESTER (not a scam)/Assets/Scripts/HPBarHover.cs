using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPBarHover : MonoBehaviour
{
    public GameObject hoverObj;
    public TMPro.TextMeshProUGUI hoverText;
    public GameManagerScript gameManager;

    // Start is called before the first frame update
    void Start()
    {
        //Move the popup away from the screen
        hoverObj.transform.position = new Vector3(0, 8, -6);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMouseEnter()
    {
        //set the text to be correct depending on day
        if(gameManager.day == 3)
        {
            hoverText.SetText("HP: Human Population");
        }
        else if(gameManager.day == 2 && gameManager.EMPHappened) //post emp
        {
            hoverText.SetText("HP: H*!an P#p@?a$^%n");
        }
        else
        {
            hoverText.SetText("HP: H*!%< P#/@?*$^%&");
        }

    }

    void OnMouseOver()
    {
        //follow the cursor
        Vector3 mousePos = Input.mousePosition;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);

        hoverObj.transform.position = new Vector3(mousePos.x + 1, mousePos.y, -6);
    }

    void OnMouseExit()
    {
        //go home/leave screen
        hoverObj.transform.position = new Vector3(0, 8, -6);
    }
}
