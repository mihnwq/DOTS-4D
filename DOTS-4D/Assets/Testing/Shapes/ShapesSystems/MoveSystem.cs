using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial struct MoveSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        
        bool isUpArrowPressed = Input.GetKey(KeyCode.UpArrow);

  
        if (!isUpArrowPressed) return;

        float deltaTime = SystemAPI.Time.DeltaTime;

        
        foreach (var (transform, movement) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<MoveData>>())
        {
            var draw = SystemAPI.GetSingletonRW<DotsDrawBuffer>().ValueRW;

           for(float i = 0; i <= 3; i+=0.1f)
            {
                float3 start = new float3(-2.76f, 0.17f, 0);
                float3 end = new float3(2.76f + i , 2, 0);
                Color32 myColor = new Color32(255, 0, 0, 255);

                draw.Line(start, end, myColor);

                Debug.Log("Drew Line!");
            }
            

            float3 forwardDirection = new float3(0, 0, 1);

            Debug.Log("I'm in");

            
            transform.ValueRW.Position += forwardDirection * movement.ValueRO.speed * deltaTime;
        }
    }
}