using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;
    private EnemyPool pool;

    public Vector3? LastKnownPlayerPos { get; private set; }

    [Header("References")]
    [SerializeField] private Transform mapCenter;
    private MapPoints map;
    public MapPoints Map => map;
    [SerializeField] private PlayerModel player;

    [Header("Spawn")]
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private int initialEnemies = 20;

    [Header("Flying Enemies")]
    [SerializeField] private FlyingEnemyModel flyingEnemyPrefab;
    [SerializeField] private int initialFlyingEnemies = 4;
    [SerializeField] private float flyingHeight = 72f;

    private float spawnHeight;
    private float spawnRadius;
    private float timer;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        pool = GetComponent<EnemyPool>();
        map = FindAnyObjectByType<MapPoints>();
    }

    private void Start()
    {
        if (player == null)
            player = FindAnyObjectByType<PlayerModel>();

        SphereCollider sphere = mapCenter.GetComponent<SphereCollider>();

        spawnRadius = sphere.radius * mapCenter.lossyScale.x;
        spawnHeight = sphere.radius * mapCenter.lossyScale.y;

        for (int i = 0; i < initialEnemies; i++)
        {
            SpawnEnemy();
        }
        for (int i = 0; i < initialFlyingEnemies; i++)
        {
            SpawnFlyingEnemy();
        }
    }

    private void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            SpawnEnemy();
            timer = spawnInterval;
        }
    }

    private void SpawnEnemy()
    {
        GameObject enemy = pool.Get();

        Vector2 circle = Random.insideUnitCircle * spawnRadius;

        Vector3 spawnPos = mapCenter.position +
                           new Vector3(circle.x, spawnHeight, circle.y);

        enemy.transform.SetPositionAndRotation(spawnPos, Quaternion.identity);

        // Inicializar el enemigo
        EnemyBase enemyBase = enemy.GetComponent<EnemyBase>();

        if (enemyBase != null)
        {
            enemyBase.Initialize(player.transform, mapCenter, this);
        }
    }
    private void SpawnFlyingEnemy()
    {
        Vector2 circle = Random.insideUnitCircle * spawnRadius;

        Vector3 spawnPos = mapCenter.position +
                           new Vector3(circle.x, flyingHeight, circle.y);

        FlyingEnemyModel flyingEnemy = Instantiate(
            flyingEnemyPrefab,
            spawnPos,
            Quaternion.identity);

        flyingEnemy.Initialize(player.transform, mapCenter, this);
    }
    public void ReportPlayerPosition(Vector3 position)
    {
        LastKnownPlayerPos = position;
    }

    public void ClearPlayerPosition()
    {
        LastKnownPlayerPos = null;
    }
}
