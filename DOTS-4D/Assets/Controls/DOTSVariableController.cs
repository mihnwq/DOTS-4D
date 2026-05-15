using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using UnityEngine.UI;

public class DOTSVariableController : MonoBehaviour
{

    private EntityManager em;

    private float value1, value2, value3;

    [SerializeField]
    Scrollbar atribute1;

    [SerializeField]
    Scrollbar atribute2;

    [SerializeField]
    Scrollbar atribute3;

    void Start()
    {
        em = World.DefaultGameObjectInjectionWorld.EntityManager;

        atribute1.onValueChanged.AddListener(OnValueChangedA1);
        atribute2.onValueChanged.AddListener(OnValueChangedA2);
        atribute3.onValueChanged.AddListener(OnValueChangedA3);
    }

  

    public void ApplyVariablesToDOTS(float value1 , float value2, float value3)
    {
     
        EntityQuery query = em.CreateEntityQuery(typeof(ObjectIDData));
        NativeArray<Entity> entities = query.ToEntityArray(Allocator.Temp);

        foreach (Entity entity in entities)
        {
           
                
         if (em.HasComponent<HypercubeRawData>(entity))
          {
              
          HypercubeRawData cubeData = em.GetComponentData<HypercubeRawData>(entity);


            cubeData.size = Mathf.Clamp(value1 , 1f, 3f);
            cubeData.wSlice = Mathf.Clamp(value2 , -2f, 2f);
            cubeData.sliceThickness = Mathf.Clamp(value3 , 2 , 7);

                   
           em.SetComponentData(entity, cubeData);
          }else if (em.HasComponent<HyperconeData>(entity))
          {
                HyperconeData coneData = em.GetComponentData<HyperconeData>(entity);

                float slope = Mathf.Clamp(Mathf.Round(value2 / 100 * 3),0.8f,3);

                float wmax = Mathf.Clamp(Mathf.Round(value3 / 100 * 5),2,5);

                coneData.wSlices = Mathf.Clamp((int)value1 , 2 , 200);

                coneData.coneSlope = slope;

                coneData.wMax = wmax;

                /*  Debug.Log("Slices + " + Mathf.Clamp((int)value1, 15, 100)); 
                  Debug.Log("Slope + " + Mathf.Clamp((int)value2, 15, 10));
                  Debug.Log("WMax +" + Mathf.Clamp((int)value3, 10, 10));*/

                Debug.Log(value1);
                Debug.Log(value2);
                Debug.Log(value3);

                em.SetComponentData(entity, coneData);
          }else if(em.HasComponent<HypersphereData>(entity))
            {
                HypersphereData sphereData = em.GetComponentData<HypersphereData>(entity);

                sphereData.thetaSegments = Mathf.Clamp((int)value1, 10, 50);
                sphereData.phiSegments = Mathf.Clamp((int)value2, 10, 50);
                sphereData.psiSegments = Mathf.Clamp((int)value3, 10, 50);

                em.SetComponentData(entity, sphereData);
            }else
            {
                HyperCylinderData cyclinderData = em.GetComponentData<HyperCylinderData>(entity);

                cyclinderData.n_h = Mathf.Clamp((int)value1 , 12 , 50);
                cyclinderData.n_phi = Mathf.Clamp((int)value1, 6, 40);
                cyclinderData.n_theta = Mathf.Clamp((int)value1, 4, 20);

                em.SetComponentData(entity, cyclinderData);
            }

               
                break;
            
        }

       
        entities.Dispose();
    }

    public void OnValueChangedA1(float value)
    {
        value *= 100;

        value1 = value;

        ApplyVariablesToDOTS(value1, value2, value3);
    }

    public void OnValueChangedA2(float value)
    {
        value *= 100;

        value2 = value;

        ApplyVariablesToDOTS(value1, value2, value3);
    }

    public void OnValueChangedA3(float value)
    {
        value *= 100;

        value3 = value;

        ApplyVariablesToDOTS(value1, value2, value3);
    }
}