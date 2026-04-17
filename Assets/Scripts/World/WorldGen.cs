using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class WorldGen : MonoBehaviour
{
    public Dictionary<int,RenderChunk> chunkMaps = new Dictionary<int, RenderChunk>();
    public TextureManager textureManager;

    // 需要写到配置文件里的数据，暂时写在这里
    int m_chunkSize = 16;// chunk的大小
    NoiseProperties m_noiseProperties;
    int m_localWorldSize = 8;// 世界的大小
    float m_halfLocalWorldSize;// 世界大小的一半，方便计算
    float m_xChunkOffset = 1;// chunk坐标的偏移，方便世界生成算法计算
    float m_zChunkOffset = 1;

    public static WorldGen Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    void InitData()
    {
        m_halfLocalWorldSize = m_localWorldSize * 0.5f;
        m_noiseProperties = new NoiseProperties();
        m_noiseProperties.seed = UnityEngine.Random.Range(1, 10000);
    }
    void Start()
    {
        // 初始化数据
        InitData();
        // 初始化纹理管理器
        textureManager = new TextureManager();
        textureManager._initText();
        // 创建Chunk
        CreateChunks(0, 0);
        // 渲染Chunk
        RenderChunks();
    }

    public void CreateChunks(int c_x,int c_z) {
        for (int x = 0; x <= m_localWorldSize; x++)
        {
            for (int z = 0; z <= m_localWorldSize; z++)
            {
                CreateChunk(x + c_x, z + c_z);
            }
        }
    }

    void CreateChunk(int c_x, int c_z)
    {
        // 生成一个Chunk
        GameObject chunkObj = new GameObject("Chunk");
        //chunkObj.transform.position = new Vector3(c_x, c_y, c_z);
        // 添加RenderChunk组件并初始化
        RenderChunk renderChunk = chunkObj.AddComponent<RenderChunk>();
        renderChunk.InitChunk();
        GenChunk(renderChunk, c_x, c_z);
        // 将RenderChunk添加到字典中
        AddChunkToDicts(renderChunk);
    }

    private void GenChunk(RenderChunk renderChunk,int p_xChunk, int p_zChunk)
    {
        for (byte zBlock = 0; zBlock < m_chunkSize; zBlock++)
        {
            for (byte xBlock = 0; xBlock < m_chunkSize; xBlock++)
            {
                int localX = xBlock + p_xChunk * m_chunkSize;
                int localZ = zBlock + p_zChunk * m_chunkSize;
                int worldX = LocalToWorldX(localX);
                int worldZ = LocalToWorldZ(localZ);
                byte surfaceHeight = HeightAt((float)worldX, (float)worldZ);
                Debug.Log($"surfaceHeight:{surfaceHeight}");
                if (surfaceHeight <= 19)
                {
                    renderChunk.GetChunk().SetBlock(worldX, surfaceHeight, worldZ, BlockInfo.GetBlockType(BlockType.Water));
                }
                else
                {
                    renderChunk.GetChunk().SetBlock(worldX, surfaceHeight, worldZ, BlockInfo.GetBlockType(BlockType.GrassDirt));
                }
                //renderChunk.GetChunk().SetBlock(worldX, surfaceHeight, worldZ, BlockInfo.GetBlockType(BlockType.GrassDirt));
            }
        }
    }
    public void RenderChunks()
    {
        foreach (var kvp in chunkMaps)
        {
            kvp.Value.Render();
        }
    }

    void AddChunkToDicts(RenderChunk _renderChunk)
    {
        if (!chunkMaps.ContainsKey(_renderChunk.GetChunk().ChunkID))
        {
            chunkMaps.Add(_renderChunk.GetChunk().ChunkID, _renderChunk);   
        }
    }


    public byte HeightAt(float p_x, float p_y)
    {
        float noiseHeight = 0f;
        float amplitude = 1f;
        float frequency = 1f;

        float offsetX = (m_noiseProperties.seed % 10000) * 0.001f;
        float offsetY = (m_noiseProperties.seed % 10000) * 0.002f;

        // Octaves循环
        for (byte i = 0; i < m_noiseProperties.octaves; i++)
        {
            float sampleX = (p_x / m_noiseProperties.scale) * frequency;
            float sampleY = (p_y / m_noiseProperties.scale) * frequency;

            float perlinValue = Mathf.PerlinNoise(sampleX, sampleY);
            noiseHeight += perlinValue * amplitude;

            amplitude *= m_noiseProperties.persistance;
            frequency *= m_noiseProperties.lacunarity;
        }

        noiseHeight = m_noiseProperties.maxHeight - Mathf.Floor(noiseHeight * m_noiseProperties.maxHeight);

        noiseHeight = Mathf.Pow(noiseHeight * m_noiseProperties.heightScale, m_noiseProperties.redistribution);

        return (byte)(noiseHeight + m_noiseProperties.additionalElevation);
    }

    public int LocalToWorldX(int p_localX)
    {
        return (int)(p_localX - (m_halfLocalWorldSize - m_xChunkOffset) * m_chunkSize);
    }

    public int LocalToWorldZ(int p_localZ)
    {
        return (int)(p_localZ - (m_halfLocalWorldSize - m_zChunkOffset) * m_chunkSize);
    }

    private int From3Dto1D(uint p_x, uint p_y, uint p_z)
    {
        return (int)(p_x + m_localWorldSize * (p_z + p_y * m_localWorldSize));
    }
    public byte GetBlockID(float p_x, float p_y, float p_z)
    {
        p_x = Mathf.Round(p_x);
        p_y = Mathf.Round(p_y);
        p_z = Mathf.Round(p_z);
        int xChunk = (int)((p_x / m_chunkSize) + (m_halfLocalWorldSize) - m_xChunkOffset);
        int yChunk = (int)(p_y / m_chunkSize);
        int zChunk = (int)((p_z / m_chunkSize) + (m_halfLocalWorldSize) - m_zChunkOffset);
        if (xChunk < 0 || xChunk >= m_localWorldSize ||
            yChunk < 0 || yChunk >= 16 ||
            zChunk < 0 || zChunk >= m_localWorldSize)
            return 0;
        RenderChunk target;
        if (!chunkMaps.TryGetValue(From3Dto1D((uint)xChunk, (uint)yChunk, (uint)zChunk), out target))
            return 0;
        return target.GetChunk().GetBlock((byte)((p_x + m_halfLocalWorldSize * m_chunkSize) % m_chunkSize),
                                (byte)(p_y % m_chunkSize),
                                (byte)((p_z + m_halfLocalWorldSize * m_chunkSize) % m_chunkSize));
    }
}
