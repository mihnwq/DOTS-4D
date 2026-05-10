using Unity.Entities;
using UnityEngine;

public struct HyperconeData : IComponentData
{
    public float coneSlope;
    public float wMin;
    public float wMax;
    public int wSlices;
    public int latSamples;
    public int lonSamples;
    public float projectionDistance;
    public bool usePerspective;
    public Color32 lineColor;
}