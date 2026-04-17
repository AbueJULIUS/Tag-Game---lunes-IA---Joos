using UnityEngine;

public class SteeringBehaviours
{
    public static Vector3 Seek(Transform self, Vector3 target)
    {
        Vector3 dir = target - self.position;
        dir.y = 0;
        return dir.normalized;
    }
    public static Vector3 Flee(Transform self, Vector3 target)
    {
        Vector3 dir = self.position - target;
        dir.y = 0;
        return dir.normalized;
    }
    public static Vector3 Arrive(Transform self, Vector3 target, float slowSpeed)
    {
        Vector3 dir = target -self.position;
        float dist = dir.magnitude;
        if (dist < 0.01f)
        {
            return Vector3.zero;
        }
        float speedFactor = Mathf.Clamp01(dist / slowSpeed);
        return dir.normalized * speedFactor;
    }
    private static Vector3 CalculateFuturePos(Transform self, Transform target, Rigidbody targetRb, float maxPredTime)
    {
        Vector3 targetVelocity = targetRb.linearVelocity;

        Vector3 toTarget = target.position - self.position;
        toTarget.y = 0;

        float dist = toTarget.magnitude;
        float predictionTime = Mathf.Clamp(dist / 5f, 0f, maxPredTime);

        return target.position + targetVelocity * predictionTime;
    }
    public static Vector3 Pursue(Transform self, Transform target, Rigidbody rb, float maxPredTime)
    {
        Vector3 futurePos = CalculateFuturePos(self, target, rb, maxPredTime);
        return Arrive(self, futurePos, 2.5f);
    }
    public static Vector3 Evade(Transform self, Transform target, Rigidbody rb, float maxPredTime)
    {
        Vector3 futurePos = CalculateFuturePos(self, target, rb, maxPredTime);
        return Flee(self, futurePos);
    }
    public static Vector3 Wander(Vector3 currentDir, float maxAngleChange)
    {
        float randomAngle = Random.Range(-maxAngleChange, maxAngleChange);
        Quaternion rotation = Quaternion.Euler(0f, randomAngle, 0f);
        Vector3 newDir = rotation * currentDir;
        return newDir.normalized;
    }
}
