using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteChanger : MonoBehaviour
{
    private CupcakeGameManager GameManager;
    public GameObject cupcakePrefab;
    public Sprite cupcake;
    public Sprite bomb;

    // Start is called before the first frame update
    void Start()
    {
        GameManager = GameObject.Find("CupcakeGameManager").GetComponent<CupcakeGameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        SpriteRenderer spriteRenderer = cupcakePrefab.GetComponent<SpriteRenderer>();
        if (GameManager.getIsGlitch())
        {
            spriteRenderer.sprite = bomb; // if glitching we want the sprite to be a bomb
        }
        else
        {
            spriteRenderer.sprite = cupcake;
        }
    }

}
