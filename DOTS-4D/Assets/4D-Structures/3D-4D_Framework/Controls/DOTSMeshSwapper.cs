using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;

public class DOTSMeshSwapper : MonoBehaviour
{
    private EntityManager em;

    public Mesh testMesh;
    public Material testMaterial;

    private Mesh lastMesh;

    public static DOTSMeshSwapper instance;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        em = World.DefaultGameObjectInjectionWorld.EntityManager;
        SwapMesh(null);
    }

    void Update()
    {
        if(testMesh != lastMesh)
        {
            Debug.Log("Swapping");
            SwapMesh(testMesh);
        }
    }

    public void SwapMesh(Mesh newMesh, Material newMaterial = null)
    {
        EntityQuery query = new EntityQueryBuilder(Allocator.Temp)
            .WithAll<Mesh4DReference, OriginalVertex4D>()
            .WithOptions(EntityQueryOptions.IncludeDisabledEntities)
            .Build(em);

        NativeArray<Entity> entities = query.ToEntityArray(Allocator.Temp);

        foreach (Entity entity in entities)
        {
            DynamicBuffer<OriginalVertex4D> vertexBuffer = em.GetBuffer<OriginalVertex4D>(entity);

            
            vertexBuffer.Clear();

            
            if (newMesh != null)
            {
                Vector3[] newVertices = newMesh.vertices;
                foreach (Vector3 v in newVertices)
                {
                    vertexBuffer.Add(new OriginalVertex4D { value = new float4(v.x, v.y, v.z, 0f) });
                }
            }

            Mesh4DReference meshRef = em.GetComponentData<Mesh4DReference>(entity);

            if (meshRef.workingMesh != null)
            {
                Destroy(meshRef.workingMesh);
            }

          
            meshRef.originalMesh = newMesh;
            meshRef.workingMesh = null;

            if (newMaterial != null)
            {
                meshRef.material = newMaterial;
            }
        }

        entities.Dispose();

       
         lastMesh = newMesh; 
    }



}