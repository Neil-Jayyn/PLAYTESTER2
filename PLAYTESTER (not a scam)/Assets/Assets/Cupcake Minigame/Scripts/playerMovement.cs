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

    // Start is called before the first frame update
    void Start()
    {
        playCupcakeMinigame = false;
        //anim= GetComponent<Animator>();
        anim.SetBool("isReady", false);


    }

    void Awake() { 
        sfxDrop=GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playCupcakeMinigame)
        {
            anim.SetBool("isReloading", true);
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
                if (isReady==true && readyTime >= 0.0F) //ding 
                {
                 
                    anim.SetBool("isReady", true);
                    //have isReady state true
                    readyTime -= Time.deltaTime;
                    //TODO: SFX FOR DING
                    
                }
                else
                {

                    anim.SetBool("isReady", false);
                    if (Input.GetKeyDown(KeyCode.Space)) //player drops cupcake
                    {
                        sfxDrop.Play();
                        Instantiate(cupcake, transform.position, Quaternion.identity);
                        waitTime = 1.5f;
                        isReloading = true;
                        anim.SetBool("isReloading", true);
                        readyTime = 1.0f;

                    }
                    else
                    { //else overtime starts
                        //TODO: SFX FOR OVERTIME
                        anim.SetBool("isReloading", false);
                        if (overtime <= 0.0f)
                        {
                            sfxDrop.Play();
                            Instantiate(cupcake, transform.position, Quaternion.identity);
                            waitTime = 1.5f;
                            overtime = 3.0f;
                            isReloading = true;
                            anim.SetBool("isReloading", true);
                            anim.SetBool("isReady",false);
                            readyTime = 1.0f;
                        }
                        overtime -= Time.deltaTime;
                    }
                }
            }
        }

    }


}
