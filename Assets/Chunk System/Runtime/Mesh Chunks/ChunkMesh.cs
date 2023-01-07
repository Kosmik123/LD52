using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using Bipolar.ChunkSystem.Generation;

namespace Bipolar.ChunkSystem.MeshChunks
{
    public class ChunkMesh : MonoBehaviour
    {
        public static event System.Action<ChunkMesh> OnMeshCreated;

        public bool refreshOnGizmos;

        [Header("To Link")]
        [SerializeField]
        private Chunk chunk;
        public Chunk Chunk => chunk;

        [SerializeField]
        private MeshFilter meshFilter;
        public MeshFilter MeshFilter => meshFilter;

        [SerializeField]
        private MeshRenderer meshRenderer;

        [SerializeField]
        private MeshCollider meshCollider;
        public MeshCollider MeshCollider => meshCollider;

        [Header("Settings")]
        [SerializeField]
        private int resolution;
        [SerializeField]
        private Material material;

        [SerializeField]
        private MapMeshGenerator meshGenerator;

        [Header("Noise")]
        [SerializeField]
        private Object heightMapProvider;
        public IMapProvider Algorithm
        {
            get => heightMapProvider as IMapProvider;
            set
            {
                heightMapProvider = value as Object;
            }
        }

        [SerializeField]
        private ChunkMeshSmoothing smoothing;



        private Queue<MapThreadInfo> meshDataActionsQueue = new Queue<MapThreadInfo>();

        private void Reset()
        {
            if (chunk == null)
                TryGetComponent(out chunk);
            if (chunk != null && chunk.Content != null)
            {
                meshFilter = GetOrAddComponent<MeshFilter>(chunk.Content);
                meshRenderer = GetOrAddComponent<MeshRenderer>(chunk.Content);
                meshCollider = GetOrAddComponent<MeshCollider>(chunk.Content);
            }
        }

        public T GetOrAddComponent<T>(Component container) where T : Component
        {
            if (container.TryGetComponent(out T component) == false)
                component = container.gameObject.AddComponent<T>();
            
            return component;
        }

        private void Awake()
        {
            if (chunk == null)
                chunk = GetComponent<Chunk>();
            if (meshFilter == null)
                meshFilter = GetComponent<MeshFilter>();
            if (meshRenderer == null)
                meshRenderer = GetComponent<MeshRenderer>();
            if (meshCollider == null)
                meshCollider = GetComponent<MeshCollider>();

            chunk.OnChunkInitialized += GenerateMesh;
        }

        private void Update()
        {
            if (meshDataActionsQueue.Count > 0)
            {
                var mapThreadInfo = meshDataActionsQueue.Dequeue();
                mapThreadInfo.action.Invoke(mapThreadInfo.data);
                OnMeshCreated?.Invoke(this);
            }
        }


        [ContextMenu("Generate Mesh")]
        private void TestGenerateMesh()
        {
            Vector2 meshSize = chunk.Settings != null ? new Vector2(chunk.Settings.ChunkSize.x, chunk.Settings.ChunkSize.z) : new Vector2(40, 40);
            var meshData = GetMeshData(meshSize.x, meshSize.y, resolution + 2, resolution + 2);
            CreateMesh(meshData);
        }

        private void GenerateMesh()
        {
            RequestMeshData(CreateMesh);
        }

        private void RequestMeshData(System.Action<MapMeshData> actionAfterGeneration)
        {
            void threadStart() { MeshDataThread(actionAfterGeneration); }
            new Thread(threadStart).Start();
            //MeshDataThread(actionAfterGeneration); // not threaded option
        }

        public struct MapThreadInfo
        {
            public readonly MapMeshData data;
            public readonly System.Action<MapMeshData> action;

            public MapThreadInfo(MapMeshData data, System.Action<MapMeshData> action)
            {
                this.data = data;
                this.action = action;
            }
        }

        private void MeshDataThread(System.Action<MapMeshData> actionAfterGeneration)
        {
            Vector2 meshSize = chunk.Settings != null ? new Vector2(chunk.Settings.ChunkSize.x, chunk.Settings.ChunkSize.z) : new Vector2(40, 40);
            var meshData = GetMeshData(meshSize.x, meshSize.y, resolution + 2, resolution + 2);
            var mapThreadInfo = new MapThreadInfo(meshData, actionAfterGeneration);
            meshDataActionsQueue.Enqueue(mapThreadInfo);
        }

        private MapMeshData GetMeshData(float width, float height, int xVertCount, int zVertCount)
        {
            var index = chunk.Index;
            float xDelta = width / (xVertCount - 3);
            float zDelta = height / (zVertCount - 3);

            var noiseMap = Algorithm.GetMap(xVertCount, zVertCount,
                width + 2 * xDelta, height + 2 * zDelta, index.x * width, index.z * height);
            return meshGenerator.GenerateMeshData(width, height, noiseMap);
        }


        private void CreateMesh(MapMeshData meshData)
        {
            int textureResolution = 4; // meshData.xVertCount - 1;
                                       // meshData.yVertCount - 1;
            if (material == null)
            {
                Texture2D texture = CreateTexture(textureResolution, textureResolution, meshData.uvs);
                material = meshRenderer.material ?? meshRenderer.sharedMaterial;
                material.SetFloat("_Smoothness", 0);
                material.mainTexture = texture;
            }
            meshRenderer.material = material;

            var mesh = meshData.ToMesh();
            meshFilter.mesh = mesh;
        }

        static Texture2D texture;
        private static Texture2D CreateTexture(int xResolution, int yResolution, Vector2[] map = null)
        {
            if (ChunkMesh.texture != null)
                return ChunkMesh.texture;

            float xDelta = 1f / xResolution;
            float yDelta = 1f / yResolution;

            Color[] colorMap = new Color[xResolution * yResolution];
            Texture2D texture = new Texture2D(xResolution, yResolution, TextureFormat.RGB24, true);
            int idx = 0;
            for (int j = 0; j < xResolution; j++)
            {
                for (int i = 0; i < yResolution; i++)
                {
                    //float blue = Mathf.Max(1 - uv.x - uv.y, 0);
                    colorMap[idx] = new Color(xDelta * (i + 0.5f), yDelta * (j + 0.5f), 0);
                    //if (map != null)
                    //colorMap[idx] = Color.Lerp(Color.red, Color.green, map[i, j]);
                    idx++;
                }
            }
            texture.SetPixels(colorMap);
            texture.wrapMode = TextureWrapMode.Repeat;
            texture.filterMode = FilterMode.Point;
            texture.Apply();

            return texture;
        }

        private static Texture2D CreateTexture(int xResolution, int yResolution, Vector3[] normalsToDraw)
        {
            float xDelta = 1f / xResolution;
            float yDelta = 1f / yResolution;

            Color[] colorMap = new Color[xResolution * yResolution];
            Texture2D texture = new Texture2D(xResolution, yResolution, TextureFormat.RGB24, true);
            int idx = 0;
            for (int j = 0; j < xResolution; j++)
            {
                for (int i = 0; i < yResolution; i++)
                {
                    colorMap[idx] = new Color(xDelta * (i + 0.5f), yDelta * (j + 0.5f), 0);
                    idx++;
                }
            }
            texture.SetPixels(colorMap);
            texture.wrapMode = TextureWrapMode.Repeat;
            texture.filterMode = FilterMode.Point;
            texture.Apply();

            return texture;
        }

        private void OnDestroy()
        {
            chunk.OnChunkInitialized -= GenerateMesh;
        }

        private void OnValidate()
        {
            if (resolution < 2)
                resolution = 2;
            if (Application.isPlaying == false)
                GenerateMesh();
        }

        private void OnDrawGizmos()
        {
            if (refreshOnGizmos)
            {

                //chunk.index = new Vector2(transform.position.x / meshSize, transform.position.z / meshSize);
            }
        }
    }
}
