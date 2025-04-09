using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnable : MonoBehaviour
{
    
    // public float speed = 2f;
    public float xThreshold;

    // Animations/Sprites
    private Animator animator;

    MinigameManager CoinGameManager;
    void Start()
    {
        CoinGameManager = GameObject.Find("MinigameManager").GetComponent<MinigameManager>();
        animator = GetComponent<Animator>();
        //UpdateAnimationState();
    }

    // Update is called once per frame
    void Update()
    {

        if (CompareTag("Coin"))
        { 
            bool glitchState = CoinGameManager.isGlitch;
            animator.SetBool("isGlitch", glitchState);
        }

        if (CompareTag("EvilCoin"))
        {
            bool glitchState = CoinGameManager.isGlitch;
            animator.SetBool("isGlitch", glitchState);
        }

        transform.position += Vector3.right * CoinGameManager.scrollSpeed * Time.deltaTime; // move spawnable to the right

        if (transform.position.x > xThreshold) // If the spawnable goes out of screen, remove it
        {
            Destroy(gameObject);
        }
   
        
    }
}
