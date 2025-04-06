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
        isGlitch = false;
        gameManager=GameObject.Find("CupcakeGameManager").GetComponent<CupcakeGameManager>();
        spriteRenderer=GetComponent<SpriteRenderer>();

    }

    void Awake() { 
        sfxDrop=GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playCupcakeMinigame)
        {
            if (!gameManager.isGlitch)
            {
                spriteRenderer.sprite=normal;
                //anim.SetBool("isReloading",true);
                AnimationYes("isReloading", gameManager.isGlitch);
                anim.speed = reloadRate;
                //reload time
                waitTime -= Time.deltaTime;

                //player input
                if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                {
                    transform.position += Vector3.left * Time.deltaTime * speed;
                }
                if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                {
                    transform.position += Vector3.right * Time.deltaTime * speed;
                }

                //reload time and overtime handling
                if (waitTime <= 0.0f)//if cupcake is ready to drop
                {
                    anim.speed = 1;
                    isReady = true;
                    if (isReady == true && readyTime >= 0.0F) //ding 
                    {

                        //anim.SetBool("isReady", true);
                        AnimationYes("isReady", gameManager.isGlitch);
                        //have isReady state true
                        readyTime -= Time.deltaTime;
                        //TODO: SFX FOR DING

                    }
                    else
                    {

                        AnimationNo("isReady", gameManager.isGlitch);
                        //anim.SetBool("isReady", false);
                        if (Input.GetKeyDown(KeyCode.Space)) //player drops cupcake
                        {
                            sfxDrop.Play();
                            Instantiate(cupcake, transform.position, Quaternion.identity);
                            waitTime = 1.5f;
                            isReloading = true;
                            AnimationYes("isReloading", gameManager.isGlitch);
                            //anim.SetBool("isReloading", true);
                            readyTime = 1.0f;

                        }
                        else
                        { //else overtime starts
                          //TODO: SFX FOR OVERTIME
                            AnimationNo("isReloading", gameManager.isGlitch);
                            //anim.SetBool("isReloading", false);
                            if (overtime <= 0.0f)
                            {
                                sfxDrop.Play();
                                Instantiate(cupcake, transform.position, Quaternion.identity);
                                waitTime = 1.5f;
                                overtime = 3.0f;
                                isReloading = true;
                                AnimationYes("isReloading", gameManager.isGlitch);
                                //anim.SetBool("isReloading", true);
                                //anim.SetBool("isReady",false); 
                                AnimationNo("isReady", gameManager.isGlitch);
                                readyTime = 1.0f;
                            }
                            overtime -= Time.deltaTime;
                        }
                    }
                }
            }
            else { anim.enabled = false; spriteRenderer.sprite = glitched; }
        }

    }

    void AnimationYes(string animation, bool isGlitch) {
        if (!isGlitch) {
            anim.SetBool(animation, true);
        }
    }

    void AnimationNo(string animation, bool isGlitch) {
        if (!isGlitch) { 
            anim.SetBool(animation, false);

        }
    }

    //void AnimationCheck() { 
      


}
