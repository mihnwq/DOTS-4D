using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial struct HypercubeSystem : ISystem
{
   
    private NativeArray<int2> edges;

    
    public void OnCreate(ref SystemState state)
    {
        
        edges = new NativeArray<int2>(32, Allocator.Persistent);

    
        int[] rawEdges = {
            0,1, 1,3, 3,2, 2,0,
            4,5, 5,7, 7,6, 6,4,
            0,4, 1,5, 2,6, 3,7,
            8,9, 9,11, 11,10, 10,8,
            12,13, 13,15, 15,14, 14,12,
            8,12, 9,13, 10,14, 11,15,
            0,8, 1,9, 2,10, 3,11,
            4,12, 5,13, 6,14, 7,15
        };

        for (int i = 0; i < 32; i++)
        {
            edges[i] = new int2(rawEdges[i * 2], rawEdges[(i * 2) + 1]);
        }

       
        state.RequireForUpdate<DotsDrawBuffer>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        if (edges.IsCreated) edges.Dispose();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
    
        var drawBuffer = SystemAPI.GetSingletonRW<DotsDrawBuffer>().ValueRW;

        

        foreach (var (hypercube, obj4D, transform) in SystemAPI.Query<RefRO<HypercubeRawData>, RefRO<Object4DData>, RefRO<LocalToWorld>>())
        {

           // Debug.Log("Entered!");

            float angleRad = math.radians(obj4D.ValueRO.angle);
            float size = hypercube.ValueRO.size;
            float wSlice = hypercube.ValueRO.wSlice;
            float sliceThick = hypercube.ValueRO.sliceThickness;
            Color32 color = hypercube.ValueRO.sliceColor;

          
            NativeArray<float4> rotatedPoints = new NativeArray<float4>(16, Allocator.Temp);
            int i = 0;

            

            for (int x = -1; x <= 1; x += 2)
            {
                for (int y = -1; y <= 1; y += 2)
                {
                    for (int z = -1; z <= 1; z += 2)
                    {
                        for (int w = -1; w <= 1; w += 2)
                        {
                            float4 p = new float4(x, y, z, w) * size;

                            
                            float rx = p.x * math.cos(angleRad) - p.w * math.sin(angleRad);
                            float rw = p.x * math.sin(angleRad) + p.w * math.cos(angleRad);
                            p.x = rx; p.w = rw;

                     
                            float ry = p.y * math.cos(angleRad) - p.z * math.sin(angleRad);
                            float rz = p.y * math.sin(angleRad) + p.z * math.cos(angleRad);
                            p.y = ry; p.z = rz;

                            rotatedPoints[i++] = p;
                        }
                    }
                }
            }

          //  Debug.Log("RotatedPoints!");

         
            for (int e = 0; e < 32; e++)
            {
                float4 p1 = rotatedPoints[edges[e].x];
                float4 p2 = rotatedPoints[edges[e].y];

              //  if (math.abs(p1.w - wSlice) < sliceThick || math.abs(p2.w - wSlice) < sliceThick)
               // {
                
                    float w1 = 1f / (2f - p1.w);
                    float3 a = new float3(p1.x * w1, p1.y * w1, p1.z * w1);

                    float w2 = 1f / (2f - p2.w);
                    float3 b = new float3(p2.x * w2, p2.y * w2, p2.z * w2);

                  
                    float3 worldA = math.transform(transform.ValueRO.Value, a);
                    float3 worldB = math.transform(transform.ValueRO.Value, b);

                
                    drawBuffer.Line(worldA, worldB, color);

                  //  Debug.Log("Drew Line!");
              //  }
            }

  
            rotatedPoints.Dispose();
        }
    }
}