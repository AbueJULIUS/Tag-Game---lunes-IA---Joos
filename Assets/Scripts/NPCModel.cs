using UnityEngine;

public class NPCModel : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float speed = 8f;
    [SerializeField] float rotVelocity = 10f;

    [Header("Wander")]
    [SerializeField] float wanderTimer = 2f;
    [SerializeField] float wanderAngle = 90f;
    private float mapRadius;
    private Transform mapCenter;

    Rigidbody rb;
    LineOfSight los;

    Vector3 wanderDir;
    float timer;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        los = GetComponent<LineOfSight>();

        wanderDir = transform.forward;
        timer = wanderTimer;
        
    }
    private void Start()
    {
        mapCenter = EnemyManager.Instance.MapCenter;
        SphereCollider sphere = mapCenter.GetComponent<SphereCollider>();
        mapRadius = sphere.radius * mapCenter.lossyScale.x;
    }

    void Update()//no hice arbol ni fsm porque son solo dos comportamientos //y sin canSee multiple porque son muchos enemigos
    {
        Vector3 dir;

        if (los.TryGetVisibleEnemy(out var enemy) && enemy != null)
        {   
            dir = SteeringBehaviours.Evade(transform, enemy.transform, enemy.Rb, 2f);                  
        }
        else
        {
            timer -= Time.deltaTime;

            if (timer <= 0f)
            {
                wanderDir = SteeringBehaviours.Wander(wanderDir, wanderAngle);
                timer = wanderTimer;
            }

            dir = wanderDir;
        }

        Move(dir);
    }
    protected virtual void LateUpdate()
    {
        LimitMovement();
    }
    void Move(Vector3 dir)
    {
        Vector3 vel = rb.linearVelocity;
        Vector3 horizontal = dir * speed;

        rb.linearVelocity = new Vector3(horizontal.x, vel.y, horizontal.z);

        if (dir.sqrMagnitude > 0.01f)
            transform.forward = dir;
    }
    void Rotate(Vector3 dir)
    {
        if (dir.sqrMagnitude > 0.01f)
        {
            transform.forward = Vector3.Lerp(
                transform.forward,
                dir,
                rotVelocity * Time.deltaTime);
        }
    }
    protected void LimitMovement()
    {
        Vector3 offset = transform.position - mapCenter.position;

        //solo limite en XZ
        offset.y = 0;

        if (offset.magnitude > mapRadius)
        {
            Vector3 clampedPos = mapCenter.position + offset.normalized * mapRadius;

            transform.position = new Vector3(
                clampedPos.x,
                transform.position.y,
                clampedPos.z
            );
        }
    }
}
