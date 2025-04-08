using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scrollBg : MonoBehaviour
{
    // public float speed = 2f;
    public float xThreshold;
    public float xResetTo;

    public bool startCoinMinigame;

    // to change to glitched bg
    public SpriteRenderer backgroundRenderer;
    public Sprite glitchedCoinBackground;
    public Sprite normalCoinBackground;

    MinigameManager CoinGameManager;

    void Start() { 
        startCoinMinigame = false;
        CoinGameManager = GameObject.Find("MinigameManager").GetComponent<MinigameManager>();
    }
    // Update is called once per frame
    void Update()
    {
        if (startCoinMinigame) { 

            if (CoinGameManager.isGlitch)
            {
                backgroundRenderer.sprite = glitchedCoinBackground;
            } else
            {
                backgroundRenderer.sprite = normalCoinBackground;
            }
        transform.position += Vector3.right * CoinGameManager.scrollSpeed * Time.deltaTime; // Move background to the right

        if (transform.position.x > xThreshold) // Check out of bounds
        {
            transform.position = new Vector3(xResetTo, transform.position.y, transform.position.z); // Reset background
        } 
        }
    }

}
