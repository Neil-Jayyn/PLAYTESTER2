using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyMovement : MonoBehaviour
{ 
    public float speed;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float minSpeed;

    //color choice
    [SerializeField] Color color1= new Color(0f,32f,101f,1f);
    [SerializeField] private Color color2;
    [SerializeField] private Color color3;
    [SerializeField] private Color color4;
    [SerializeField] private Color color5;
    [SerializeField] private Color color6;
    [SerializeField] private Color color7;
    private int colorChoice;

    private int amountBounced;

    SpriteRenderer spriteRenderer;
 
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        speed=Random.Range(minSpeed,maxSpeed); //randomize speed of people
        SetPersonColor();
        amountBounced = 0;
  
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector2.left * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //if it hits the walls of the game, move other way only one time, then you leave the area
        if (other.gameObject.tag == "LBoundary")
        {
            if (amountBounced < 3)
            {
                speed *= -1;
                spriteRenderer.flipX = false;
                amountBounced++;
            }
            else {
                Destroy(gameObject);
            }
            

        }

        if (other.gameObject.tag == "RBoundary")
        {
            if (amountBounced < 3)
            {
                speed *= -1;
                spriteRenderer.flipX = true;
                amountBounced++;
            }
            else
            {
                Destroy(gameObject);
            }

        }
    }

    private void SetPersonColor()
    {  //set each person to diff color
        colorChoice = Random.Range(0, 8); //randomize index of choice
        switch (colorChoice)
        {
            case 0:
                spriteRenderer.color = color1;
                break;
            case 1:
                spriteRenderer.color = color2;
                break;
            case 2:
                spriteRenderer.color = color3;
                break;
            case 3:
                spriteRenderer.color = color4;
                break;
            case 4:
                spriteRenderer.color = color5;
                break;
            case 5:
                spriteRenderer.color = color6;
                break;
            case 6:
                spriteRenderer.color = color7;
                break;
            case 7:
                spriteRenderer.color = Color.white;
                break;
            default:
                spriteRenderer.color = Color.red;
                break;
        }
    }
}
