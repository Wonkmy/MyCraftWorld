[System.Serializable]
public class NoiseProperties
{
    public int seed = 12345;// 随机种子
    public float scale = 35.0f;// 噪声的缩放，越大越平坦，越小越崎岖
    public byte octaves = 6;// 迭代次数，越多越细节，但也越慢
    public float persistance = 0.06f;// 每次迭代振幅的衰减，越小越平坦，越大越崎岖
    public float lacunarity = 1.4f;// 每次迭代频率的增加，越大越细节，越小越平坦

    public float maxHeight = 40f;// 最大高度，噪声值将被乘以这个值来得到最终的高度
    public float heightScale = 0.5f;// 高度缩放，越大越高，越小越低
    public float redistribution = 1.05f;// 重分布，越大越平坦，越小越崎岖
    public float additionalElevation = 0f;// 额外的高度提升，越大越高，越小越低
}