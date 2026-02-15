using System;
using System.Collections;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [System.Serializable]
    public struct WaveConfig
    {
        public int count;
        public float spawnInterval;
    }

    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private WaveConfig[] waves =
    {
        new WaveConfig { count = 3, spawnInterval = 0.7f },
        new WaveConfig { count = 5, spawnInterval = 0.7f },
        new WaveConfig { count = 7, spawnInterval = 0.7f },
        new WaveConfig { count = 9, spawnInterval = 0.7f }
    };
    [SerializeField] private float interWaveDelay = 2f;

    private int aliveEnemies;
    private int currentWave;
    private bool lastWaveSpawned;
    private bool winTriggered;
    private GameRoot gameRoot;

    public int CurrentWave => currentWave;
    public int AliveEnemies => aliveEnemies;
    public event Action<int> WaveChanged;

    private void Start()
    {
        gameRoot = GameRoot.Instance;
        if (gameRoot == null)
        {
            gameRoot = FindObjectOfType<GameRoot>();
        }

        if (!ValidateSetup())
        {
            return;
        }

        StartCoroutine(RunWavesCoroutine());
    }

    private bool ValidateSetup()
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("WaveManager: Enemy Prefab is not assigned. Wave spawning stopped.");
            return false;
        }

        if (waves == null || waves.Length == 0)
        {
            Debug.LogError("WaveManager: Waves are not configured. Wave spawning stopped.");
            return false;
        }

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("WaveManager: Spawn Points list is empty. Wave spawning stopped.");
            return false;
        }

        bool hasAnyValidSpawnPoint = false;
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            if (spawnPoints[i] != null)
            {
                hasAnyValidSpawnPoint = true;
                break;
            }
        }

        if (!hasAnyValidSpawnPoint)
        {
            Debug.LogError("WaveManager: All Spawn Points are null. Wave spawning stopped.");
            return false;
        }

        return true;
    }

    private IEnumerator RunWavesCoroutine()
    {
        interWaveDelay = Mathf.Max(0f, interWaveDelay);
        currentWave = 0;

        for (int waveIndex = 0; waveIndex < waves.Length; waveIndex++)
        {
            currentWave = waveIndex + 1;
            WaveChanged?.Invoke(currentWave);

            if (gameRoot != null)
            {
                gameRoot.SetCurrentWave(currentWave);
            }

            int enemiesToSpawn = Mathf.Max(0, waves[waveIndex].count);
            float spawnInterval = Mathf.Max(0.01f, waves[waveIndex].spawnInterval);

            for (int i = 0; i < enemiesToSpawn; i++)
            {
                SpawnEnemy();

                if (i < enemiesToSpawn - 1)
                {
                    yield return new WaitForSeconds(spawnInterval);
                }
            }

            // Do not start the next wave until all enemies from the current wave are dead.
            while (aliveEnemies > 0)
            {
                yield return null;
            }

            if (waveIndex < waves.Length - 1)
            {
                yield return new WaitForSeconds(interWaveDelay);
            }
        }

        lastWaveSpawned = true;
        TryFinishWithWin();
    }

    private void SpawnEnemy()
    {
        Transform point = GetRandomSpawnPoint();
        if (point == null)
        {
            Debug.LogError("WaveManager: Could not find a valid Spawn Point for enemy spawn.");
            return;
        }

        GameObject enemyInstance = Instantiate(enemyPrefab, point.position, point.rotation);
        if (enemyInstance == null)
        {
            Debug.LogError("WaveManager: Instantiate returned null enemy instance.");
            return;
        }

        EnemyHealth enemyHealth = enemyInstance.GetComponent<EnemyHealth>();
        if (enemyHealth == null)
        {
            Debug.LogError("WaveManager: Spawned enemy has no EnemyHealth component. Destroying instance.");
            Destroy(enemyInstance);
            return;
        }

        aliveEnemies++;
        enemyHealth.Died += OnEnemyDied;
    }

    private Transform GetRandomSpawnPoint()
    {
        int startIndex = UnityEngine.Random.Range(0, spawnPoints.Length);
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            int index = (startIndex + i) % spawnPoints.Length;
            Transform candidate = spawnPoints[index];
            if (candidate != null)
            {
                return candidate;
            }
        }

        return null;
    }

    private void OnEnemyDied(EnemyHealth deadEnemy)
    {
        if (deadEnemy != null)
        {
            deadEnemy.Died -= OnEnemyDied;
        }

        aliveEnemies = Mathf.Max(0, aliveEnemies - 1);
        TryFinishWithWin();
    }

    private void TryFinishWithWin()
    {
        if (winTriggered || !lastWaveSpawned || aliveEnemies > 0)
        {
            return;
        }

        if (gameRoot == null)
        {
            gameRoot = GameRoot.Instance;
            if (gameRoot == null)
            {
                gameRoot = FindObjectOfType<GameRoot>();
            }
        }

        if (gameRoot == null || !gameRoot.IsPlaying)
        {
            return;
        }

        winTriggered = true;
        gameRoot.EndGame(true);
    }

    private void OnValidate()
    {
        interWaveDelay = Mathf.Max(0f, interWaveDelay);
    }
}
