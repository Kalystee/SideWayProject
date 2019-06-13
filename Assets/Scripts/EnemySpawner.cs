using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public Transform[] spawnPoints;
    [SerializeField]
    float timeBetweenSpawn = 3f;
    float countdownToNextSpawn;
    public EnemyController enemy;
    
    // Start is called before the first frame update
    void Start()
    {
        if (spawnPoints.Length == 0)
        {
            Debug.LogError("No SpawnPoint referenced");
        }
        countdownToNextSpawn = timeBetweenSpawn;
    }

    // Update is called once per frame
    void Update()
    {
        if(countdownToNextSpawn <= 0)
        {
            StartCoroutine(SpawnEnemy(enemy));
        }
        else
        {
            countdownToNextSpawn -= Time.deltaTime;
        }
        
    }

    IEnumerator SpawnEnemy(EnemyController _enemy)
    {
        Debug.Log("Spawn Enemy");
        Transform sp = spawnPoints[Random.Range(0, spawnPoints.Length)];

        EnemyController enemySpawned = Instantiate(_enemy, sp.position, sp.rotation);
        enemySpawned.target = PlayerControler.Instance.EntityController;
        countdownToNextSpawn = timeBetweenSpawn;
        yield return new WaitForSeconds(this.timeBetweenSpawn);
    }
}
