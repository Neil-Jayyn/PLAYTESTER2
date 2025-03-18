using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject person;
    public GameObject speedyPerson;

    [SerializeField] private int numEnemies;
    private int currentNumEnemies;
    private float waitTime;
    [SerializeField] public bool isLeftEnemySpawner;
    public bool playCupcakeMinigame; //turned to true by the cupcake game manager script


    //rare speedy sprite
    CupcakeGameManager cupcakeManager;

    private float speedyWaitTime;
    [SerializeField]public float constSpeedWaitTime;
    public bool isSpeedyObject=false;
    public int numSpeedyObjects=0;

    // Start is called before the first frame update
    void Start()
    {
        playCupcakeMinigame = false;
        cupcakeManager = GameObject.Find("CupcakeGameManager").GetComponent<CupcakeGameManager>();
        setSpeedySpawnTime();
    }

    // Update is called once per frame
    void Update()
    {
        if (playCupcakeMinigame)
        {
            //spawn enemies after each spawn wait time with a max amount of people
            waitTime -= Time.deltaTime;
            speedyWaitTime -= Time.deltaTime;

            if (speedyWaitTime <= 0)
            {
                isSpeedyObject = true;
            }

            if ((waitTime <= 0 || currentNumEnemies == 0))
            {
                if (!isSpeedyObject)
                {

                    person.GetComponent<enemyMovement>().isLeftSpawner = isLeftEnemySpawner;
                    Instantiate(person, transform.position, Quaternion.identity);
                    setSpawnTime();
                    currentNumEnemies++;
                }
                else
                {
                    if (numSpeedyObjects < 1)
                    {
                        speedyPerson.GetComponent<speedyEnemyMovement>().isLeftSpawner = isLeftEnemySpawner;
                        Instantiate(speedyPerson, transform.position, Quaternion.identity);
                        setSpeedySpawnTime();
                        setSpawnTime();
                        currentNumEnemies++;
                        numSpeedyObjects++;
                    }
                }
            }
        }
        else {
            resetSpawner();
        }
    }

    void setSpawnTime(){
        waitTime=Random.Range(0.5f,2.0f);
    }

    void resetSpawner() {
        currentNumEnemies = 0;
        numSpeedyObjects= 0;
        setSpawnTime();
        setSpeedySpawnTime() ;


    }
    public void setSpeedySpawnTime() {
        isSpeedyObject = false;
        speedyWaitTime = constSpeedWaitTime;
    }

    public void SetNumOfEnemies(int num) {
        numEnemies = num;
    }

    public void KilledEnemy()
    {
        currentNumEnemies--;
        return;
    }
}
