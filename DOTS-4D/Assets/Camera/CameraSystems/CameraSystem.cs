using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial struct CameraSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {

     

        bool mouseForward = Input.GetAxis("Mouse ScrollWheel") < 0;
        bool mouseBackwards = Input.GetAxis("Mouse ScrollWheel") > 0;

        float mouseX = 0f;
        float mouseY = 0f;

        if (Input.GetMouseButton(1))
        {
            mouseX = Input.GetAxis("Mouse X");
            mouseY = Input.GetAxis("Mouse Y");
        }
        else if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            mouseX = touch.deltaPosition.x * 0.02f;
            mouseY = touch.deltaPosition.y * 0.02f;
        }

        float deltaTime = SystemAPI.Time.DeltaTime;

        foreach (var (transform, cameraData) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<CameraData>>())
        {
            

            Entity target = cameraData.ValueRO.targetEntity;

            if (!SystemAPI.HasComponent<LocalTransform>(target)) return;

       

            LocalTransform targetTransform = SystemAPI.GetComponent<LocalTransform>(target);

            
            cameraData.ValueRW.distanceFromTarget = Zoom(
                cameraData.ValueRO.distanceFromTarget,
                cameraData.ValueRO.minDistance,
                cameraData.ValueRO.maxDistance,
                mouseForward,
                mouseBackwards
            );

 
            float scaledMouseX = mouseX * cameraData.ValueRO.mouseSensitivity;
            float scaledMouseY = mouseY * cameraData.ValueRO.mouseSensitivity;

            cameraData.ValueRW.rotationY += scaledMouseX;
            
            cameraData.ValueRW.rotationX -= scaledMouseY;

            
            cameraData.ValueRW.rotationX = math.clamp(cameraData.ValueRW.rotationX, -89f, 89f);

            float3 nextRotation = new float3(cameraData.ValueRO.rotationX, cameraData.ValueRO.rotationY, 0f);

           
            float3 currentVel = cameraData.ValueRO.smoothVelocity;

            cameraData.ValueRW.currentRotation = SmoothDamp(
                cameraData.ValueRO.currentRotation,
                nextRotation,
                ref currentVel,
                cameraData.ValueRO.mapSmoothTime,
                deltaTime
            );

       
            cameraData.ValueRW.smoothVelocity = currentVel;

           
            float3 radians = math.radians(cameraData.ValueRO.currentRotation);
            transform.ValueRW.Rotation = quaternion.Euler(radians);

       
            float3 newCameraForward = math.mul(transform.ValueRO.Rotation, new float3(0, 0, 1));

            transform.ValueRW.Position = CalculateNextPosition(
                newCameraForward,
                targetTransform.Position,
                cameraData.ValueRO.distanceFromTarget
            );

       
        }
    }

    [BurstCompile]
    private float3 CalculateNextPosition(float3 from, float3 to, float distance)
    {
        return to - (from * distance);
    }

    [BurstCompile]
    public float Zoom(float distanceFromTarget, float minDistance, float maxDistance, bool mouseForward, bool mouseBackwards)
    {
        if (mouseForward)
        {
            distanceFromTarget += 2;
            if (distanceFromTarget > maxDistance) distanceFromTarget = maxDistance;
        }
        else if (mouseBackwards)
        {
            distanceFromTarget -= 2;
            if (distanceFromTarget < minDistance) distanceFromTarget = minDistance;
        }

        return distanceFromTarget;
    }

  
    private static float3 SmoothDamp(float3 current, float3 target, ref float3 currentVelocity, float smoothTime, float deltaTime)
    {
        smoothTime = math.max(0.0001f, smoothTime);

        float omega = 2f / smoothTime;
        float x = omega * deltaTime;
        float exp = 1f / (1f + x + 0.48f * x * x + 0.235f * x * x * x);

        float3 change = current - target;
        float3 temp = (currentVelocity + omega * change) * deltaTime;

        currentVelocity = (currentVelocity - omega * temp) * exp;
        return target + (change + temp) * exp;
    }
}