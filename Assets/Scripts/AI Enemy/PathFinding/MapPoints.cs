using System.Collections.Generic;
using UnityEngine;

public class MapPoints : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform mapCenter;
    [SerializeField] private LayerMask walkableMask;
    [SerializeField] private LayerMask nodeMask;
    [SerializeField] private LayerMask obstacleMask;

    [Header("Grid Settings")]
    [SerializeField] private GameObject nodePrefab;
    [SerializeField] private Vector2Int gridSize = new Vector2Int(50, 50);
    [SerializeField] private float spacing = 4f;
    [SerializeField] private float sampleHeight = 100f;

    [Header("Bounds")]
    private float radius;

    [Header("Nodes")]
    private List<Node> allNodes = new List<Node>();
    public List<Node> AllNodes => allNodes;

    private Dictionary<Vector2Int, Node> grid = new();

    public bool IsReady { get; private set; }

    private SphereCollider mapBorder;

    public static MapPoints Instance;

    private void Awake()
    {
        Instance = this;
        mapBorder = GetComponent<SphereCollider>();
    }

    private void Start()
    {
        radius = mapBorder.radius * mapBorder.transform.lossyScale.x;

        GenerateGrid();
        BuildConnections();
    }
    void GenerateGrid()
    {
        allNodes.Clear();

        Vector3 center = mapCenter.position;

        for (int x = -gridSize.x; x <= gridSize.x; x++)
        {
            for (int z = -gridSize.y; z <= gridSize.y; z++)
            {
                Vector3 worldPos =
    center +
    new Vector3(x * spacing, sampleHeight, z * spacing);

                if (Physics.Raycast(worldPos, Vector3.down, out RaycastHit hit, sampleHeight * 2f, walkableMask))
                {
                    Vector3 finalPos = hit.point;

                    //FILTRO ESFERA DSPS DEL SNAP
                    if (Vector3.Distance(finalPos, center) > radius)
                        continue;

                    if (((1 << hit.collider.gameObject.layer) & obstacleMask) != 0)
                        continue;

                    GameObject go = Instantiate(nodePrefab, finalPos, Quaternion.identity, transform);

                    Node node = go.GetComponent<Node>();
                    node.neighbours = new List<Node>();

                    allNodes.Add(node);
                }
            }
        }
    }
    void BuildConnections()
    {
        float connectionRadius = spacing * 1.1f;

        foreach (Node node in allNodes)
        {
            node.neighbours.Clear();

            Collider[] hits = Physics.OverlapSphere(
                node.transform.position,
                connectionRadius,
                nodeMask
            );

            foreach (Collider c in hits)
            {
                Node other = c.GetComponent<Node>();

                if (other == null || other == node)
                    continue;

                if (IsWalkableConnection(node, other))
                {
                    node.neighbours.Add(other);
                }
            }
        }
    }

    void TryConnect(Vector2Int p, Node a, Vector2Int dir)
    {
        Vector2Int n = p + dir;

        if (grid.TryGetValue(n, out Node b))
        {
            a.neighbours.Add(b);
        }
    }

    bool IsInsideBounds(Vector3 point)
    {
        Vector2 p = new Vector2(point.x, point.z);
        Vector2 c = new Vector2(mapCenter.position.x, mapCenter.position.z);

        return (p - c).sqrMagnitude <= radius * radius;
    }
    public bool HasLineOfSight(Node a, Node b) 
    {
        Vector3 dir = (b.transform.position - a.transform.position);
        float dist = dir.magnitude;

        Vector3 stepDir = dir.normalized;

        float stepSize = 0.5f; // menor que spacing
        int steps = Mathf.CeilToInt(dist / stepSize);

        Vector3 current = a.transform.position;

        for (int i = 0; i < steps; i++)
        {
            current += stepDir * stepSize;

            if (IsBlockedByGraph(current))
                return false;
        }

        return true;
    }
    bool IsBlockedByGraph(Vector3 point)
    {
        foreach (var node in allNodes)
        {
            if (Vector3.Distance(node.transform.position, point) < spacing * 0.5f)
                return false; //camino valido cerca
        }

        return true; //vacio => bloqueado
    }
    bool IsWalkableConnection(Node a, Node b)
    {
        Vector3 from = a.transform.position + Vector3.up * 0.2f;
        Vector3 to = b.transform.position + Vector3.up * 0.2f;

        Vector3 dir = to - from;
        float dist = dir.magnitude;

        return !Physics.Raycast(from, dir.normalized, dist, obstacleMask);
    }
    public Node GetClosestNode(Vector3 pos)
    {
        Node best = null;
        float bestDist = float.MaxValue;

        foreach (var n in allNodes)
        {
            float d = (n.transform.position - pos).sqrMagnitude;

            if (d < bestDist)
            {
                bestDist = d;
                best = n;
            }
        }

        return best;
    }
}
