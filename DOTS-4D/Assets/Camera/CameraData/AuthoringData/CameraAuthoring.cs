using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class CameraAuthoring : MonoBehaviour
{
    
    public Transform target;
    public float mouseSensitivity = 3.0f;
    public float distanceFromTarget = 5.0f;
    public float maxDistance = 100f;
    public float minDistance = 0f;
    public float mapSmoothTime = 0.2f;
    public float smallestClampDown = 0f;
    public float minClampDown = -20f;

    class Baker : Baker<CameraAuthoring>
    {
        public override void Bake(CameraAuthoring authoring)
        {
           // Debug.Log("Baked Camera");

            var entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new CameraData
            {
                // Mapped from Authoring
                targetEntity = GetEntity(authoring.target, TransformUsageFlags.Dynamic),
                mouseSensitivity = authoring.mouseSensitivity,
                distanceFromTarget = authoring.distanceFromTarget,
                maxDistance = authoring.maxDistance,
                minDistance = authoring.minDistance,
                mapSmoothTime = authoring.mapSmoothTime,
                smallestClampDown = authoring.smallestClampDown,
                minClampDown = authoring.minClampDown,

                // Initialized directly in the Baker (the "private non-declared" ones)
                rotationY = 0f,
                rotationX = 0f,
                currentRotation = float3.zero,
                smoothVelocity = float3.zero
            }) ;
        }
    }
}
