using Unity.Entities;
using UnityEngine;

public struct HypersphereData : IComponentData
{
    public int psiSegments;
    public int thetaSegments;
    public int phiSegments;
    public float radius;
    public float projectionScale;
    public float maxDistanceClamp;

    public bool rotateXW;
    public bool rotateYW;
    public bool rotateZW;
    public bool rotateXY;

    public Color32 lineColor;
}