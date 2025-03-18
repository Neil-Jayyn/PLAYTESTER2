using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverScript : MonoBehaviour
{
    private GameObject box;
    private GameObject textObj;
    private TMPro.TextMeshProUGUI text;
    private Vector3 home = new Vector3(0, 10, -2);
    private float letterLength = 0.008831552f;
    private float yScale = 0.015f;

    private float xAdjust = -70;
    private float yAdjust = -230;

    public bool hoverable = false; //set to false by default since this script will appear on all buttons 
    //using app object prefab and I dont want them to be hoverable. We can just enable it on
    //everything that does qualify
    public string myHoverText = "";
    
    
    // Start is called before the first frame update
    void Start()
    {
        if(hoverable)
        {
            //dont waste time finding the objects if its not even hoverable
            box = GameObject.Find("Hover Box");
            textObj = GameObject.Find("Hover Text");
            text = textObj.GetComponent<TMPro.TextMeshProUGUI>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //make the popup appear and follow the cursor
    void OnMouseOver()
    {
        if (hoverable)
        {
            //set the text
            text.SetText(myHoverText);

            //adjust the size of the box to fit the text
            box.transform.localScale = new Vector3(letterLength * text.textInfo.characterCount, yScale, 0);

            //go to the mouse position
            Vector3 mousePos = Input.mousePosition;

            box.transform.position = new Vector3(mousePos.x + xAdjust, mousePos.y + yAdjust, -1);

            textObj.transform.position = new Vector3(mousePos.x + xAdjust, mousePos.y + yAdjust, -1);

        }
    }


    //put away the popup
    void OnMouseExit()
    {
        if(hoverable)
        {
            box.transform.position = home;
            text.transform.position = home;
        }
    }
}
