using UnityEngine;

public class EnemyModel : EnemyBase
{
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
}
