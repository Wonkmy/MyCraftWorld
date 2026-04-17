using System.Collections.Generic;
using UnityEngine;
public enum BlockType
{
    Air = -1,
    Grass = 0,
    GrassDirt = 1,
    Dirt = 2,
    Stone = 3,
    Water = 4,
    // 可以继续添加其他方块类型
}

public static class BlockInfo
{
    // 每种方块类型对应的UV坐标（假设每个方块贴图是图集中的一个小格，atlasSize为图集行/列数）
    public static Dictionary<int, Vector2[]> BlockUVs = new Dictionary<int, Vector2[]>();
    public static int GetBlockType(BlockType blockType)
    {
        return (int)blockType;
    }
    public static BlockType GetBlockType(int blockType)
    {
        return (BlockType)blockType;
    }
    public static void InitBlockUVs(int atlasSize)
    {
        BlockUVs[GetBlockType(BlockType.Air)] = null;// 空气不需要UV
        BlockUVs[GetBlockType(BlockType.Grass)] = GetUVsForAtlasCell(0, 0, atlasSize);// 0 代表草地
        BlockUVs[GetBlockType(BlockType.GrassDirt)] = GetUVsForAtlasCell(1, 0, atlasSize);// 1 代表d带草地的泥土
        BlockUVs[GetBlockType(BlockType.Dirt)] = GetUVsForAtlasCell(2, 0, atlasSize);// 2 代表泥土
        BlockUVs[GetBlockType(BlockType.Stone)] = GetUVsForAtlasCell(3, 0, atlasSize);// 3 代表石头
        BlockUVs[GetBlockType(BlockType.Water)] = GetUVsForAtlasCell(1, 1, atlasSize);// 4 代表水
    }

    // 获取图集中特定格子的UV坐标
    static Vector2[] GetUVsForAtlasCell(int cellX, int cellY, int atlasSize)
    {
        float unit = 1.0f / atlasSize;
        float x = cellX * unit;
        //float y = cellY * unit;
        float y = (atlasSize - 1 - cellY) * unit;
        return new Vector2[]
        {
            new Vector2(x, y),// 左下
            new Vector2(x + unit, y),// 右下
            new Vector2(x + unit, y + unit),// 右上
            new Vector2(x, y + unit)// 左上
        };
    }

    public static bool IsCollidable(BlockType blockType)
    {
        return blockType != BlockType.Air && blockType != BlockType.Water;
    }
}