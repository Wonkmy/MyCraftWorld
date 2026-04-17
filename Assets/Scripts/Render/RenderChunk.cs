using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class RenderChunk : MonoBehaviour
{
    Chunk chunk;

    MeshFilter meshFilter;
    MeshRenderer meshRenderer;
    public void InitChunk()
    {
        chunk = new Chunk();
        // 这里就是实际生成每个Chunk数据的地方，目前是测试用的，叠加了两层草地方块，后续会根据世界生成算法来生成不同的方块
        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = chunk.GetMesh();
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.material = new Material(Shader.Find("Standard"));
        meshRenderer.material.mainTexture = WorldGen.Instance.textureManager.GetTexture();
        meshRenderer.enabled = false;
    }
    public void Render()
    {
        meshRenderer.enabled = true;
    }

    public Chunk GetChunk()
    {
        return chunk;
    }
}
