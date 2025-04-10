using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject duckPrefab;   
    public GameObject trashPrefab;  
    public Transform[] spawnPoints; // Where to spawn the objects 
    public float spawnInterval = 2f; // Time between spawns
    public float showInterval = 2f; // Time spawnable will show up for
    public int spawnablesPerTime = 1; // Number of spawnables that will spawnable each time
    private List<int> blockedIndexes = new List<int>();

    DuckGameManager duckGameManager;

    // Start is called before the first frame update
    void Start()
    {
        duckGameManager = GameObject.Find("DuckGameManager").GetComponent<DuckGameManager>();
       
    }

    public void SpawnStart()
    {
        StartCoroutine(SpawnItems());
    }

    // Coroutine to spawn items (ducks and trash)
    IEnumerator SpawnItems()
    {
        while (true) 
        {
            // Wait for the next spawn interval
            yield return new WaitForSeconds(spawnInterval);

            

            for (int i = 0; i < spawnablesPerTime; i++)
            {
                int index;
                int attempts = 0;
                int maxAttempts = 10;
                do
                {
                    index = Random.Range(0, spawnPoints.Length);
                    attempts++;
                } while ((blockedIndexes.Contains(index)) && attempts < maxAttempts);

                blockedIndexes.Add(index);
                Transform spawnPoint = spawnPoints[index];

                bool isDuck = Random.value > 0.5f;  // 50% chance to spawn a duck or trash
                
                if (isDuck)
                {
                    StartCoroutine(SpawnDuck(spawnPoint, index));
                }
                else
                {
                    StartCoroutine(SpawnTrash(spawnPoint, index));
                }
            }
        }
    }

    IEnumerator SpawnDuck(Transform spawnPoint, int index)
    {
        
        GameObject newDuck = Instantiate(duckPrefab, new Vector3(spawnPoint.position.x, spawnPoint.position.y, -2), Quaternion.identity);
        yield return new WaitForSeconds(showInterval);
        Destroy(newDuck);
        blockedIndexes.Remove(index);
    }

    IEnumerator SpawnTrash(Transform spawnPoint, int index)
    {
        GameObject newTrash = Instantiate(trashPrefab, new Vector3(spawnPoint.position.x, spawnPoint.position.y, -2), Quaternion.identity);
        yield return new WaitForSeconds(showInterval);
        Destroy(newTrash);
        blockedIndexes.Remove(index);
    }
}
