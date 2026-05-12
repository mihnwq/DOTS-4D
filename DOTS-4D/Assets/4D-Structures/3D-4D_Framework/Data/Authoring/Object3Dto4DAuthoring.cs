using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class Object3Dto4DAuthoring : MonoBehaviour
{
    [Header("Rendering")]
    public Mesh sourceMesh;
    public Material meshMaterial;

    [Header("4D Parameters")]
    public float rotationSpeed = 1f;
    public float wDistance = 3f;
    public float wOffset = 0f;
    public float wOscillationAmplitude = 0f;
    public float wOscillationSpeed = 2f;
    public float wDeformStrength = 0f;

    class Baker : Baker<Object3Dto4DAuthoring>
    {
        public override void Bake(Object3Dto4DAuthoring authoring)
        {
            if (authoring.sourceMesh == null || authoring.meshMaterial == null) return;

            var entity = GetEntity(TransformUsageFlags.Dynamic);

        
            AddComponent(entity, new Object4DData
            {
                angle = 0f,
                rotationSpeed = authoring.rotationSpeed
            });

            
            AddComponent(entity, new Object3Dto4DData
            {
                wDistance = authoring.wDistance,
                wOffset = authoring.wOffset,
                wOscillationAmplitude = authoring.wOscillationAmplitude,
                wOscillationSpeed = authoring.wOscillationSpeed,
                wDeformStrength = authoring.wDeformStrength
            });

            var vertexBuffer = AddBuffer<OriginalVertex4D>(entity);

            
            if (authoring.sourceMesh != null)
            {
                Vector3[] vertices = authoring.sourceMesh.vertices;
                foreach (Vector3 v in vertices)
                {
                    vertexBuffer.Add(new OriginalVertex4D { value = new float4(v.x, v.y, v.z, 0f) });
                }
            }


            AddComponentObject(entity, new Mesh4DReference
            {
                originalMesh = authoring.sourceMesh,
                workingMesh = null, 
                material = authoring.meshMaterial
            });
        }
    }
}