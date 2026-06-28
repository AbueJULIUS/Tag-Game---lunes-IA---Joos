using UnityEngine;

public class EnemyModel : EnemyBase
{

    protected override void Awake()
    {
        base.Awake();

        fsm = GetComponent<FSMClasses>();
    }   

    private void LateUpdate()
    {
        LimitMovement();
    }

    public override void Move(Vector3 dir)
    {
        Vector3 velocity = rb.linearVelocity;
        Vector3 horizontalVelocity = dir * speed;

        rb.linearVelocity = new Vector3(
            horizontalVelocity.x,
            velocity.y,
            horizontalVelocity.z);

        Rotate(dir);
    }

    private void Rotate(Vector3 dir)
    {
        if (dir.sqrMagnitude > 0.01f)
        {
            transform.forward = Vector3.Lerp(
                transform.forward,
                dir,
                rotVelocity * Time.deltaTime);
        }
    }

    private void LimitMovement()
    {
        Vector3 offset = transform.position - mapCenter.position;

        if (offset.magnitude > mapRadius)
        {
            transform.position =
                mapCenter.position +
                offset.normalized * mapRadius;
        }
    }
}
