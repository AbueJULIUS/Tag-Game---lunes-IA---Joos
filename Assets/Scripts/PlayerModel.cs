using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerModel : MonoBehaviour, ITagable
{
    private Rigidbody rb;
    private CapsuleCollider coll;
    private PlayerInput playerInput;
    private PlayerView playerView;
    //private HealthController healthController;

    Vector2 moveInput;
    Vector2 aimInput;

    [Header("Jump Parametres")]
    [SerializeField] private float jumpForce = 15f;
    private float groundCheckRadius = 0.3f;
    [SerializeField] private float speed = 10f;
    public float Speed { get => speed; }
    private float currentSpeed;
    public float CurrentSpeed { get => currentSpeed; set => currentSpeed = value; }

    private bool tagged;
    public bool Tagged => tagged;

    private float rotVelocity = 10;
    [Header("Wallclimbing Parametres")]
    [SerializeField] float offWallJumpForce = 0.4f;
    [SerializeField] float wallCheckDistance = 0.5f;
    [SerializeField] float capsuleHeight = 1.5f;
    [SerializeField] float capsuleRadius = 0.3f;
    [SerializeField] LayerMask wallMask;

    [Header("Camera Settings")]
    [SerializeField] private Transform cameraPivot;
    [SerializeField] private Transform cam;
    [Range(50f, 200f)][SerializeField] private float mouseSensitivity = 100f;
    [SerializeField] private float cameraDistance = 5f;
    [SerializeField] private float minCameraDistance = 1f;
    [SerializeField] private float smoothCameraSpeed = 10f;

    float xRotation = 0f;
    float yRotation = 0f;

    [Header("MapBounds")]
    [SerializeField] private Transform mapCenter;
    private float mapRadius;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        CurrentSpeed = speed;

        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        coll = GetComponent<CapsuleCollider>();
        playerView = GetComponent<PlayerView>();
        SphereCollider sphere = mapCenter.GetComponent<SphereCollider>();
        mapRadius = sphere.radius * mapCenter.lossyScale.x;
    }

    private void Start()
    {
        ToggleCursor(false);
    }
    // Update is called once per frame
    void Update()
    {               

        moveInput = playerInput.actions["Move"].ReadValue<Vector2>();

        aimInput = playerInput.actions["Aim"].ReadValue<Vector2>();

        float mouseX = aimInput.x * mouseSensitivity * Time.deltaTime;
        float mouseY = aimInput.y * mouseSensitivity * Time.deltaTime;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);
        
        transform.rotation = Quaternion.Euler(0f, yRotation, 0f);
        cameraPivot.localRotation = Quaternion.Euler(xRotation, 0f, 0f);


    }
    private void LateUpdate()
    {
        HandleCameraCollision();
        LimitMovement();
    }
    void FixedUpdate()
    {
        Move();
    }
    void Move()
    {
        Vector3 forward = cameraPivot.forward;
        Vector3 right = cameraPivot.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        Vector3 direction = forward * moveInput.y + right * moveInput.x;

        Vector3 velocity = rb.linearVelocity;
        Vector3 horizontalVelocity = direction * currentSpeed;
        rb.linearVelocity = new Vector3(horizontalVelocity.x, velocity.y, horizontalVelocity.z);

        Rotate(direction);
    }
    void LimitMovement()
    {
         
        Vector3 offset = transform.position - mapCenter.position;

        if (offset.magnitude > mapRadius)
        {
            transform.position = mapCenter.position + offset.normalized * mapRadius;
        }
    
    }
    void Rotate(Vector3 dir)
    {
        if (dir.magnitude > 0.1f)
        {
            Vector3 targetDir = dir.normalized;
            transform.forward = Vector3.Lerp(transform.forward, targetDir, rotVelocity * Time.deltaTime);
        }
    }
    public void ToggleCursor(bool state)
    {
        if (state)
        {
            Cursor.lockState = CursorLockMode.None;           
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        Cursor.visible = state;

    }
    void HandleCameraCollision()//raycast hacia atras q si hay pared redce distancia de camara
    {
        Vector3 desiredPosition = cameraPivot.position - cameraPivot.forward * cameraDistance;

        RaycastHit hit;

        float targetDistance = cameraDistance;

        if (Physics.Raycast(cameraPivot.position, -cameraPivot.forward, out hit, cameraDistance))
        {
            targetDistance = Mathf.Clamp(hit.distance, minCameraDistance, cameraDistance);
        }

        Vector3 finalPosition = cameraPivot.position - cameraPivot.forward * targetDistance;

        cam.position = Vector3.Lerp(cam.position, finalPosition, smoothCameraSpeed * Time.deltaTime);
    }
    bool IsGrounded()//si se apreta muy rapido se salta super alto
    {
        //hacer esfera debajo del jugador (no raycast por si se cae un poquito)
        Vector3 bottom = transform.position + coll.center - Vector3.up * (coll.height / 2 + groundCheckRadius + 0.02f);
        return Physics.CheckSphere(bottom, 0.3f);
    }
    bool IsTouchingWall()
    {
        Vector3 center = transform.position + Vector3.up * (capsuleHeight / 2);

        Vector3 forwardOffset = transform.forward * wallCheckDistance;

        Vector3 point1 = center + forwardOffset + Vector3.up * (capsuleHeight / 2 - capsuleRadius);
        Vector3 point2 = center + forwardOffset - Vector3.up * (capsuleHeight / 2 - capsuleRadius);

        return Physics.CheckCapsule(point1, point2, capsuleRadius, wallMask);
    }

    public void Jump(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.performed) //para q no se ejecute 3 veces - started y canceled
        {
            if (IsGrounded())
            {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }
            else if (IsTouchingWall())
            {
                rb.linearVelocity = Vector3.zero;
                Vector3 jumpDir = (Vector3.up - transform.forward * offWallJumpForce).normalized;
                rb.AddForce(jumpDir * jumpForce, ForceMode.Impulse);
            }
            
            playerView.Jump();
        }
    }

    public void ToggleTagged(bool state)
    {
        tagged = state;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Tagable"))
        {
            GameManager.Instance.FinishGame();
        }
    }

}
