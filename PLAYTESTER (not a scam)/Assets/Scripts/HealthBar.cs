using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class HealthBar : MonoBehaviour
{
    public Slider healthBar;
    public GameManagerScript GameManager;

    private void Start()
    {
        GameManager = GameObject.Find("Game Manager").GetComponent<GameManagerScript>();
        healthBar = GetComponent<Slider>();
        healthBar.maxValue = 100;
        healthBar.value = 100;
    }
    public void SetHealth(int hp)
    {
        healthBar.value = hp;
    }
}