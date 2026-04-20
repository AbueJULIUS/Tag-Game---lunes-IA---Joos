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
    [SerializeField] private int streetFrequency = 3;//cada cuantos edificios
    [SerializeField] private float streetWidth = 10f;//ancho de calle

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
                float offsetX = x * spacing;
                float offsetZ = z * spacing;

                //calles
                offsetX += (x / streetFrequency) * streetWidth;
                offsetZ += (z / streetFrequency) * streetWidth;

                Vector3 position = new Vector3(offsetX, 0, offsetZ);

                GameObject prefab = buildings[Random.Range(0, buildings.Count)];

                GameObject obj = Instantiate(prefab, position, prefab.transform.rotation, transform);
                obj.transform.localScale = prefab.transform.localScale;
            }
        }
    }
}
