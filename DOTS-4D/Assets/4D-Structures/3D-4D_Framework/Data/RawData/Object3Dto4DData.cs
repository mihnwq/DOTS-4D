using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;


public struct Object3Dto4DData : IComponentData
{
    public float wDistance;
    public float wOffset;
    public float wOscillationAmplitude;
    public float wOscillationSpeed;
    public float wDeformStrength;
}


public struct OriginalVertex4D : IBufferElementData
{
    public float4 value;
}


public class Mesh4DReference : IComponentData, IDisposable
{
    public Mesh originalMesh;
    public Mesh workingMesh;
    public Material material;

    public void Dispose()
    {
        if (workingMesh != null)
        {
            UnityEngine.Object.Destroy(workingMesh);
        }
    }
}