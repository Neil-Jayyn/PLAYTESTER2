using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnable : MonoBehaviour
{
    public float speed = 2f;
    public float xThreshold; 

    // Update is called once per frame
    void Update()
    {
        transform.position += Vector3.right * speed * Time.deltaTime; // move spawnable to the right

        if (transform.position.x > xThreshold) // If the spawnable goes out of screen, remove it
        {
            Destroy(gameObject);
        }
    }
}
