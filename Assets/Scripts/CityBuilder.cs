using System.Collections.Generic;
using UnityEngine;

public class CityBuilder : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private List<GameObject> buildings;

    [Header("Grid")]
    [SerializeField] private int width = 5;
    [SerializeField] private int height = 5;
    [SerializeField] private float spacing = 10f;

    [Header("Settings")]
    [SerializeField] private bool randomRotation = true;

    void Start()
    {
        GenerateCity();
    }

    void GenerateCity()
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Vector3 position = new Vector3(
                    x * spacing,
                    0,
                    z * spacing
                );

                GameObject prefab = buildings[Random.Range(0, buildings.Count)];

                Quaternion rotation = Quaternion.identity;

                if (randomRotation)
                {
                    rotation = Quaternion.Euler(0, Random.Range(0, 4) * 90f, 0);
                }

                Instantiate(prefab, position, rotation, transform);
            }
        }
    }
}
