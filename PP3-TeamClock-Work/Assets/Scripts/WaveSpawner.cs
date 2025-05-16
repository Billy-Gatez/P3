using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [Header("Wave Setup")]
    [Range(30, 120)][SerializeField] float waveTimerLength; // timer minimum of 30 secs, max of 2 mins
    public Wave[] wave;
    private int currentWaveIndex = 0;
    private bool waitingForWave;

    [Header("Spawn Setup")]
    public Transform[] spawnPos;
    public float spawnTimer;
    int spawnDowntime = 0; //similar to spawner timeBetweenSpawn variable


    private void Start()
    {
        spawnTimer = 0;
        newWave();
    }
    // Update is called once per frame
    void Update()
    {

        if(gamemanager.instance.gameGoalCountText.text == "0" && gamemanager.instance.waveCount != 0 && !waitingForWave) //prevents new wave from spawning instantly after previous, start wave timer
        {
            waitingForWave = true;
            spawnTimer = waveTimerLength;
            gamemanager.instance.waveTimerPopup.SetActive(true);
        }
        if (spawnTimer  > 0)
        {
            spawnTimer -= 1 * Time.deltaTime;
            gamemanager.instance.waveTimerPopupTxt.text = spawnTimer.ToString("F0");
        }
        if (spawnTimer <= 0)
        {
            gamemanager.instance.waveTimerPopup.SetActive(false);
        }
        if(gamemanager.instance.waveCount != 0 && gamemanager.instance.gameGoalCountText.text == "0" && spawnTimer <= spawnDowntime) //start new wave when timer is done
        {
            newWave();
        }
    }

    public void newWave()
    {
        gamemanager.instance.waveCount -= 1;
        for (int i = 0; i < wave[currentWaveIndex].Enemies.Length; ++i)
        {
            int arrayInd = Random.Range(0, spawnPos.Length);
            Instantiate(wave[currentWaveIndex].Enemies[i], spawnPos[arrayInd].position, spawnPos[arrayInd].rotation);
            gamemanager.instance.updateGameGoal(1, 0);
        }
        currentWaveIndex += 1;
        if(currentWaveIndex >= wave.Length)
        {
            currentWaveIndex = wave.Length - 1;
        }
        waitingForWave = false;
    }
}

[System.Serializable]
public class Wave
{
    public GameObject[] Enemies;
}