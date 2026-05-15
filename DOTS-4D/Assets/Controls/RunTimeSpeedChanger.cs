using Unity.Entities;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RuntimeSpeedChanger : MonoBehaviour
{
    private EntityManager em;
    private float lastValue;
    private float speed;

    [SerializeField]
    float speedMultiplier;

    [SerializeField]
    Scrollbar rotationSpeed;

    void Start()
    {
        em = World.DefaultGameObjectInjectionWorld.EntityManager;
        rotationSpeed.onValueChanged.AddListener(OnScroll);

        

        OnScroll(0f);
    }



    public void ChangeSpeed(float newSpeed)
    {
        EntityQuery query = em.CreateEntityQuery(typeof(Object4DData));
        NativeArray<Entity> entities = query.ToEntityArray(Allocator.Temp);

        foreach (Entity entity in entities)
        {
           
            Object4DData data = em.GetComponentData<Object4DData>(entity);
        
            data.rotationSpeed = newSpeed;

            em.SetComponentData(entity, data);
        }

 
        entities.Dispose();
    }
    /*
     * if (em.HasComponent<HypercubeRawData>(entity))
          {
     */

    private float GetMaximumSpeed()
    {
        EntityQuery query = em.CreateEntityQuery(typeof(Object4DData));
        NativeArray<Entity> entities = query.ToEntityArray(Allocator.Temp);

        foreach (Entity entity in entities)
        {
            if (em.HasComponent<Object3Dto4DData>(entity))
            {
                entities.Dispose();
                return 10f;
            }

            

        }

        entities.Dispose();

        return 1.5f;
    }

        public void OnScroll(float value)
    {
        float delta = value - lastValue;
      //  Debug.Log(value);

        if (Mathf.Abs(delta) > 0.0001f)
        {

            speed += delta * speedMultiplier;


            speed = Mathf.Clamp(speed, 0.0f, GetMaximumSpeed());

     

           
            ChangeSpeed(speed);

        }

        lastValue = value;
    }
}