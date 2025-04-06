using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject person;
    public GameObject person1;
    public GameObject person3;
    public GameObject person2;
    public GameObject speedyPerson;

    //num of enemies handling
    [SerializeField] private int numEnemies;
    private int currentNumEnemies;
    [SerializeField]private float waitTime;


    [SerializeField] public bool isLeftEnemySpawner;

    public bool playCupcakeMinigame; //turned to true by the cupcake game manager script

    //rare speedy sprite handling
    private float speedyWaitTime;
    [SerializeField]public float constSpeedWaitTime;
    public bool isSpeedyObject=false;
    public int numSpeedyObjects=0;

    CupcakeGameManager cupcakeManager;
    float timeRemaining;

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

            if (speedyWaitTime <= 0 && numSpeedyObjects<1)
            {
                isSpeedyObject = true;
            }

            if ((waitTime <= 0 || currentNumEnemies == 0))
            {
                //Debug.Log("IsSpeedy"+isSpeedyObject);
                if (!isSpeedyObject)
                {
                    GameObject person = PersonChoice();
                    //Debug.Log(isLeftEnemySpawner + " should spawn");
                    EnemiesSpeedHandling(timeRemaining);
                    person.GetComponent<enemyMovement>().isLeftSpawner = isLeftEnemySpawner;
                    Instantiate(person, transform.position, Quaternion.identity); 
                }
                else
                {
                    isSpeedyObject = false;
                    if (numSpeedyObjects < 1)
                    {
                        speedyPerson.GetComponent<speedyEnemyMovement>().isLeftSpawner = isLeftEnemySpawner;
                        Instantiate(speedyPerson, transform.position, Quaternion.identity);
                        setSpeedySpawnTime();
                       
                        numSpeedyObjects++;
                    }
                    
                }
                setSpawnTime();
                currentNumEnemies++;
            }
        }
        else {
            //Debug.Log("Spawner Ends");
            resetSpawner();
        }
    }

    void setSpawnTime(){
        waitTime=Random.Range(0.5f,2.0f);
        isSpeedyObject = false;
    }

    void resetSpawner() {
        currentNumEnemies = 0;
        numSpeedyObjects= 0;
        setSpawnTime();
        setSpeedySpawnTime() ;
        person.GetComponent<enemyMovement>().ResetSpeedRange(2.5f, 3.5f);


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

    public void EnemiesSpeedHandling(float time) {
        timeRemaining = cupcakeManager.timeRemaining;
        float min=2.5f;float max=3.5f;
        if (time > 45f) {
            
        }
        else if (time <= 45f)
        {
            min = 2.8f;max = 3.8f;
        }
        else if (time <= 30f)
        {
            min = 3.0f;max = 4.2f;
        }
        else if (time <= 15f)
        {
            min = 3.2f;max = 4.5f;
        }
        else {
            min = 3.5f;max = 4.7f;
        }
        //Debug.Log(time);
        person.GetComponent<enemyMovement>().SetSpeedRange(min, max);
    }

    public GameObject PersonChoice() {
        int choice=Random.Range(0, 3);
        switch (choice) { 
            case 0:
                return person1;
            case 1:
                return person2;
            case 2: 
                return person3;
            default: 
                return person1;

        }

    }
}
