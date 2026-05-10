using Unity.Entities;
using UnityEngine;

public class MoveAuthoring : MonoBehaviour
{
    public float speed = 5f;

    class Baker : Baker<MoveAuthoring>
    {
        public override void Bake(MoveAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new MoveData
            {
                speed = authoring.speed
            });

            Debug.Log("Baked");
        }
    }
}
