using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureManager
{
    Texture2D texture;

    public void _initText()
    {
        texture = Resources.Load<Texture2D>("Textures/blocks");

        // 初始化纹理
        BlockInfo.InitBlockUVs(6);
    }
    public Texture2D GetTexture()
    {
        if (texture == null)
        {
            return null;
        }
        return texture;
    }
}
