using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private EnemyPool pool;

    [Header("Spawn")]
    [SerializeField] private Transform mapCenter;
    private float spawnHeight = 20f;
    private float spawnRadius = 15f;
    [SerializeField] private float spawnInterval = 2f;

    private float timer;

    void Start()
    {
        pool = GetComponent<EnemyPool>();
        SphereCollider sphere = mapCenter.GetComponent<SphereCollider>();
        spawnRadius = sphere.radius * mapCenter.lossyScale.x;
        spawnHeight = sphere.radius * mapCenter.lossyScale.y;
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

        Vector3 randomPos = Random.insideUnitSphere * spawnRadius;
        randomPos.y = 0;

        Vector3 spawnPos = transform.position + randomPos + Vector3.up * spawnHeight;

        enemy.transform.position = spawnPos;
        enemy.transform.rotation = Quaternion.identity;
    }
}
