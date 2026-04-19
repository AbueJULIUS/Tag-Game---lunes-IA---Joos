using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class EnemyModel : MonoBehaviour, ITagable
{
    [SerializeField] private bool tagged;
    public bool Tagged => tagged;
    [SerializeField] private float speed;
    private float rotVelocity = 15;
    private Vector3 wanderDirection;
    [Range(1f, 3f)][SerializeField] private float wanderTimer = 1f;
    public float WanderTimer => wanderTimer;

    private Rigidbody rb;
    public Rigidbody Rb => rb;
    private LineOfSight los;
    private Transform player;
    public Transform PlayerTransform => player;
    private bool canSeePlayer;

    FSMClasses fsm;
    [Header("MapBounds")]
    private Transform mapCenter;
    private float mapRadius;
    private void Awake()
    {
        los = GetComponent<LineOfSight>();
        fsm = GetComponent<FSMClasses>();
        wanderDirection = transform.forward;
       
    }
    private void Start()
    {
        player = FindAnyObjectByType<PlayerModel>().transform;
        rb = GetComponent<Rigidbody>();
        mapCenter = GameObject.FindGameObjectWithTag("Border").transform;
        SphereCollider sphere = mapCenter.GetComponent<SphereCollider>();
        mapRadius = sphere.radius * mapCenter.lossyScale.x;
    }

    // Update is called once per frame
    void Update()
    {        

        canSeePlayer = los.IsInRange(gameObject.transform, player) &&
            los.CheckAngle(gameObject.transform, player) &&
            los.CheckObstacles(gameObject.transform, player);

        fsm.UpdateState(canSeePlayer);
                
    }
    private void LateUpdate()
    {
        LimitMovement();
    }
    public void Move(Vector3 dir)
    {
        Vector3 velocity = rb.linearVelocity;
        Vector3 horizontalVelocity = dir * speed;
        rb.linearVelocity = new Vector3(horizontalVelocity.x, velocity.y, horizontalVelocity.z);
        Rotate(dir);
    }
    void Rotate(Vector3 dir)
    {
        if (dir.sqrMagnitude > 0.01f)
        {
            transform.forward = Vector3.Lerp(transform.forward, dir, rotVelocity * Time.deltaTime);
        }
    }
    void LimitMovement()
    {

        Vector3 offset = transform.position - mapCenter.position;

        if (offset.magnitude > mapRadius)
        {
            transform.position = mapCenter.position + offset.normalized * mapRadius;
        }

    }
    public void ToggleTagged(bool state)
    {
        tagged = state;
    }
}
