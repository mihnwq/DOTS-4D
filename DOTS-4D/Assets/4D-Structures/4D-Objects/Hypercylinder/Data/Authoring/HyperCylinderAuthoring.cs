using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class HyperCylinderAuthoring : MonoBehaviour
{
    public float radius = 1.0f;
    public float height = 2.0f;
    public int n_theta = 24;
    public int n_phi = 12;
    public int n_h = 8;
    public Color sliceColor = Color.cyan;

    class Baker : Baker<HyperCylinderAuthoring>
    {
        public override void Bake(HyperCylinderAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new HyperCylinderData
            {
                radius = authoring.radius,
                height = authoring.height,
                n_theta = math.max(1, authoring.n_theta),
                n_phi = math.max(1, authoring.n_phi),
                n_h = math.max(1, authoring.n_h),
                sliceColor = authoring.sliceColor
            });
        }
    }
}