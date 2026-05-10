using UnityEngine;
using Unity.Entities;
using Unity.Collections;

public class DOTSObjectManager : MonoBehaviour
{
    private EntityManager em;
    private int starterIndex;
    private int lastIndex;
    private int currentIndex;


   public GameObject cobject;

    void Start()
    {
        em = World.DefaultGameObjectInjectionWorld.EntityManager;
        starterIndex = 1;
        lastIndex = 4;
        currentIndex = starterIndex;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.RightArrow))
        {
      
            GetIndex(1);
            SwitchToObject(currentIndex);
           
        }

        if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            GetIndex(-1);
            SwitchToObject(currentIndex);
        }

     //   Debug.Log(currentIndex);
    }


    public void SwitchToObject(int targetID)
    {
       
        EntityQuery query = new EntityQueryBuilder(Allocator.Temp)
            .WithAll<ObjectIDData>()
            .WithOptions(EntityQueryOptions.IncludeDisabledEntities) 
            .Build(em);

    
        NativeArray<Entity> entities = query.ToEntityArray(Allocator.Temp);

       
        foreach (Entity entity in entities)
        {
            ObjectIDData data = em.GetComponentData<ObjectIDData>(entity);

            if (data.id == targetID)
            {
               
                if (!em.IsEnabled(entity))
                {
                    em.SetEnabled(entity, true);
               //     Debug.Log("Object started with ID: {data.id}");
                }
            }
            else
            {
                
                if (em.IsEnabled(entity))
                {
                    em.SetEnabled(entity, false);
                }
            }
        }

      
        entities.Dispose();
    }

    private void GetIndex(int next)
    {
        currentIndex += next;

        if (currentIndex > lastIndex)
            currentIndex = lastIndex;
        else if (currentIndex < starterIndex)
            currentIndex = starterIndex;

    }
}