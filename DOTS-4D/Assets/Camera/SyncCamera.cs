using UnityEngine;
using Unity.Entities;
using Unity.Transforms;

public class SyncCamera : MonoBehaviour
{
    private EntityManager em;
    private EntityQuery cameraQuery;

    void Start()
    {
        
        em = World.DefaultGameObjectInjectionWorld.EntityManager;

        
        cameraQuery = em.CreateEntityQuery(typeof(CameraData), typeof(LocalTransform));
    }

    void LateUpdate()
    {
        
        if (cameraQuery.IsEmpty) return;

     
        LocalTransform dotsTransform = cameraQuery.GetSingleton<LocalTransform>();

        
        transform.position = dotsTransform.Position;
        transform.rotation = dotsTransform.Rotation;
    }
}