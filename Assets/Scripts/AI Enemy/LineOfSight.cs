using Unity.VisualScripting;
using UnityEngine;

public class LineOfSight : MonoBehaviour
{
    [SerializeField] private float distance;
    [SerializeField] private float horizontalAngle;
    [SerializeField] private float verticalAngle;
    [SerializeField] LayerMask wallMask;
    public bool IsInRange(Transform self, Transform target)
    {
        return Vector3.Distance(self.position, target.position) < distance;
    }
    public bool CheckAngle(Transform self, Transform target)//checkea angulo para los dos lados y adelante
    {
        Vector3 dir = (target.position - self.position).normalized;

        //horizontal
        Vector3 flatForward = new Vector3(self.forward.x, 0, self.forward.z). normalized;
        Vector3 flatDir = new Vector3(dir.x, 0, dir.z).normalized;
        float horAngle = Vector3.Angle(flatForward, flatDir);

        //vertical
        float vertAngle = Mathf.Asin(dir.y) * Mathf.Rad2Deg; //arcoseno + conversion a grados

        return horAngle < horizontalAngle / 2 && Mathf.Abs(vertAngle) < verticalAngle / 2;
    }
    public bool CheckObstacles(Transform self, Transform target)
    {
        Vector3 dir = target.position - self.position;
        return !Physics.Raycast(self.position, dir.normalized, dir.magnitude, wallMask);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Vector3 origin = transform.position;

        int hSteps = 20; //cantidad de lineas a dibujar en el eje
        int vSteps = 10;

        float hStep = horizontalAngle / hSteps;
        float vStep = verticalAngle / vSteps;

        for (int i = 0; i <= hSteps; i++)
        {
            float hAngle = -horizontalAngle / 2 + hStep * i;

            for (int j = 0; j <= vSteps; j++)
            {
                float vAngle = -verticalAngle / 2 + vStep * j;

                Quaternion rot = Quaternion.Euler(vAngle, hAngle, 0);
                Vector3 dir = rot * transform.forward;

                Gizmos.DrawRay(origin, dir * distance);
            }
        }
    }
}
