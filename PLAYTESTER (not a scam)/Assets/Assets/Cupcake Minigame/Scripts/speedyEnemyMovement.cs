using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class speedyEnemyMovement : MonoBehaviour
{
    [SerializeField] float speed = 7f;
    SpriteRenderer spriteRenderer;

    [SerializeField] public bool isLeftSpawner;
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        SetDirectionPerson(isLeftSpawner);
        if (isLeftSpawner)
        {
            transform.Translate(Vector2.right * speed * Time.deltaTime);
        }
        else
        {
            transform.Translate(Vector2.left * speed * Time.deltaTime);
        }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //if it hits the walls of the game, move other way

        if (other.gameObject.tag == "RBoundary")
        {
            Destroy(gameObject);

        }
        if (other.gameObject.tag == "LBoundary")
        {
            Destroy(gameObject);

        }
    }

    public void SetDirectionPerson(bool isLeftSpawner)
    {
        if (isLeftSpawner)
        {
            spriteRenderer.flipX = false;
        }
        else
        {
            spriteRenderer.flipX = true;
        }
    }
}
