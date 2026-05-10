using Unity.Entities;
using UnityEngine;

public class HypercubeAuthoring : MonoBehaviour
{
    [Range(-2f, 2f)]
    public float wSlice = 5f; 

    public float size = 1f;
    public float sliceThickness = 0.5f;
    public Color sliceColor = Color.cyan;

    class Baker : Baker<HypercubeAuthoring>
    {
        public override void Bake(HypercubeAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new HypercubeRawData
            {
                wSlice = authoring.wSlice,
                size = authoring.size,
                sliceThickness = authoring.sliceThickness,
                sliceColor = authoring.sliceColor
            });

            Debug.Log("Baked Hypercube!");
        }
    }
}