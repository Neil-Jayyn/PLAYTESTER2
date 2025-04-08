using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnMg : MonoBehaviour
{
    public GameObject spawnable;
    public Sprite coin;
    public Sprite evilCoin;
    public Transform[] spawnPos;
    // public float spawnInterval = 1.5f;
    // public float speed = 2f;
    // Color colorOne = new Color(1f, 1f, 0f, 1f); // Yellow
    // Color colorTwo = new Color(0.5f, 0f, 0.5f, 1f); // Purple

    //Spawn variables 
    public float spawnDistance = 3f;
    private float distanceSinceLastSpawn = 0f;
    // private bool startSpawnMg = false;
 
    private MinigameManager gameManager;

    public bool startCoinMinigame;

    void Start()
    {
        startCoinMinigame = false;
        gameManager = GameObject.Find("MinigameManager").GetComponent<MinigameManager>();
    }

    //public void SpawnMgStart()
    //{
    //    StartCoroutine(SpawnObjects());
    //}

    void Update()
    {
        if (startCoinMinigame)
        {

            // Make spawnables after x amount of distance
            distanceSinceLastSpawn += gameManager.scrollSpeed * Time.deltaTime;

            if (distanceSinceLastSpawn >= spawnDistance)
            {
                SpawnObjects();
                distanceSinceLastSpawn = 0f;
            }

          
        }
    }

    void SpawnObjects()
    {
        Debug.Log("entered the spawn objects call at all");
        
        int slotOne = Random.Range(0, spawnPos.Length);
        int slotTwo;
        do
        {
            slotTwo = Random.Range(0, spawnPos.Length);
        } while (slotTwo == slotOne); // Ensure the slots are different

        Spawn(spawnPos[slotOne], coin, null); // This will be the good coin
        Spawn(spawnPos[slotTwo], null, evilCoin); // This will be the bad coin
        
    }

    void Spawn(Transform spawnPos, Sprite coinSprite, Sprite evilCoinSprite)
    {
        GameObject newSpawnable = Instantiate(spawnable, spawnPos.position, Quaternion.identity); // Make the spawnable

        SpriteRenderer spriteRenderer = newSpawnable.GetComponent<SpriteRenderer>(); // Access the spawnable's color
        spriteRenderer.sortingOrder = 0;
        // spriteRenderer.color = color; // Change the spawnable's color
        Animator anim = newSpawnable.GetComponent<Animator>();

        if (coinSprite != null)
        {
            spriteRenderer.sprite = coinSprite;
            newSpawnable.tag = "Coin";
            Debug.Log("Spawned a Coin with tag: " + newSpawnable.tag); // Debugging
            if (anim != null)
            {
                //GlitchAnimationCheck(anim, "GoodCoinAnimation", "PersonWalking");
                anim.Play("GoodCoinAnimation"); // 
            }
        }
        else if (evilCoinSprite != null)
        {
            spriteRenderer.sprite = evilCoinSprite;
            newSpawnable.tag = "EvilCoin";
            Debug.Log("Spawned an Evil Coin with tag: " + newSpawnable.tag); // Debugging
            if (anim != null)
            {
                //GlitchAnimationCheck(anim, "EvilCoinAnimation", "Trash");
                anim.Play("EvilCoinAnimation");
            }
        }
    }

    void GlitchAnimationCheck(Animator anim, string animNormal, string animGlitch)
    {
        bool isGlitch = gameManager.isGlitch;
        if (!isGlitch)
        {
            anim.Play(animNormal);
        }
        else
        {
            anim.Play(animGlitch);
        }
    }
}

    /*
    IEnumerator GlitchCheckRoutine() {
        return;
    }
    */