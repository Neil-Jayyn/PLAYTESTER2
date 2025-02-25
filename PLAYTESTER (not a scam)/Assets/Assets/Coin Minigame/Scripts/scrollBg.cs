using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scrollBg : MonoBehaviour
{
    public float speed = 2f;
    public float xThreshold;
    public float xResetTo;

    // Update is called once per frame
    void Update()
    {
        transform.position += Vector3.right * speed * Time.deltaTime; // Move background to the right

        if (transform.position.x > xThreshold) // Check out of bounds
        {
            transform.position = new Vector3(xResetTo, transform.position.y, transform.position.z); // Reset background
        }    
    }
}
