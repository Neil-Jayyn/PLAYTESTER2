using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CupcakeMovement : MonoBehaviour
{
    
    [SerializeField] public float speed = 6f;
    // Audio

    private CupcakeGameManager GameManager;


    // Start is called before the first frame update
    void Start()
    {
    
    }

    void Awake() 
    {
        GameManager = GameObject.Find("CupcakeGameManager").GetComponent<CupcakeGameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector2.down * speed * Time.deltaTime); //cupcake goes down
    }

    private void OnTriggerEnter2D(Collider2D collision){  
        // collider check if it hits the ground or people
        if(collision.gameObject.tag == "Ground") 
        {  
            Destroy(gameObject);
        }
        if (collision.gameObject.tag == "Person")
        {
            GameManager.SfxCupcakeHit(GameManager.isGlitch); //plays yay sfx through game manager
            /* Do sfx through the minigame manager instead
            if (sfxYay != null)
            {
                if (!GameManager.isGlitch)
                {
                    sfx.clip = sfxYay;
                    sfx.Play();
                } else // means the glitch is occuring
                {
                    sfx.clip = sfxGlitchedYay;
                    sfx.Play();
                }
            }*/
            GameManager.AddPoints(1);
            
            //decrease total enemies on screen
            GameObject.Find("EnemySpawner").GetComponent<EnemySpawner>().KilledEnemy();
            Destroy(collision.gameObject);
            Destroy(gameObject);

            
        }  
    }

    public void DestroyCupcake(bool hasHitPerson) { 
        if (hasHitPerson)
        {
            Destroy(gameObject);
        }
    }
}
