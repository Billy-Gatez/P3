using UnityEngine;

public class spawner : MonoBehaviour
{
    [SerializeField] GameObject objectToSpawn;
    [SerializeField] int numToSpawn;
    [SerializeField] int timeBetweenSpawn;
    [SerializeField] Transform[] spawnPos;

    float spawnTimer;
    int spawnCount;
    bool startSpawning;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //gamemanager.instance.updateGameGoal(numToSpawn, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (startSpawning)
        {
            spawnTimer += Time.deltaTime;
            if (spawnCount < numToSpawn && spawnTimer >= timeBetweenSpawn)
            {
                spawn();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            startSpawning = true;
        }
    }
    void spawn()
    {
        int arrayPos = Random.Range(0, spawnPos.Length);

        // Instantiate the enemy at a random spawn position
        Instantiate(objectToSpawn, spawnPos[arrayPos].position, spawnPos[arrayPos].rotation);

        // Update the game goal each time an enemy is spawned
        gamemanager.instance.updateGameGoal(1, 0); // Update the game goal by 1 for each spawned enemy

        spawnCount++;
        spawnTimer = 0;
    }

}
