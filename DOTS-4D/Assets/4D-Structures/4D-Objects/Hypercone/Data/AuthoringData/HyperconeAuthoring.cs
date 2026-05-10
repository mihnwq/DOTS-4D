using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class HyperconeAuthoring : MonoBehaviour
{
    [Header("Cone shape")]
    public float coneSlope = 0.8f;
    public float wMin = 0.05f;
    public float wMax = 2.0f;
    public int wSlices = 18;
    public int latSamples = 10;
    public int lonSamples = 18;

    [Header("Projection & visuals")]
    public float projectionDistance = 3.0f;
    public bool usePerspective = true;
    public Color lineColor = new Color(0.2f, 0.8f, 1f, 1f);

    class Baker : Baker<HyperconeAuthoring>
    {
        public override void Bake(HyperconeAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new HyperconeData
            {
                coneSlope = authoring.coneSlope,
                wMin = authoring.wMin,
                wMax = authoring.wMax,
                wSlices = math.max(2, authoring.wSlices),
                latSamples = math.max(2, authoring.latSamples),
                lonSamples = math.max(3, authoring.lonSamples),
                projectionDistance = math.max(0.001f, authoring.projectionDistance),
                usePerspective = authoring.usePerspective,
                lineColor = authoring.lineColor
            });
        }
    }
}