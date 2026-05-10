using Unity.Entities;
using UnityEngine;

public class Object4DAuthoring : MonoBehaviour
{
    public float startingAngle = 0f;
    public float rotationSpeed = 0.02f;
    public float rotationSpeedMultiplier = 1f;

    class Baker : Baker<Object4DAuthoring>
    {
        public override void Bake(Object4DAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new Object4DData
            {
                angle = authoring.startingAngle,
                rotationSpeed = authoring.rotationSpeed,
                rotationSpeedMultiplier = authoring.rotationSpeedMultiplier
            });
        }
    }
}