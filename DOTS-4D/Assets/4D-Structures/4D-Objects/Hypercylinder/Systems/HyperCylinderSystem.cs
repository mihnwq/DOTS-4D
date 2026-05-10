using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial struct HyperCylinderSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<DotsDrawBuffer>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var drawBuffer = SystemAPI.GetSingletonRW<DotsDrawBuffer>().ValueRW;

        foreach (var (cylinder, obj4D, transform) in SystemAPI.Query<RefRO<HyperCylinderData>, RefRO<Object4DData>, RefRO<LocalToWorld>>())
        {
            float angleRad = math.radians(obj4D.ValueRO.angle);

            int n_theta = cylinder.ValueRO.n_theta;
            int n_phi = cylinder.ValueRO.n_phi;
            int n_h = cylinder.ValueRO.n_h;
            int totalVerts = n_theta * n_phi * n_h;

            NativeArray<float3> projectedPoints = new NativeArray<float3>(totalVerts, Allocator.Temp);

            CreateAndProjectVertices(ref projectedPoints, cylinder.ValueRO, transform.ValueRO, angleRad);
            DrawEdges(ref drawBuffer, ref projectedPoints, cylinder.ValueRO);

            projectedPoints.Dispose();
        }
    }

    [BurstCompile]
    private void CreateAndProjectVertices(ref NativeArray<float3> projectedPoints, in HyperCylinderData cyl, in LocalToWorld transform, float angle)
    {
        float cosA = math.cos(angle);
        float sinA = math.sin(angle);

        int n_theta = cyl.n_theta;
        int n_phi = cyl.n_phi;
        int n_h = cyl.n_h;
        float height = cyl.height;
        float radius = cyl.radius;

        for (int i = 0; i < n_theta; i++)
        {
            float theta = 2f * math.PI * i / n_theta;
            float cosTheta = math.cos(theta);
            float sinTheta = math.sin(theta);

            for (int j = 0; j < n_phi; j++)
            {
                float phi = math.PI * j / (n_phi / 2f);
                float cosPhi = math.cos(phi);
                float sinPhi = math.sin(phi);

                for (int k = 0; k < n_h; k++)
                {
                    float w = -height / 2f + k * height / math.max(1f, n_h - 1);
                    float x = radius * cosTheta * sinPhi;
                    float y = radius * sinTheta * sinPhi;
                    float z = radius * cosPhi;

                    float rx = x * cosA - w * sinA;
                    float rw = x * sinA + w * cosA;
                    float ry = y * cosA - z * sinA;
                    float rz = y * sinA + z * cosA;

                    float projW = 1f / (2f - rw);
                    float3 proj3D = new float3(rx * projW, ry * projW, rz * projW);

                    int idx = i * n_phi * n_h + j * n_h + k;
                    projectedPoints[idx] = math.transform(transform.Value, proj3D);
                }
            }
        }
    }

    [BurstCompile]
    private void DrawEdges(ref DotsDrawBuffer drawBuffer, ref NativeArray<float3> projectedPoints, in HyperCylinderData cyl)
    {
        Color32 col = cyl.sliceColor;
        int n_theta = cyl.n_theta;
        int n_phi = cyl.n_phi;
        int n_h = cyl.n_h;

        for (int i = 0; i < n_theta; i++)
        {
            for (int j = 0; j < n_phi; j++)
            {
                for (int k = 0; k < n_h; k++)
                {
                    int idx = i * n_phi * n_h + j * n_h + k;

                    int idx_theta = ((i + 1) % n_theta) * n_phi * n_h + j * n_h + k;
                    drawBuffer.Line(projectedPoints[idx], projectedPoints[idx_theta], col);

                    if (j + 1 < n_phi)
                    {
                        int idx_phi = i * n_phi * n_h + (j + 1) * n_h + k;
                        drawBuffer.Line(projectedPoints[idx], projectedPoints[idx_phi], col);
                    }

                    if (k + 1 < n_h)
                    {
                        int idx_h = i * n_phi * n_h + j * n_h + (k + 1);
                        drawBuffer.Line(projectedPoints[idx], projectedPoints[idx_h], col);
                    }
                }
            }
        }
    }
}