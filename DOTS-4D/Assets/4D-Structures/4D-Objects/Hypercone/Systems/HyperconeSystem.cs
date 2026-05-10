using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct HyperconeSystem : ISystem
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

        foreach (var (cone, obj4D, transform) in SystemAPI.Query<RefRO<HyperconeData>, RefRO<Object4DData>, RefRO<LocalToWorld>>())
        {
            float angleRad = math.radians(obj4D.ValueRO.angle);

            int vertsPerSlice = cone.ValueRO.latSamples * cone.ValueRO.lonSamples;
            int totalVerts = cone.ValueRO.wSlices * vertsPerSlice;

          
            NativeArray<float4> samples = new NativeArray<float4>(totalVerts, Allocator.Temp);

      
            BuildSamples(ref samples, cone.ValueRO);
            DrawHypercone(ref drawBuffer, ref samples, cone.ValueRO, transform.ValueRO, angleRad);

       
            samples.Dispose();
        }
    }



    [BurstCompile]
    private void BuildSamples(ref NativeArray<float4> samples, in HyperconeData cone)
    {
        for (int i = 0; i < cone.wSlices; i++)
        {
            float t = (float)i / (cone.wSlices - 1);
            float w = math.lerp(cone.wMin, cone.wMax, t);
            float r = cone.coneSlope * math.abs(w);

            for (int lat = 0; lat < cone.latSamples; lat++)
            {
                float v = (cone.latSamples == 1) ? 0f : ((float)lat / (cone.latSamples - 1));
                float phi = math.lerp(-math.PI / 2f, math.PI / 2f, v);

                float cosPhi = math.cos(phi);
                float sinPhi = math.sin(phi);

                for (int lon = 0; lon < cone.lonSamples; lon++)
                {
                    float u = (float)lon / cone.lonSamples;
                    float theta = u * math.PI * 2f;

                    float x = r * cosPhi * math.cos(theta);
                    float y = r * cosPhi * math.sin(theta);
                    float z = r * sinPhi;

                    int idx = (i * cone.latSamples * cone.lonSamples) + (lat * cone.lonSamples) + lon;
                    samples[idx] = new float4(x, y, z, w);
                }
            }
        }
    }

    [BurstCompile]
    private void DrawHypercone(ref DotsDrawBuffer drawBuffer, ref NativeArray<float4> samples, in HyperconeData cone, in LocalToWorld transform, float angle)
    {
        UnityEngine.Color32 col = cone.lineColor;
        int vertsPerSlice = cone.latSamples * cone.lonSamples;

        for (int i = 0; i < cone.wSlices; i++)
        {
     
            int sliceOffset = i * vertsPerSlice;

            for (int lat = 0; lat < cone.latSamples; lat++)
            {
                for (int lon = 0; lon < cone.lonSamples; lon++)
                {
                    int a = sliceOffset + lat * cone.lonSamples + lon;
                    int b = sliceOffset + lat * cone.lonSamples + ((lon + 1) % cone.lonSamples);

                    float3 pa = ProjectAndRotate4Dto3D(samples[a], angle, cone);
                    float3 pb = ProjectAndRotate4Dto3D(samples[b], angle, cone);

                  
                    drawBuffer.Line(math.transform(transform.Value, pa), math.transform(transform.Value, pb), col);
                }
            }

  
            for (int lon = 0; lon < cone.lonSamples; lon++)
            {
                for (int lat = 0; lat < cone.latSamples - 1; lat++)
                {
                    int a = sliceOffset + lat * cone.lonSamples + lon;
                    int b = sliceOffset + (lat + 1) * cone.lonSamples + lon;

                    float3 pa = ProjectAndRotate4Dto3D(samples[a], angle, cone);
                    float3 pb = ProjectAndRotate4Dto3D(samples[b], angle, cone);

                    drawBuffer.Line(math.transform(transform.Value, pa), math.transform(transform.Value, pb), col);
                }
            }

      
            if (i < cone.wSlices - 1)
            {
                int nextSliceOffset = (i + 1) * vertsPerSlice;
                for (int v = 0; v < vertsPerSlice; v++)
                {
                    int a = sliceOffset + v;
                    int b = nextSliceOffset + v;

                    float3 pa = ProjectAndRotate4Dto3D(samples[a], angle, cone);
                    float3 pb = ProjectAndRotate4Dto3D(samples[b], angle, cone);

                    drawBuffer.Line(math.transform(transform.Value, pa), math.transform(transform.Value, pb), col);
                }
            }
        }
    }

    [BurstCompile]
    private float3 ProjectAndRotate4Dto3D(float4 p, float angle, in HyperconeData cone)
    {
        float cosA = math.cos(angle);
        float sinA = math.sin(angle);

  
        float x = p.x * cosA - p.w * sinA;
        float w = p.x * sinA + p.w * cosA;

     
        float y = p.y * cosA - p.z * sinA;
        float z = p.y * sinA + p.z * cosA;

        float4 rotated = new float4(x, y, z, w);

      
        if (cone.usePerspective)
        {
            float denom = cone.projectionDistance - rotated.w;
            if (math.abs(denom) < 0.0001f)
                denom = 0.0001f * math.sign(denom == 0 ? 1f : denom);

            float scale = 1f / denom;
            return new float3(rotated.x * scale, rotated.y * scale, rotated.z * scale);
        }
        else
        {
            return new float3(rotated.x, rotated.y, rotated.z);
        }
    }
}