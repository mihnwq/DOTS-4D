using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;
using TMPro;
using System.Collections.Generic;

public class DOTSInputField : MonoBehaviour
{

    private EntityManager em;

    [SerializeField]
    List<TMP_InputField> inputFields;

    private string name = " ";

    [SerializeField]
    TextMeshProUGUI textObj;

    private float deformation, oscilationA, oscilationS;


    private void Start()
    {
        em = World.DefaultGameObjectInjectionWorld.EntityManager;
    }

    private void ChangeInputVariables(float deformation, float oscilationS, float oscilationA)
    {
        EntityQuery query = em.CreateEntityQuery(typeof(Object4DData));
        NativeArray<Entity> entities = query.ToEntityArray(Allocator.Temp);

        foreach (Entity entity in entities)
        {
            Object3Dto4DData data = em.GetComponentData<Object3Dto4DData>(entity);

            data.wDeformStrength = deformation;
            data.wOscillationSpeed = oscilationS;
            data.wOscillationAmplitude = oscilationA;

            em.SetComponentData(entity, data);

        }

        entities.Dispose();
    }

    public void SetDeformStrength()
    {
        deformation = float.Parse(inputFields[0].text);
        ChangeInputVariables(deformation, oscilationS, oscilationA);
    }

    public void SetOscillationAmplitude()
    {
        oscilationA = float.Parse(inputFields[1].text);
        ChangeInputVariables(deformation, oscilationS, oscilationA);
    }

    public void SetOscillationSpeed()
    {
        oscilationS = float.Parse(inputFields[2].text);
        ChangeInputVariables(deformation, oscilationS, oscilationA);
    }
}

