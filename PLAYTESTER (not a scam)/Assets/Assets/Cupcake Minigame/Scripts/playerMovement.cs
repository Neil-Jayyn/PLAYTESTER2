using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMovement : MonoBehaviour
{
    public GameObject cupcake;
    public GameObject bomb;
    public float speed = 1.85f;

    public float reloadRate = 1;

    //reload time and overtime 
    private float waitTime = 1.5f;
    private float overtime = 3.0f;
    private float readyTime = 0.5f;


    public bool playCupcakeMinigame; //turned to true by the cupcake game manager script
    AudioSource sfxDrop;
    public AudioClip[] sfxDrops;

    public AudioSource sfxOven;
    public AudioClip[] sfxDrone;
    public AudioClip[] sfxOvenTick;


    public Animator anim;
    public bool isReloading=true;
    public bool isReady=false;

    public bool isGlitch;
    CupcakeGameManager gameManager;

    SpriteRenderer spriteRenderer;
    public Sprite normal;
    public Sprite glitched;

    // Start is called before the first frame update
    void Start()
    {
        playCupcakeMinigame = false;
        //anim= GetComponent<Animator>();
        anim.SetBool("isReady", false);
        anim.SetBool("isGlitch", false);
        isGlitch = false;
        gameManager=GameObject.Find("CupcakeGameManager").GetComponent<CupcakeGameManager>();
        spriteRenderer=GetComponent<SpriteRenderer>();

    }

    void Awake() { 
        sfxDrop=GetComponent<AudioSource>();

        //if (sfxOven != null) { 
       //     sfxOven=gameObject.AddComponent<AudioSource>();
       // }
        

    }

    // Update is called once per frame
    void Update()
    {
        AnimationCheck(); //handles animation

        if (playCupcakeMinigame)
        {
            //player input
            PlayerInput();

            //Drop sfx
            SfxDropping(gameManager.isGlitch);

            //Drone sfx if it is glitched
            //SfxDroneMoving(gameManager.isGlitch);

            //Oven
            //SfxOvenTick();
                       
                //reload time
                waitTime -= Time.deltaTime;

                //reload time and overtime handling
                if (waitTime <= 0.0f)//if cupcake is ready to drop
                {
                    isReady = true;
                    if (isReady == true && readyTime >= 0.0F) //ding 
                    {
                    sfxDrop.clip = sfxOvenTick[0];
                    sfxDrop.Play();
                        //have isReady state true
                        readyTime -= Time.deltaTime;
                        //TODO: SFX FOR DING
                    }
                    else
                    {  // overtime starts
                        if (overtime <= 0.0f)  //waittime <0 and isReady ==false 0r readyTime<0
                        {
                            //TODO: SFX OVERTIME
                            sfxDrop.Play();
                            Instantiate(cupcake, transform.position, Quaternion.identity);
                            waitTime = 1.5f;
                            overtime = 3.0f;
                            isReloading = true;

                            readyTime = 1.0f;
                        }
                        overtime -= Time.deltaTime;
                    }

                    if (Input.GetKeyDown(KeyCode.Space)) //player drops cupcake
                    {
                        sfxDrop.Play();
                        Instantiate(cupcake, transform.position, Quaternion.identity);
                        waitTime = 1.5f;
                        isReloading = true;
                        isReady = false;


                        readyTime = 1.0f;
                    }
                }

            SfxDropping(gameManager.isGlitch);
        }
    }

    void AnimationCheck() {
        anim.speed = reloadRate;
        if (!gameManager.isGlitch)
        {
            anim.SetBool("isGlitch", false);
            if (playCupcakeMinigame)
            { //anim starts after mg starts
                anim.SetBool("isReloading", true);
                if (waitTime <= 0.0f)
                { //oven finished reloading
                    anim.SetBool("isReloading", false);
                    if (readyTime >= 0.0f && isReady)
                    { //ding and ready animation starts
                        anim.SetBool("isReady", true);
                    }
                    else
                    { //overtime animation starts
                        anim.SetBool("isReady", false);
                        if (overtime <= 0.0f)
                        { //if overtime runs out, restart animation
                            anim.SetBool("isReloading", true);
                            anim.SetBool("isReady", false);
                        }
                    }

                    if (Input.GetKeyDown(KeyCode.Space))
                    { //if dropped down restart animation
                        anim.SetBool("isReloading", true);
                        anim.SetBool("isReady", false);
                    }
                }
            }
        }
        else {
            anim.SetBool("isGlitch", true);
            //anim.enabled = false;
            //SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            //spriteRenderer.sprite = glitched;
        }
    }

    void PlayerInput() {
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            transform.position += Vector3.left * Time.deltaTime * speed;
            spriteRenderer.flipX = false;
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            transform.position += Vector3.right * Time.deltaTime * speed;
            spriteRenderer.flipX = true;
        }
    }

    private void SfxDropping(bool isGlitch) {
        // 0:cupcake drop 1:bomb drop
        if (!isGlitch)
        {
            sfxDrop.clip = sfxDrops[0];
        }
        else {
            sfxDrop.clip = sfxDrops[1];
        }
    }

    private void SfxDroneMoving(bool isGlitch) {
        if (isGlitch==true)
        {
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                //if oven moves
                sfxOven.clip = sfxDrone[0];
                sfxOven.Play();

            }
            else
            {
                //if oven is stationary
                sfxOven.clip = sfxDrone[1];
                sfxOven.Play();
            }


            
        }
    }

    private void SfxOvenTick() {
        if (anim.GetBool("isReady"))
        {
            sfxOven.clip = sfxOvenTick[0];
        }
        else if (anim.GetBool("isReloading"))
        {
            sfxOven.clip = sfxOvenTick[1];
        }
        if (sfxOven != null)
         {
            sfxOven.Play();
        }
    }

    private void SoundCheck(bool isGlitch)
    {
        

            if (anim.GetBool("isReady"))
            {
                //ding ding sound if ready
            }
            else if (anim.GetBool("isReloading"))
            {
                //if oven is reloading
            }
        
    }

    public void UpdateReloadRate(float multiplier) { 
        reloadRate= reloadRate * multiplier;
    }

}
