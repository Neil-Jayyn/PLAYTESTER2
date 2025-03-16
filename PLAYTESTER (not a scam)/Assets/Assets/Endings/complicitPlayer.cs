using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class complicitPlayer : MonoBehaviour
{
    public SpriteRenderer crosshairSprite;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //TODO: crosshair over player, hit player as final HP bar is set

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        crosshairSprite.transform.position = mousePos;
    }
}
