using UnityEngine;

public abstract class EnemyBase : MonoBehaviour, ITagable
{
    [SerializeField] protected float speed;
    [SerializeField] protected float rotVelocity = 15;

    protected Rigidbody rb;
    protected LineOfSight los;
    protected Transform player;
    protected Transform mapCenter;
    protected EnemyManager manager;
    protected FSMClasses fsm;

    protected float mapRadius;

    protected bool tagged;
    public bool Tagged => tagged;
    public Rigidbody Rb => rb;
    public Transform PlayerTransform => player;
    public EnemyManager Manager => manager;
    public Transform MapCenter => mapCenter;
    [SerializeField]
    protected float wanderTimer = 1f;
    public float WanderTimer => wanderTimer;

    public virtual void Initialize(Transform player, Transform mapCenter, EnemyManager manager)
    {
        this.player = player;
        this.mapCenter = mapCenter;
        this.manager = manager;

        SphereCollider sphere = mapCenter.GetComponent<SphereCollider>();
        mapRadius = sphere.radius * mapCenter.lossyScale.x;
    }
    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        los = GetComponent<LineOfSight>();
        fsm = GetComponent<FSMClasses>();
    }
    public abstract void Move(Vector3 dir);

    private void Update()
    {
        bool canSeePlayer =
            los.IsInRange(transform, player) &&
            los.CheckAngle(transform, player) &&
            los.CheckObstacles(transform, player);

        fsm.UpdateState(canSeePlayer);
    }
    protected virtual void LateUpdate()
    {
        LimitMovement();
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
    public virtual void ToggleTagged(bool state)
    {
        tagged = state;
    }
}
