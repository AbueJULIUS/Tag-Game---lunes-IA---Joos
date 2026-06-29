using Unity.VisualScripting;
using UnityEngine;

public class LineOfSight : MonoBehaviour
{
    [SerializeField] private float distance;
    [SerializeField] private float horizontalAngle;
    [SerializeField] private float verticalAngle;
    [SerializeField] LayerMask wallMask;

    [SerializeField] private Vector3 viewDirection = Vector3.forward;
    public bool IsInRange(Transform self, Transform target)
    {
        return Vector3.Distance(self.position, target.position) < distance;
    }
    public bool CheckAngle(Transform self, Transform target)//checkea angulo para los dos lados y adelante
    {
        Vector3 dir = (target.position - self.position).normalized;
        Vector3 forward = self.TransformDirection(viewDirection);

        //horizontal
        Vector3 flatForward = new Vector3(forward.x, 0, forward.z). normalized;
        Vector3 flatDir = new Vector3(dir.x, 0, dir.z).normalized;

        float horAngle = Vector3.Angle(flatForward, flatDir);

        //vertical
        float vertAngle = Mathf.Asin(Vector3.Dot(dir, Vector3.up)) * Mathf.Rad2Deg; //arcoseno + conversion a grados

        return horAngle < horizontalAngle / 2 && Mathf.Abs(vertAngle) < verticalAngle / 2;
    }
    public bool CheckObstacles(Transform self, Transform target)
    {
        Vector3 dir = target.position - self.position;
        return !Physics.Raycast(self.position, dir.normalized, dir.magnitude, wallMask);
    }
    public bool TryGetVisibleEnemy(out EnemyBase enemy)//checkear si enemigo enfrente
    {
        enemy = null;

        Vector3 origin = transform.position;
        Vector3 baseDir = transform.TransformDirection(viewDirection).normalized;

        Quaternion basis = Quaternion.LookRotation(baseDir);

        int hSteps = 20;
        int vSteps = 10;

        float hStep = horizontalAngle / hSteps;
        float vStep = verticalAngle / vSteps;

        for (int i = 0; i <= hSteps; i++)
        {
            float hAngle = -horizontalAngle * 0.5f + hStep * i;

            for (int j = 0; j <= vSteps; j++)
            {
                float vAngle = -verticalAngle * 0.5f + vStep * j;

                Quaternion rot =
                    basis *
                    Quaternion.Euler(vAngle, hAngle, 0);

                Vector3 dir = rot * Vector3.forward;

                if (Physics.Raycast(origin, dir, out RaycastHit hit, distance))
                {
                    if (hit.collider.CompareTag("Tagable"))
                    {
                        enemy = hit.collider.GetComponent<EnemyBase>();
                        return true;
                    }
                }
            }
        }

        return false;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Vector3 origin = transform.position;

        //dir =  a checkangle
        Vector3 baseDir = transform.TransformDirection(viewDirection).normalized;

        //rot alineada a dir
        Quaternion basis = Quaternion.LookRotation(baseDir);

        int hSteps = 20;
        int vSteps = 10;

        float hStep = horizontalAngle / hSteps;
        float vStep = verticalAngle / vSteps;

        for (int i = 0; i <= hSteps; i++)
        {
            float hAngle = -horizontalAngle * 0.5f + hStep * i;

            for (int j = 0; j <= vSteps; j++)
            {
                float vAngle = -verticalAngle * 0.5f + vStep * j;

                Quaternion rot =
                    basis *
                    Quaternion.Euler(vAngle, hAngle, 0);

                Vector3 dir = rot * Vector3.forward;

                Gizmos.DrawRay(origin, dir * distance);
            }
        }
    }
}
