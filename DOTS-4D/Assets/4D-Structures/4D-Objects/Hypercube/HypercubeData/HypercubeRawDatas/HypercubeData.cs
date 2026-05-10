using Unity.Entities;
using UnityEngine;

public struct HypercubeRawData : IComponentData
{
    public float wSlice;
    public float size;
    public float sliceThickness;
    public Color32 sliceColor;
}