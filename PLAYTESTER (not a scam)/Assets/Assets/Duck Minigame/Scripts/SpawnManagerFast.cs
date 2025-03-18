using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManagerFast : MonoBehaviour
{
    public GameObject duckPrefab;
    public GameObject trashPrefab;
    public Transform[] spawnPoints; // Where to spawn the objects
    public float spawnInterval = 1f; // Time between spawns
    public float showInterval = 1f; // Time spawnable will show up for
    public int spawnablesPerTime = 1; // Number of spawnables that will spawnable each time

    // Start is called before the first frame update
    void Start()
    {
       // StartCoroutine(SpawnItems());
    }

    public void SpawnStart() {
        StartCoroutine(SpawnItems());
    }

    // Coroutine to spawn items (ducks and trash)
    IEnumerator SpawnItems()
    {
        while (true)
        {
            // Wait for the next spawn interval
            yield return new WaitForSeconds(spawnInterval);

            List<int> usedIndexes = new List<int>();

            int index;
            for (int i = 0; i < spawnablesPerTime; i++)
            {
                do
                {
                    index = Random.Range(0, spawnPoints.Length);
                } while (usedIndexes.Contains(index));

                usedIndexes.Add(index);
                Transform spawnPoint = spawnPoints[index];

                bool isDuck = Random.value > 0.50;  // 50% chance to spawn a duck

                if (isDuck)
                {
                    StartCoroutine(SpawnDuck(spawnPoint));
                }
                else
                {
                    StartCoroutine(SpawnTrash(spawnPoint));
                }
            }
        }
    }

    IEnumerator SpawnDuck(Transform spawnPoint)
    {

        GameObject newDuck = Instantiate(duckPrefab, new Vector3(spawnPoint.position.x, spawnPoint.position.y, -2), Quaternion.identity);
        yield return new WaitForSeconds(showInterval);
        Destroy(newDuck);
    }

    IEnumerator SpawnTrash(Transform spawnPoint)
    {
        GameObject newTrash = Instantiate(trashPrefab, new Vector3(spawnPoint.position.x, spawnPoint.position.y, -2), Quaternion.identity);
        yield return new WaitForSeconds(showInterval);
        Destroy(newTrash);
    }
}
