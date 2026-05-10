using Unity.Entities;
using UnityEngine;


public struct ObjectIDData : IComponentData
{
    public int id;
}

public class ObjectIDAuthoring : MonoBehaviour
{
    public int objectID;

    class Baker : Baker<ObjectIDAuthoring>
    {
        public override void Bake(ObjectIDAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new ObjectIDData { id = authoring.objectID });
        }
    }
}
