using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;


[BurstCompile]
public struct Deform4DVerticesJob : IJobParallelFor
{
    [ReadOnly] public NativeArray<OriginalVertex4D> baseVertices;
    [WriteOnly] public NativeArray<Vector3> projectedVertices;

   
    public float angle;
    public float time;
    public float wDistance;
    public float wOffset;
    public float wOscillationAmplitude;
    public float wOscillationSpeed;
    public float wDeformStrength;

    public void Execute(int i)
    {
       
        float4 v = baseVertices[i].value;

      
        v.w += wOffset;
        float currentOscillation = math.sin(time * wOscillationSpeed) * wOscillationAmplitude;
        v.w += currentOscillation;
        v.w += (v.x + v.y + v.z) * wDeformStrength;

        
        v = RotateXW(v, angle * 0.5f);
        v = RotateYW(v, angle * 0.3f);
        // v = RotateZW(v, angle * 0.2f); 

        projectedVertices[i] = ProjectTo3D(v);
    }


    private float4 RotateXW(float4 v, float a)
    {
        float c = math.cos(a);
        float s = math.sin(a);

        float x = v.x * c - v.w * s;
        float w = v.x * s + v.w * c;

        return new float4(x, v.y, v.z, w);
    }

    private float4 RotateYW(float4 v, float a)
    {
        float c = math.cos(a);
        float s = math.sin(a);

        float y = v.y * c - v.w * s;
        float w = v.y * s + v.w * c;

        return new float4(v.x, y, v.z, w);
    }

    private Vector3 ProjectTo3D(float4 v)
    {
        float k = wDistance;

        float wFactor = k - v.w;
        if (wFactor < 0.01f) wFactor = 0.01f;

        return new Vector3(
            v.x / wFactor,
            v.y / wFactor,
            v.z / wFactor
        );
    }
}


public partial class Object3Dto4DSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float time = (float)SystemAPI.Time.ElapsedTime;

     
        foreach (var (data, obj4D, meshRef, buffer, transform) in SystemAPI.Query<RefRO<Object3Dto4DData>, RefRO<Object4DData>, Mesh4DReference, DynamicBuffer<OriginalVertex4D>, RefRO<LocalToWorld>>())
        {

            if (meshRef.originalMesh == null || meshRef.material == null)
                continue;

            if (meshRef.workingMesh == null)
            {
                 meshRef.workingMesh = UnityEngine.Object.Instantiate(meshRef.originalMesh);
                 meshRef.workingMesh.MarkDynamic();

               // break;
            }


            if (buffer.Length == 0 || buffer.Length != meshRef.workingMesh.vertexCount)
                continue;

          
            if (transform.ValueRO.Value.c3.w == 0)
                continue;
         

            int vertexCount = buffer.Length;
            NativeArray<Vector3> tempProjectedVertices = new NativeArray<Vector3>(vertexCount, Allocator.TempJob);

            
            float angleRadians = math.radians(obj4D.ValueRO.angle);

            
            Deform4DVerticesJob job = new Deform4DVerticesJob
            {
                baseVertices = buffer.AsNativeArray(),
                projectedVertices = tempProjectedVertices,
                angle = angleRadians, 
                time = time,
                wDistance = data.ValueRO.wDistance,
                wOffset = data.ValueRO.wOffset,
                wOscillationAmplitude = data.ValueRO.wOscillationAmplitude,
                wOscillationSpeed = data.ValueRO.wOscillationSpeed,
                wDeformStrength = data.ValueRO.wDeformStrength
            };

         
            JobHandle handle = job.Schedule(vertexCount, 64);
            handle.Complete(); 

            
            meshRef.workingMesh.SetVertices(tempProjectedVertices);
            meshRef.workingMesh.RecalculateNormals();
            meshRef.workingMesh.RecalculateBounds();

            
            Graphics.DrawMesh(meshRef.workingMesh, transform.ValueRO.Value, meshRef.material, 0);

           
            tempProjectedVertices.Dispose();
        }
    }
}