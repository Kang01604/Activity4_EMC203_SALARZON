using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro; 

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game State")]
    public bool isGameOver = false;
    public GameObject failUI; 

    [Header("Player Stats")]
    public int maxHP = 20;
    public int currentHP;
    public int coins = 0;

    [Header("References")]
    public HealthBarController healthBar; 
    public TextMeshProUGUI coinText;      
    public RectTransform coinTargetUI;

    [Header("Wave Settings")]
    public Transform[] quadraticPoints;
    public Transform[] cubicPoints;
    public float spawnInterval = 1.5f;

    // POOLING SYSTEM
    [Header("Pooling")]
    public GameObject enemyPrefab;
    public GameObject coinPrefab;
    public int initialPoolSize = 20;

    private List<GameObject> _enemyPool = new List<GameObject>();
    private List<GameObject> _coinPool = new List<GameObject>();

    // Active Enemy List for Turrets
    public List<EnemyController> activeEnemies = new List<EnemyController>();

    void Awake()
    {
        Instance = this;
        currentHP = maxHP;
        if (failUI != null) failUI.SetActive(false);
        
        InitializePools();
    }

    void Start()
    {
        UpdateCoinUI();
        StartCoroutine(SpawnWave());
    }

    void InitializePools()
    {
        // Pre-fill Enemy Pool
        for (int i = 0; i < initialPoolSize; i++)
        {
            GameObject obj = Instantiate(enemyPrefab);
            obj.SetActive(false);
            _enemyPool.Add(obj);
        }

        // Pre-fill Coin Pool
        for (int i = 0; i < initialPoolSize; i++)
        {
            GameObject obj = Instantiate(coinPrefab);
            obj.SetActive(false);
            _coinPool.Add(obj);
        }
    }

    public GameObject GetEnemyFromPool()
    {
        foreach (GameObject obj in _enemyPool)
        {
            if (!obj.activeInHierarchy)
            {
                obj.SetActive(true);
                return obj;
            }
        }
        // Expand pool if needed
        GameObject newObj = Instantiate(enemyPrefab);
        _enemyPool.Add(newObj);
        return newObj;
    }

    public GameObject GetCoinFromPool()
    {
        foreach (GameObject obj in _coinPool)
        {
            if (!obj.activeInHierarchy)
            {
                obj.SetActive(true);
                return obj;
            }
        }
        GameObject newObj = Instantiate(coinPrefab);
        _coinPool.Add(newObj);
        return newObj;
    }

    // Return to pool (instead of Destroy)
    public void ReturnEnemy(GameObject enemyObj)
    {
        enemyObj.SetActive(false);
        EnemyController ec = enemyObj.GetComponent<EnemyController>();
        if (activeEnemies.Contains(ec)) activeEnemies.Remove(ec);
    }

    public void ReturnCoin(GameObject coinObj)
    {
        coinObj.SetActive(false);
    }

    // GAME LOGIC

    IEnumerator SpawnWave()
    {
        for (int i = 0; i < 20; i++) // 20 enemies per wave
        {
            if (isGameOver) yield break;

            bool useCubic = (i % 2 != 0); 
            Transform[] chosenPath = useCubic ? cubicPoints : quadraticPoints;

            SpawnEnemy(chosenPath, useCubic);

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnEnemy(Transform[] path, bool isCubic)
    {
        // Get from Pool
        GameObject enemyObj = GetEnemyFromPool();
        
        // Reset Position
        enemyObj.transform.position = path[0].position;
        enemyObj.transform.rotation = Quaternion.identity;

        // Initialize Script
        EnemyController ec = enemyObj.GetComponent<EnemyController>();
        ec.Initialize(path, isCubic);

        // Register for Turrets
        activeEnemies.Add(ec);
    }

    public void TakeDamage(int damage)
    {
        if (isGameOver) return;
        currentHP -= damage;
        if (healthBar != null) healthBar.UpdateHealth(currentHP, maxHP);
        if (currentHP <= 0) GameOver();
    }

    public void AddCoin(int amount)
    {
        coins += amount;
        UpdateCoinUI();
    }

    void UpdateCoinUI()
    {
        // Force update the text
        if (coinText != null) 
            coinText.text = "Coins: " + coins.ToString();
    }

    void GameOver()
    {
        isGameOver = true;
        if (failUI != null) failUI.SetActive(true);
    }
}