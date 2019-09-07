using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public bool devMode;

    public Enemy enemy;
    public Wave[] wave;
    Wave currentWave;

    int currentWaveNumber;
    int enemiesRemainingToSpawn;
    int enemyRemainingAlive;
    float nextTimeToSpawn;

    MapGenerator map;
    LivingEntity playerEntity;
    Transform playerTransform;

    float timeBetweenCampChecks = 2f;
    float nextCampCheck;
    float campThresholdDist = 1.5f;
    bool isCamping;
    Vector3 oldCampPosition;

    bool isDisabled;

    public event System.Action<int> OnNewWave;

    private void Start()
    {
        nextCampCheck = Time.time + timeBetweenCampChecks;
        playerEntity = FindObjectOfType<Player>();
        playerTransform = playerEntity.transform;
        playerEntity.OnDeath += OnPlayerDeath;
        oldCampPosition = playerTransform.position;
        map = FindObjectOfType<MapGenerator>();
        NextWave();
    }
    private void Update()
    {
        //Debug.Log(map.map[1]);
        if (!isDisabled)
        {
            if (Time.time > nextCampCheck)
            {
                nextCampCheck = Time.time + timeBetweenCampChecks;
                isCamping = Vector3.Distance(oldCampPosition, playerTransform.position) < campThresholdDist;
                oldCampPosition = playerTransform.position;
            }
            if ((enemiesRemainingToSpawn > 0 || currentWave.isInfinite) && Time.time > nextTimeToSpawn)
            {
                enemiesRemainingToSpawn--;
                nextTimeToSpawn = Time.time + currentWave.timeBetweenSpawns;
                StartCoroutine(SpawnEnemy());
            }
        }

        if (devMode)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                StopCoroutine(SpawnEnemy());
                foreach (Enemy enemy in FindObjectsOfType<Enemy>())
                {
                    GameObject.Destroy(enemy.gameObject);
                }
                NextWave();
            }
        }
    }
    IEnumerator SpawnEnemy()
    {
        float spawnDelay=1;
        float flashTileSpeed=4;
        Transform spawnTile = map.GetRandomOpenTile();
        if (isCamping)
        {
            spawnTile = map.GetTileFromPosition(playerTransform.position);
        }
        Material tileMat = spawnTile.GetComponent<Renderer>().material;
        Color initialColor = Color.white;
        Color flashColor = Color.red;
        float spawnTimer = 0;
        while (spawnTimer<spawnDelay)
        {
            spawnTimer += Time.deltaTime;
            tileMat.color = Color.Lerp(initialColor, flashColor, Mathf.PingPong(spawnTimer * flashTileSpeed, 1));
            yield return null;
        }
        Enemy spawnedEnemy = Instantiate(enemy, Vector3.up + spawnTile.position, Quaternion.identity) as Enemy;
        spawnedEnemy.SetCharacteristics(currentWave.speed, currentWave.enemyHealth, currentWave.hitsToKill, currentWave.skinColor);
        spawnedEnemy.OnDeath += OnEnemyDeath;
    }

    void OnPlayerDeath()
    {
        isDisabled = true;
    }

    void OnEnemyDeath()
    {
        enemyRemainingAlive--;
        if (enemyRemainingAlive == 0)
        {
            NextWave();
        }
    }

    void ResetPlayerPosition()
    {
        playerTransform.position = map.GetTileFromPosition(Vector3.zero).position + (Vector3.up * 3f);
    }

    void NextWave()
    {
        if (currentWaveNumber > 0)
        {
            AudioManager.instance.Play2DSound("Level Complete");
        }
        currentWaveNumber++;
        if (currentWaveNumber - 1 < wave.Length)
        {
            currentWave = wave[currentWaveNumber - 1];
            enemiesRemainingToSpawn = currentWave.enemyCount;
            enemyRemainingAlive = enemiesRemainingToSpawn;
            if (OnNewWave != null)
            {
                OnNewWave(currentWaveNumber);
            }
            ResetPlayerPosition();

        }
    }

    [System.Serializable]
   public class Wave
    {
        public bool isInfinite;
        public int enemyCount;
        public float timeBetweenSpawns;
        public float speed;
        public int hitsToKill;
        public int enemyHealth;
        public Color skinColor;
    }
}
