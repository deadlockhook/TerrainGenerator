using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    public int width = 50;       
    public int depth = 50;    
    public float scale = 5f;    
    public int octaves = 4;       
    public float persistence = 0.5f; 
    public float lacunarity = 2f;   
    public float heightMultiplier = 5f; 

    private MeshFilter meshFilter;

    void Start()
    {
        meshFilter = gameObject.GetComponent<MeshFilter>();
        MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();
        GenerateTerrain();
    }

    void GenerateTerrain()
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
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
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

                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = (x + transform.position.x) / scale * frequency;
                    float sampleZ = (z + transform.position.z) / scale * frequency;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleZ);
                    height += perlinValue * amplitude;

                    amplitude *= persistence;
                    frequency *= lacunarity;
                }

                heightMap[x, z] = height;
            }
        }
        return heightMap;
    }

}
