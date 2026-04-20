using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private EnemyPool pool;

    [Header("Spawn")]
    [SerializeField] private Transform mapCenter;
    private float spawnHeight = 20f;
    private float spawnRadius = 15f;
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private int initialEnemies = 40;

    private float timer;

    void Start()
    {
        pool = GetComponent<EnemyPool>();
        SphereCollider sphere = mapCenter.GetComponent<SphereCollider>();
        spawnRadius = sphere.radius * mapCenter.lossyScale.x;
        spawnHeight = sphere.radius * mapCenter.lossyScale.y;

        for (int i = 0; i <= initialEnemies; i++)
        {
            SpawnEnemy();
        }
    }
    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            SpawnEnemy();
            timer = spawnInterval;
        }
    }

    void SpawnEnemy()
    {
        GameObject enemy = pool.Get();

        Vector2 circle = Random.insideUnitCircle * spawnRadius;

        Vector3 spawnPos = mapCenter.position +
                           new Vector3(circle.x, spawnHeight, circle.y);

        enemy.transform.position = spawnPos;
        enemy.transform.rotation = Quaternion.identity;
    }
}
