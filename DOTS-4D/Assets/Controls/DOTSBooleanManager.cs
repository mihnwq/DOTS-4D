using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using UnityEngine.UI;

public class DOTSBooleanManager : MonoBehaviour
{
    private EntityManager em;

    [SerializeField]
    Toggle perspectiveToggle;

    public void Start()
    {
        em = World.DefaultGameObjectInjectionWorld.EntityManager;
        perspectiveToggle.onValueChanged.AddListener(OnToggle);
    }

    public void OnToggle(bool toggle)
    {

        EntityQuery query = new EntityQueryBuilder(Allocator.Temp)
            .WithAll<ObjectIDData>()
            .WithOptions(EntityQueryOptions.IncludeDisabledEntities)
            .Build(em);


        NativeArray<Entity> entities = query.ToEntityArray(Allocator.Temp);


        foreach (Entity entity in entities)
        {
          if(em.HasComponent<HyperconeData>(entity))
            {
                HyperconeData coneData = em.GetComponentData<HyperconeData>(entity);

                coneData.usePerspective = toggle;

                em.SetComponentData(entity, coneData);
            }
        }

        }
}

