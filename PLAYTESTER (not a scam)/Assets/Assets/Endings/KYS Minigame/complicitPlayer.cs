﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class complicitPlayer : MonoBehaviour
{
    public SpriteRenderer crosshairSprite;
    public LayerMask interactableLayer;

    bool hasShot;

    ComplicitEndingManager complicitManager;
    // Start is called before the first frame update
    void Start()
    {
        complicitManager = GameObject.Find("Complicit GameManager").GetComponent<ComplicitEndingManager>();
        hasShot = false;
      
    }

    // Update is called once per frame
    void Update()
    {
        if (!complicitManager.gameOver){
            //TODO: crosshair over player, hit player as final HP bar is set
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            crosshairSprite.transform.position = mousePos;

            
            if (Input.GetMouseButtonDown(0)) // Left click
            {
                if (hasShot) {
                    Debug.Log("Successful shot");
                    complicitManager.PlayerShotThemself();
                    hasShot = false;
                }

                hasShot = false;

                complicitManager.shotAudio();
                //sfx.clip = shootSFX;
                //sfx.Play();
                //duckGameManager.audioSources[2].Play();
                
               
            }
            

        }


    }

    
    void DetectObjectUnderCrosshair(Vector2 mousePos)
    {
        Collider2D hit = Physics2D.OverlapPoint(mousePos, interactableLayer);
        if (hit != null)
        {
            // If the crosshair is over something, perform actions (like hitting a duck or trash)
            if (hit.CompareTag("Duck"))
            {
                //duckGameManager.AddPoints(1);
                //duckGameManager.audioSources[0].Play();
                Destroy(hit.gameObject); 

            }
        }
    }
 
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // collider check if it hits the ground or people
        if (collision.gameObject.tag == "Duck")
        {
            Debug.Log("Kill YS");
            hasShot = true;
            
        }
    }
    
}
