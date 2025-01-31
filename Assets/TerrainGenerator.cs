using UnityEngine;
using System.Collections;

public class TerrainGenerator : MonoBehaviour
{
    public int width = 50;
    public int depth = 50;
    public float scale = 5f;
    public float heightMultiplier = 5f;

    private MeshFilter meshFilter;
    private Coroutine terrainCoroutine;

    private float progress = 0f; 

    void Start()
    {
        meshFilter = gameObject.GetComponent<MeshFilter>();
        MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();
        StartTerrainGeneration();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            StartTerrainGeneration();
        }
    }

    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 300, 20), $"Terrain Generation Progress: {progress * 100f:0.0}%");
        GUI.Label(new Rect(10, 30, 300, 20), "Press 'B' to rebuild the terrain");
    }

    void StartTerrainGeneration()
    {
        if (terrainCoroutine != null)
        {
            StopCoroutine(terrainCoroutine);
        }

        terrainCoroutine = StartCoroutine(GenerateTerrain());
    }
    IEnumerator GenerateTerrain()
    {
        float[,] heightMap = GenerateHeightMap();
        Vector3[] vertices = new Vector3[width * depth];
        int[] triangles = new int[(width - 1) * (depth - 1) * 6];

        int vertexIndex = 0;
        int triangleIndex = 0;

        Vector3 startPoint = transform.position;

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                float y = heightMap[x, z] * heightMultiplier;
                vertices[vertexIndex] = startPoint + new Vector3(x, y, z);

                if (x < width - 1 && z < depth - 1)
                {
                    triangles[triangleIndex] = vertexIndex;
                    triangles[triangleIndex + 1] = vertexIndex + width;
                    triangles[triangleIndex + 2] = vertexIndex + width + 1;

                    triangles[triangleIndex + 3] = vertexIndex;
                    triangles[triangleIndex + 4] = vertexIndex + width + 1;
                    triangles[triangleIndex + 5] = vertexIndex + 1;

                    triangleIndex += 6;
                }

                vertexIndex++;
            }

            progress = (float)(x + 1) / width;

            UpdateMesh(vertices, triangles);

            yield return new WaitForSeconds(0.02f);
        }

        UpdateMesh(vertices, triangles);
        progress = 1f; 
    }

    void UpdateMesh(Vector3[] vertices, int[] triangles)
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        meshFilter.mesh = mesh;
    }

    float[,] GenerateHeightMap()
    {
        float[,] heightMap = new float[width, depth];
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                float amplitude = 1;
                float frequency = 1;
                float height = 0;

                for (int i = 0; i < 4; i++)
                {
                    float sampleX = (x + transform.position.x) / scale * frequency;
                    float sampleZ = (z + transform.position.z) / scale * frequency;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleZ);
                    height += perlinValue * amplitude;

                    amplitude *= 0.5f;
                    frequency *= 2f;
                }

                heightMap[x, z] = height;
            }
        }
        return heightMap;
    }
}
