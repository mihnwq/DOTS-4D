using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial struct HypersphereSystem : ISystem
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

        foreach (var (sphere, obj4D, transform) in SystemAPI.Query<RefRO<HypersphereData>, RefRO<Object4DData>, RefRO<LocalToWorld>>())
        {
          
            float t = math.radians(obj4D.ValueRO.angle) * 0.5f;

            DrawHypersphere(ref drawBuffer, sphere.ValueRO, transform.ValueRO, t);
        }
    }



    [BurstCompile]
    private void DrawHypersphere(ref DotsDrawBuffer drawBuffer, in HypersphereData sphere, in LocalToWorld transform, float t)
    {
        int psiSegments = sphere.psiSegments;
        int thetaSegments = sphere.thetaSegments;
        int phiSegments = sphere.phiSegments;
        Color32 col = sphere.lineColor;

        for (int i = 0; i < psiSegments; i++)
        {
            float psi0 = math.PI * i / psiSegments;
            float psi1 = math.PI * (i + 1) / psiSegments;

            for (int j = 0; j < thetaSegments; j++)
            {
                float theta0 = math.PI * j / thetaSegments;
                float theta1 = math.PI * (j + 1) / thetaSegments;

                for (int k = 0; k < phiSegments; k++)
                {
                    float phi0 = 2f * math.PI * k / phiSegments;
                    float phi1 = 2f * math.PI * (k + 1) / phiSegments;

                    float4 p000 = S3ToR4(psi0, theta0, phi0, sphere.radius);
                    float4 p100 = S3ToR4(psi1, theta0, phi0, sphere.radius);
                    float4 p010 = S3ToR4(psi0, theta1, phi0, sphere.radius);
                    float4 p001 = S3ToR4(psi0, theta0, phi1, sphere.radius);

                    p000 = Rotate4D(p000, t, sphere);
                    p100 = Rotate4D(p100, t, sphere);
                    p010 = Rotate4D(p010, t, sphere);
                    p001 = Rotate4D(p001, t, sphere);

                    float3 v000 = ProjectTo3D(p000, sphere, transform);
                    float3 v100 = ProjectTo3D(p100, sphere, transform);
                    float3 v010 = ProjectTo3D(p010, sphere, transform);
                    float3 v001 = ProjectTo3D(p001, sphere, transform);

                    drawBuffer.Line(v000, v100, col);
                    drawBuffer.Line(v000, v010, col);
                    drawBuffer.Line(v000, v001, col);
                }
            }
        }
    }

    [BurstCompile]
    private float4 S3ToR4(float psi, float theta, float phi, float radius)
    {
        float x = math.cos(psi);
        float y = math.sin(psi) * math.cos(theta);
        float z = math.sin(psi) * math.sin(theta) * math.cos(phi);
        float w = math.sin(psi) * math.sin(theta) * math.sin(phi);

        return new float4(x, y, z, w) * radius;
    }

    [BurstCompile]
    private float4 Rotate4D(float4 v, float t, in HypersphereData sphere)
    {
        float cosA = math.cos(t);
        float sinA = math.sin(t);

        if (sphere.rotateXW)
        {
            float x = v.x * cosA - v.w * sinA;
            float w = v.x * sinA + v.w * cosA;
            v.x = x; v.w = w;
        }

        if (sphere.rotateYW)
        {
            float y = v.y * cosA - v.w * sinA;
            float w = v.y * sinA + v.w * cosA;
            v.y = y; v.w = w;
        }

        if (sphere.rotateZW)
        {
            float z = v.z * cosA - v.w * sinA;
            float w = v.z * sinA + v.w * cosA;
            v.z = z; v.w = w;
        }

        if (sphere.rotateXY)
        {
            float x = v.x * cosA - v.y * sinA;
            float y = v.x * sinA + v.y * cosA;
            v.x = x; v.y = y;
        }

        return v;
    }

    [BurstCompile]
    private float3 ProjectTo3D(float4 p, in HypersphereData sphere, in LocalToWorld transform)
    {
        float denom = 1f - p.w;

        if (math.abs(denom) < 1e-5f)
            denom = 1e-5f * math.sign(denom);

        float3 proj = new float3(p.x, p.y, p.z) / denom;

        if (math.length(proj) > sphere.maxDistanceClamp)
            proj = math.normalize(proj) * sphere.maxDistanceClamp;

       
        return math.transform(transform.Value, proj * sphere.projectionScale);
    }
}