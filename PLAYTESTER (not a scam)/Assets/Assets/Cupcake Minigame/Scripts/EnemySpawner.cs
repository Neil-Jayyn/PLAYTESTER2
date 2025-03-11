using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject person;

    [SerializeField] private int numEnemies;
    private int currentNumEnemies;
    private float waitTime;
    [SerializeField] public bool isLeftEnemySpawner;

    public bool playCupcakeMinigame; //turned to true by the cupcake game manager script


    // Start is called before the first frame update
    void Start()
    {
        playCupcakeMinigame = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (playCupcakeMinigame)
        {
            //spawn enemies after each spawn wait time with a max amount of people
            waitTime -= Time.deltaTime;
            if ((waitTime <= 0 || currentNumEnemies==0)&& currentNumEnemies < numEnemies)
            {
                person.GetComponent<enemyMovement>().isLeftSpawner = isLeftEnemySpawner;
                Instantiate(person, transform.position, Quaternion.identity);
                setSpawnTime();
                currentNumEnemies++;
            }
        }
    }

    void setSpawnTime(){
        waitTime=Random.Range(0.5f,4.0f);
    }

    public void KilledEnemy()
    {
        currentNumEnemies--;
        return;
    }
}
