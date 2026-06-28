using System.Collections.Generic;
using UnityEngine;

public class MapPoints : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform mapCenter;
    [SerializeField] private LayerMask walkableMask;

    [Header("Grid Settings")]
    [SerializeField] private float spacing = 2f;
    [SerializeField] private float sampleHeight = 100f;

    [Header("Bounds")]
    private float radius;

    [Header("Nodes")]
    private List<Node> allNodes = new List<Node>();
    public List<Node> AllNodes => allNodes;

    private Dictionary<Vector2Int, Node> grid = new();

    public bool IsReady { get; private set; }

    private SphereCollider sphere;

    private void Awake()
    {
        sphere = mapCenter.GetComponent<SphereCollider>();
    }

    private void Start()
    {
        radius = sphere.radius * mapCenter.lossyScale.x;

        GenerateGrid();
        BuildConnections();

        IsReady = true;

        Debug.Log($"RADIUS: {radius}");
        Debug.Log($"SIZE: {Mathf.CeilToInt((radius * 2f) / spacing)}");
        Debug.Log($"Nodes generated: {allNodes.Count}");
    }
    void GenerateGrid()
    {
        allNodes.Clear();
        grid.Clear();

        Vector3 center = mapCenter.position;

        int size = Mathf.CeilToInt((radius * 2f) / spacing);

        for (int x = -size; x <= size; x++)
        {
            for (int z = -size; z <= size; z++)
            {
                Vector3 worldPos = center + new Vector3(x * spacing, sampleHeight, z * spacing);

                if (!IsInsideBounds(worldPos))
                    continue;

                if (Physics.Raycast(worldPos, Vector3.down, out RaycastHit hit, sampleHeight * 2f, walkableMask))
                {
                    Vector3 finalPos = hit.point;

                    Node node = CreateNode(finalPos);

                    Vector2Int key = new Vector2Int(x, z);

                    grid[key] = node;
                    allNodes.Add(node);
                }
            }
        }
    }

    Node CreateNode(Vector3 pos)
    {
        GameObject go = new GameObject($"Node_{pos.x}_{pos.y}_{pos.z}");
        go.transform.position = pos;
        go.transform.parent = transform;

        Node node = go.AddComponent<Node>();

        node.neighbours = new List<Node>();

        return node;
    }
    void BuildConnections()
    {
        foreach (var kv in grid)
        {
            Vector2Int p = kv.Key;
            Node a = kv.Value;

            TryConnect(p, a, Vector2Int.right);
            TryConnect(p, a, Vector2Int.left);
            TryConnect(p, a, Vector2Int.up);
            TryConnect(p, a, Vector2Int.down);
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
}
