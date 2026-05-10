using Unity.Entities;
using UnityEngine;

public struct HyperCylinderData : IComponentData
{
    public float radius;
    public float height;
    public int n_theta;
    public int n_phi;
    public int n_h;
    public Color32 sliceColor;
}