using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices.ComTypes;
using TMPro;
using UnityEngine;

public class player : MonoBehaviour
{
    public Transform[] lanes;
    private int curLane = 1; // middle lane
    //public TMP_Text pointsText;
    //private int points = 0;

    // UnityEngine.Color colorOne = new UnityEngine.Color(1f, 1f, 0f, 1f); // yellow
    // UnityEngine.Color colorTwo = new UnityEngine.Color(0.5f, 0f, 0.5f, 1f); // purple

    MinigameManager CoinGameManager;


    void Start()
    {
        // sfx = GetComponent<AudioSource>();
        CoinGameManager = GameObject.Find("MinigameManager").GetComponent<MinigameManager>();
    }

    void Update()
    {
        // Update player sprite and animation

        // Detect up and down arrow keyboard input; also compatible with wasd type controls
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) 
        {
            MoveUp();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            MoveDown();
        }
    }

    void MoveUp()
    {
        if (curLane > 0) // Prevent out of bounds
        {
            curLane--;
            // transform.position = lanes[curLane].position;
            transform.position = new Vector3(lanes[curLane].position.x, lanes[curLane].position.y, -2);
        }
    }

    void MoveDown()
    {
        if (curLane < lanes.Length - 1) // Prevent out of bounds
        {
            curLane++;
            // transform.position = lanes[curLane].position;
            transform.position = new Vector3(lanes[curLane].position.x, lanes[curLane].position.y, -2);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (CoinGameManager.IsGameOver() == false)
        {
            if (other.CompareTag("Coin"))
            {
                CoinGameManager.AddPoints(1);
                Debug.Log("+1 point");
            } else if (other.CompareTag("EvilCoin"))
            {
                CoinGameManager.AddPoints(-1);
                Debug.Log("-1 point");

                // Decrease speed
                CoinGameManager.scrollSpeed -= CoinGameManager.slowDownAmount;
                CoinGameManager.scrollSpeed = Mathf.Clamp(CoinGameManager.scrollSpeed, CoinGameManager.minSpeed, CoinGameManager.maxSpeed); // make sure speed is between set bounds
            }
                
            // Play the assigned sound effect from MinigameManager
            if (CoinGameManager != null && CoinGameManager.sfx != null)
            {
                CoinGameManager.sfx.Play(); // Play sound effect
            }

            Destroy(other.gameObject);
            
        }

    }
    //private void OnTriggerEnter2D(Collider2D other)
    //{
    //    if(CoinGameManager.IsGameOver() == false)
    //    {
    //        if (other.CompareTag("spawnable"))
    //        {
    //            SpriteRenderer spriteRenderer = other.GetComponent<SpriteRenderer>();

    //            if (spriteRenderer.color == colorOne) // The spawnable touched adds points
    //            {
    //                CoinGameManager.AddPoints(1);
    //            }
    //            else // Otherwise it is a spawnable that takes away points
    //            {
    //                CoinGameManager.AddPoints(-1);
    //            }
    //            pointsText.text = "Points: " + points;

    //            // Play the assigned sound effect from MinigameManager
    //            if (CoinGameManager != null && CoinGameManager.sfx != null)
    //            {
    //                CoinGameManager.sfx.Play(); // Play sound effect
    //            }

    //            Destroy(other.gameObject);
    //        }
    //    }

    //}
}
