using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Chunk
{
    public int ChunkID {  get; private set; }
    Mesh mesh;
    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();
    List<Vector2> uvs = new List<Vector2>();

    Dictionary<Vector3Int,BlockType> blockMaps = new Dictionary<Vector3Int, BlockType>();// 存储方块数据，key是方块坐标，value是方块类型

    public Chunk()
    {
        ChunkID = System.Guid.NewGuid().GetHashCode();
        mesh = new Mesh();
    }

    public void SetBlock(int x, int y, int z, int blockType,bool needUpdate = true)
    {
        if (blockType == -1) return;// 如果是空气方块，不需要生成任何面
        // 存储方块数据
        blockMaps[new Vector3Int(x, y, z)] = BlockInfo.GetBlockType(blockType);

        if (needUpdate)
            UpdateChunk();
    }
    void UpdateChunk()
    {
        Clear();

        foreach (var kvp in blockMaps)
        {
            var pos = kvp.Key;
            var blockType = kvp.Value;
            if (blockType == BlockType.Air) continue;

            Vector2[] blockUVs;
            if (!BlockInfo.BlockUVs.TryGetValue((int)blockType, out blockUVs))
            {
                blockUVs = new Vector2[]
                {
                new Vector2(0.0f, 0.0f),
                new Vector2(1.0f, 0.0f),
                new Vector2(1.0f, 1.0f),
                new Vector2(0.0f, 1.0f)
                };
            }

            int x = pos.x, y = pos.y, z = pos.z;

            // 前面 (+z)
            if (IsAir(x, y, z + 1))
            {
                int vertIndex = vertices.Count;
                vertices.Add(new Vector3(x, y, z + 1));
                vertices.Add(new Vector3(x + 1, y, z + 1));
                vertices.Add(new Vector3(x + 1, y + 1, z + 1));
                vertices.Add(new Vector3(x, y + 1, z + 1));
                uvs.AddRange(blockUVs);
                triangles.AddRange(new int[] {
                vertIndex, vertIndex + 1, vertIndex + 2,
                vertIndex, vertIndex + 2, vertIndex + 3
            });
            }
            // 后面 (-z)
            if (IsAir(x, y, z - 1))
            {
                int vertIndex = vertices.Count;
                vertices.Add(new Vector3(x + 1, y, z));
                vertices.Add(new Vector3(x, y, z));
                vertices.Add(new Vector3(x, y + 1, z));
                vertices.Add(new Vector3(x + 1, y + 1, z));
                uvs.AddRange(blockUVs);
                triangles.AddRange(new int[] {
                vertIndex, vertIndex + 1, vertIndex + 2,
                vertIndex, vertIndex + 2, vertIndex + 3
            });
            }
            // 左面 (-x)
            if (IsAir(x - 1, y, z))
            {
                int vertIndex = vertices.Count;
                vertices.Add(new Vector3(x, y, z));
                vertices.Add(new Vector3(x, y, z + 1));
                vertices.Add(new Vector3(x, y + 1, z + 1));
                vertices.Add(new Vector3(x, y + 1, z));
                uvs.AddRange(blockUVs);
                triangles.AddRange(new int[] {
                vertIndex, vertIndex + 1, vertIndex + 2,
                vertIndex, vertIndex + 2, vertIndex + 3
            });
            }
            // 右面 (+x)
            if (IsAir(x + 1, y, z))
            {
                int vertIndex = vertices.Count;
                vertices.Add(new Vector3(x + 1, y, z + 1));
                vertices.Add(new Vector3(x + 1, y, z));
                vertices.Add(new Vector3(x + 1, y + 1, z));
                vertices.Add(new Vector3(x + 1, y + 1, z + 1));
                uvs.AddRange(blockUVs);
                triangles.AddRange(new int[] {
                vertIndex, vertIndex + 1, vertIndex + 2,
                vertIndex, vertIndex + 2, vertIndex + 3
            });
            }
            // 顶面 (+y)
            if (IsAir(x, y + 1, z))
            {
                int vertIndex = vertices.Count;
                vertices.Add(new Vector3(x, y + 1, z + 1));
                vertices.Add(new Vector3(x + 1, y + 1, z + 1));
                vertices.Add(new Vector3(x + 1, y + 1, z));
                vertices.Add(new Vector3(x, y + 1, z));
                Vector2[] _uv = blockUVs;
                if (blockType == BlockType.GrassDirt)
                    _uv = BlockInfo.BlockUVs[BlockInfo.GetBlockType(BlockType.Grass)];
                uvs.AddRange(_uv);
                triangles.AddRange(new int[] {
                vertIndex, vertIndex + 1, vertIndex + 2,
                vertIndex, vertIndex + 2, vertIndex + 3
            });
            }
            // 底面 (-y)
            if (IsAir(x, y - 1, z))
            {
                int vertIndex = vertices.Count;
                vertices.Add(new Vector3(x, y, z));
                vertices.Add(new Vector3(x + 1, y, z));
                vertices.Add(new Vector3(x + 1, y, z + 1));
                vertices.Add(new Vector3(x, y, z + 1));
                Vector2[] _uv = blockUVs;
                if (blockType == BlockType.GrassDirt)
                    _uv = BlockInfo.BlockUVs[BlockInfo.GetBlockType(BlockType.Dirt)];
                uvs.AddRange(_uv);
                triangles.AddRange(new int[] {
                vertIndex, vertIndex + 1, vertIndex + 2,
                vertIndex, vertIndex + 2, vertIndex + 3
            });
            }
        }

        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.SetUVs(0, uvs);
        mesh.RecalculateNormals();
    }

    private bool IsAir(int x, int y, int z)
    {
        return !blockMaps.ContainsKey(new Vector3Int(x, y, z)) || blockMaps[new Vector3Int(x, y, z)] == BlockType.Air;
    }
    public void Clear()
    {
        vertices.Clear();
        triangles.Clear();
        uvs.Clear();
        mesh.Clear();
    }
    public Mesh GetMesh()
    {
        return mesh;
    }
    public byte GetBlock(byte p_x, byte p_y, byte p_z)
    {
        Vector3Int pos = new Vector3Int(p_x, p_y, p_z);
        return (byte)BlockInfo.GetBlockType(blockMaps[pos]);
    }

    public byte GetBlockBySide(byte p_x, byte p_y, byte p_z, Side side)
    {
        Vector3Int pos = new Vector3Int(p_x, p_y, p_z);
        Vector3Int offset = Vector3Int.zero;
        switch (side)
        {
            case Side.FRONT: offset = new Vector3Int(0, 0, 1); break;// 前
            case Side.BACK: offset = new Vector3Int(0, 0, -1); break;// 后
            case Side.LEFT: offset = new Vector3Int(-1, 0, 0); break;// 左
            case Side.RIGHT: offset = new Vector3Int(1, 0, 0); break;// 右
            case Side.TOP: offset = new Vector3Int(0, 1, 0); break;// 上
            case Side.BOT: offset = new Vector3Int(0, -1, 0); break;// 下
        }
        return GetBlock((byte)(p_x + offset.x), (byte)(p_y + offset.y), (byte)(p_z + offset.z));
    }

    private int From3Dto1D(int x, int y, int z)
    {
        return x + y * 16 + z * 16 * 16;
    }
}