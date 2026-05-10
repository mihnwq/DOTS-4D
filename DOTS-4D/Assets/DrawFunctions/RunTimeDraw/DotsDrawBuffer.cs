using System.Runtime.InteropServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;


[StructLayout(LayoutKind.Sequential)]
public struct LineVertex
{
    public float3 position;
    public Color32 color;
}

public struct DotsDrawBuffer : IComponentData
{
    public NativeList<LineVertex> vertices;

    public void Line(float3 a, float3 b, Color32 color)
    {
        vertices.Add(new LineVertex { position = a, color = color });
        vertices.Add(new LineVertex { position = b, color = color });
    }
}


[UpdateInGroup(typeof(PresentationSystemGroup))]
public partial class DotsDebugDrawSystem : SystemBase
{
    private Mesh lineMesh;
    private Material lineMaterial;
    private NativeArray<int> indices;

    protected override void OnCreate()
    {
      
        var entity = EntityManager.CreateEntity();
        EntityManager.AddComponentData(entity, new DotsDrawBuffer
        {
            vertices = new NativeList<LineVertex>(Allocator.Persistent)
        });

        lineMesh = new Mesh { name = "DOTS_DebugLineMesh" };
        lineMesh.MarkDynamic();

        var shader = Shader.Find("Hidden/Internal-Colored") ?? Shader.Find("Sprites/Default");
        lineMaterial = new Material(shader) { hideFlags = HideFlags.HideAndDontSave };
        lineMaterial.SetInt("_SrcBlend", (int)BlendMode.SrcAlpha);
        lineMaterial.SetInt("_DstBlend", (int)BlendMode.OneMinusSrcAlpha);
        lineMaterial.SetInt("_Cull", (int)CullMode.Off);
        lineMaterial.SetInt("_ZWrite", 0);

        int maxVertices = 1000000;
        indices = new NativeArray<int>(maxVertices, Allocator.Persistent);
        for (int i = 0; i < maxVertices; i++) indices[i] = i;
    }

    protected override void OnDestroy()
    {
        if (SystemAPI.TryGetSingletonRW<DotsDrawBuffer>(out var drawBuffer))
        {
            drawBuffer.ValueRW.vertices.Dispose();
        }

        indices.Dispose();
        if (lineMesh != null) UnityEngine.Object.Destroy(lineMesh);
        if (lineMaterial != null) UnityEngine.Object.Destroy(lineMaterial);
    }

    protected override void OnUpdate()
    {
        if (!SystemAPI.TryGetSingletonRW<DotsDrawBuffer>(out var drawBuffer)) return;

        int vertexCount = drawBuffer.ValueRO.vertices.Length;
        if (vertexCount == 0) return;

        if (vertexCount > indices.Length) vertexCount = indices.Length;

        lineMesh.Clear();
        lineMesh.SetVertexBufferParams(vertexCount,
            new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3),
            new VertexAttributeDescriptor(VertexAttribute.Color, VertexAttributeFormat.UNorm8, 4));

        lineMesh.SetVertexBufferData(drawBuffer.ValueRO.vertices.AsArray(), 0, 0, vertexCount, 0, MeshUpdateFlags.DontValidateIndices);
        lineMesh.SetIndexBufferParams(vertexCount, IndexFormat.UInt32);
        lineMesh.SetIndexBufferData(indices, 0, 0, vertexCount, MeshUpdateFlags.DontValidateIndices);
        lineMesh.SetSubMesh(0, new SubMeshDescriptor(0, vertexCount, MeshTopology.Lines), MeshUpdateFlags.DontValidateIndices);

        lineMesh.bounds = new Bounds(Vector3.zero, new Vector3(10000, 10000, 10000));
        Graphics.DrawMesh(lineMesh, Vector3.zero, Quaternion.identity, lineMaterial, 0);

        
        drawBuffer.ValueRW.vertices.Clear();
    }
}
