using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public List<Node> neighbours = new List<Node>();

    public int layer;
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawSphere(transform.position, 0.5f);
    }
}
