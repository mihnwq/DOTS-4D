using Unity.Entities;
using UnityEngine;

public class HypersphereAuthoring : MonoBehaviour
{
    [Range(4, 50)] public int psiSegments = 12;
    [Range(4, 50)] public int thetaSegments = 12;
    [Range(4, 50)] public int phiSegments = 12;
    public float radius = 1f;

    public float projectionScale = 2f;
    public float maxDistanceClamp = 30f;

    public bool rotateXW = true;
    public bool rotateYW = true;
    public bool rotateZW = false;
    public bool rotateXY = false;

    public Color lineColor = Color.magenta;

    class Baker : Baker<HypersphereAuthoring>
    {
        public override void Bake(HypersphereAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new HypersphereData
            {
                psiSegments = authoring.psiSegments,
                thetaSegments = authoring.thetaSegments,
                phiSegments = authoring.phiSegments,
                radius = authoring.radius,
                projectionScale = authoring.projectionScale,
                maxDistanceClamp = authoring.maxDistanceClamp,
                rotateXW = authoring.rotateXW,
                rotateYW = authoring.rotateYW,
                rotateZW = authoring.rotateZW,
                rotateXY = authoring.rotateXY,
                lineColor = authoring.lineColor
            });
        }
    }
}