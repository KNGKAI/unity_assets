using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Terrain
{
    private static float frequency = 0.1f;

    public static float Frequency
    {
        get
        {
            return (frequency);
        }
    }

    public static void SetFrequency(float f)
    {
        frequency = f;
    }
    
    private static float amplitude = 2.0f;

    public static float Amplitude
    {
        get
        {
            return (amplitude);
        }
    }

    public static void SetAmplitude(float a)
    {
        amplitude = a;
    }

    public static float GetValue(float x, float y)
    {
        return (Mathf.PerlinNoise(x * frequency, y * frequency));
    }

    public static float GetHeight(float x, float y)
    {
        return (GetValue(x, y) * amplitude);
    }

    public class Chunk : MonoBehaviour
    {
        public static readonly int ChunkSize = 16;

        private static Texture2D defaultTexture;

        public static Texture2D DefaultTexture
        {
            get
            {
                if (defaultTexture == null)
                {
                    defaultTexture = new Texture2D(2, 2) { filterMode = FilterMode.Point };
                    defaultTexture.SetPixel(0, 0, Color.white);
                    defaultTexture.SetPixel(1, 1, Color.white);
                    defaultTexture.SetPixel(0, 1, Color.gray);
                    defaultTexture.SetPixel(1, 0, Color.gray);
                    defaultTexture.Apply();
                }
                return (defaultTexture);
            }
        }

        private static int lod = 1;

        public static void SetLOD(int l)
        {
            lod = Mathf.Clamp(l, 1, ChunkSize);
        }

        private static List<Chunk> chunks;

        public static List<Chunk> Chunks
        {
            get
            {
                if (chunks == null)
                {
                    chunks = new List<Chunk>();
                    chunks.AddRange(FindObjectsOfType<Chunk>());
                }
                return (chunks);
            }
        }

        public static int Snap(int x)
        {
            return (x - x % ChunkSize);
        }

        public static bool Contains(int x, int y)
        {
            for (int i = 0; i < Chunks.Count; i++)
            {
                if (Chunks[i].Position.x == x && Chunks[i].Position.y == y)
                {
                    return (true);
                }
            }
            return (false);
        }

        public static void DestroyAll()
        {
            for (int i = 0; i < Chunks.Count; i++)
            {
                Destroy(Chunks[i].gameObject);
            }
            Destroy(ChunkParent.gameObject);
            Chunks.Clear();
        }

        public static List<Vector3> GetVerticies(int x, int y)
        {
            List<Vector3> verticies;
            Vector3 position;
            int s;

            position = Vector3.zero;
            s = ChunkSize / lod;

            verticies = new List<Vector3>();

            for (int a = 0; a <= s; a++)
            {
                for (int b = 0; b <= s; b++)
                {
                    position.x = x + (a == s ? ChunkSize : a * lod);
                    position.z = y + (b == s ? ChunkSize : b * lod);
                    position.y = GetHeight(position.x, position.z);

                    verticies.Add(position);
                }
            }
            return (verticies);
        }

        private static List<int> triangles;

        public static List<int> Triangles
        {
            get
            {
                if (triangles == null)
                {
                    int i;
                    int s;

                    triangles = new List<int>();

                    s = ChunkSize / lod;

                    for (int a = 0; a < s; a++)
                    {
                        for (int b = 0; b < s; b++)
                        {
                            i = a + b * (s + 1);

                            triangles.Add(i);
                            triangles.Add(i + 1);
                            triangles.Add(i + 2 + s);

                            triangles.Add(i);
                            triangles.Add(i + 2 + s);
                            triangles.Add(i + 1 + s);
                        }
                    }
                }

                return (triangles);
            }
        }

        private static List<Vector2> uvs;

        public static List<Vector2> UVs
        {
            get
            {
                if (uvs == null)
                {
                    int s;

                    uvs = new List<Vector2>();
                    
                    s = ChunkSize / lod;

                    for (int a = 0; a <= s; a++)
                    {
                        for (int b = 0; b <= s; b++)
                        {
                            UVs.Add(new Vector2(a, b));
                        }
                    }
                }

                return (uvs);
            }
        }

        public static Mesh GetMesh(int x, int y)
        {
            Mesh m;

            m = new Mesh();
            m.SetVertices(GetVerticies(x, y));
            m.SetTriangles(Triangles, 0);
            m.SetUVs(0, UVs);
            m.RecalculateNormals();
            m.Optimize();
            return (m);
        }

        private static GameObject chunkParent;

        public static Transform ChunkParent
        {
            get
            {
                if (chunkParent == null)
                {
                    chunkParent = GameObject.Find("chunks");
                    if (chunkParent == null)
                    {
                        chunkParent = new GameObject("chunks");
                    }
                }
                return (chunkParent.transform);
            }
        }

        public static void Update(float x, float y, int radius = 1)
        {
            Vector2Int position;
            int a;
            int b;

            position = Vector2Int.zero;
            a = Snap((int)x);
            b = Snap((int)y);

            for (int i = -radius; i <= radius; i++)
            {
                for (int j = -radius; j <= radius; j++)
                {
                    position.x = a + i * ChunkSize;
                    position.y = b + j * ChunkSize;

                    if (!Contains(position.x, position.y))
                    {
                        AddChunk(position.x, position.y);
                    }
                }
            }
        }

        public static void AddChunk(int x, int y)
        {
            Chunk chunk;

            chunk = new GameObject("chunk").AddComponent<Chunk>();
            chunk.Init(x, y);
        }

        private Vector2Int position;

        public Vector2Int Position
        {
            get
            {
                return (position);
            }
        }

        private MeshFilter filter;

        private MeshRenderer render;

        public void Init(int x, int y)
        {
            position = new Vector2Int(x, y);

            filter = gameObject.AddComponent<MeshFilter>();
            filter.mesh = GetMesh(x, y);

            render = gameObject.AddComponent<MeshRenderer>();
            render.material = new Material(Shader.Find("Standard")) { mainTexture = DefaultTexture };

            gameObject.AddComponent<MeshCollider>();

            transform.SetParent(ChunkParent);

            Chunks.Add(this);
        }

        public void Destroy()
        {
            Destroy(gameObject);
        }
    }
}
