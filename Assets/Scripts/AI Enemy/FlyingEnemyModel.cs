using UnityEngine;

public class FlyingEnemyModel : EnemyBase
{
    [SerializeField] float height = 25;

    public override void Move(Vector3 dir)
    {
        dir.y = 0;

        rb.linearVelocity = dir.normalized * speed;

        Vector3 pos = transform.position;
        pos.y = height;
        transform.position = pos;

        if (dir.sqrMagnitude > 0.01f)
        {
            transform.rotation = Quaternion.LookRotation(dir);
        }
    }
}
