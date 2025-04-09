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

    void ChooseSFX(AudioSource audioaudioClipsSource, AudioClip[] audioClips)
    {
        int index = Random.Range(0, audioClips.Length);
        audioaudioClipsSource.clip = audioClips[index];
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (CoinGameManager.IsGameOver() == false)
        {
            if (other.CompareTag("Coin"))
            {
                CoinGameManager.AddPoints(1);

                // Sound effects               
                if (CoinGameManager != null && CoinGameManager.goodCoinSFX != null)
                {
                    if (!CoinGameManager.isGlitch)
                    {
                        ChooseSFX(CoinGameManager.goodCoinSFX, CoinGameManager.goodCoinObtainedSFX);
                    }
                    else
                    {
                        ChooseSFX(CoinGameManager.goodCoinSFX, CoinGameManager.screamsSFX);
                    }
                    CoinGameManager.goodCoinSFX.Play(); 
                }
                if (CoinGameManager != null && CoinGameManager.pirateYarrSFX != null)
                {
                    if (!CoinGameManager.isGlitch)
                    {
                        ChooseSFX(CoinGameManager.pirateYarrSFX, CoinGameManager.pirateYarrClip);
                    } else
                    {
                        ChooseSFX(CoinGameManager.pirateYarrSFX, CoinGameManager.robotSuccessClip);
                    }
                    CoinGameManager.pirateYarrSFX.Play(); 
                }
                if (CoinGameManager != null && CoinGameManager.ChainsawKillSFX != null)
                {
                    CoinGameManager.ChainsawKillSFX.Play();
                }

            } else if (other.CompareTag("EvilCoin"))
            {
                CoinGameManager.AddPoints(-1);

                // Sound effects
                if (CoinGameManager != null && CoinGameManager.badCoinSFX != null)
                {
                    CoinGameManager.badCoinSFX.Play(); 
                }
                if (CoinGameManager != null && CoinGameManager.pirateNarrSFX != null)
                {
                    if (!CoinGameManager.isGlitch)
                    {
                        ChooseSFX(CoinGameManager.pirateNarrSFX, CoinGameManager.pirateNarrClip);
                    } else
                    {
                        ChooseSFX(CoinGameManager.pirateNarrSFX, CoinGameManager.robotFailureClip);
                    }
                    CoinGameManager.pirateNarrSFX.Play(); 
                }
                //if (CoinGameManager != null && CoinGameManager.ChainsawMissSFX != null)
                //{
                //    CoinGameManager.ChainsawMissSFX.Play();
                //}
                // Decrease speed
                CoinGameManager.scrollSpeed = CoinGameManager.SlowDownSpeed;
                CoinGameManager.scrollSpeed = Mathf.Clamp(CoinGameManager.scrollSpeed, CoinGameManager.minSpeed, CoinGameManager.maxSpeed); // make sure speed is between set bounds
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
