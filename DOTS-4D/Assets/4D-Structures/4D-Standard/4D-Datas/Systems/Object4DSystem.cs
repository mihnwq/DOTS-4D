using Unity.Burst;
using Unity.Entities;

[BurstCompile]
public partial struct Object4DSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;

      
        float radToDeg = 57.29578f;

       
        foreach (var object4D in SystemAPI.Query<RefRW<Object4DData>>())
        {
            object4D.ValueRW.angle += deltaTime * object4D.ValueRO.rotationSpeed * radToDeg;
        }
    }
}