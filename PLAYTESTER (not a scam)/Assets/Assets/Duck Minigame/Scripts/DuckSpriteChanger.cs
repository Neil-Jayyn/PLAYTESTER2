using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuckSpriteChanger : MonoBehaviour
{
    private DuckGameManager duckGameManager;
    public GameObject duckPrefab;
    public Sprite duck;
    public Sprite human;

    // Start is called before the first frame update
    void Start()
    {
        duckGameManager = GameObject.Find("DuckGameManager").GetComponent<DuckGameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        SpriteRenderer spriteRenderer = duckPrefab.GetComponent<SpriteRenderer>();
        if (duckGameManager.isGlitch)
        {
            spriteRenderer.sprite = human; // if glitching we want the sprite to be a bomb
        }
        else
        {
            spriteRenderer.sprite = duck;
        }
    }
}