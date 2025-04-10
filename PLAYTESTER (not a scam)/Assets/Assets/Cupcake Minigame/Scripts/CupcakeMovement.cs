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
            GameManager.SfxCupcakeHit(GameManager.getIsGlitch()); //plays yay sfx through game manager
            GameManager.AddPoints(1);
            
            //decrease total enemies on screen
            GameObject.Find("EnemySpawner").GetComponent<EnemySpawner>().KilledEnemy();
            Destroy(collision.gameObject);
            Destroy(gameObject); 
        }
        if (collision.gameObject.tag == "SpeedyPerson")
        {
            GameManager.SfxCupcakeHit(GameManager.getIsGlitch()); //plays yay sfx through game manager
            GameManager.AddPoints(3);

            //decrease total enemies on screen
            GameObject.Find("EnemySpawner").GetComponent<EnemySpawner>().KilledEnemy();
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
    }
}
