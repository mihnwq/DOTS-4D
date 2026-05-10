using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using UnityEngine.UI;
using TMPro;

public class DOTSTextManager : MonoBehaviour
{
    private EntityManager em;

    private int currentID;
    private int lastID;

    [SerializeField]
    TextMeshProUGUI name;

    [SerializeField]
    TextMeshProUGUI atribute1;

    [SerializeField]
    TextMeshProUGUI atribute2;

    [SerializeField]
    TextMeshProUGUI atribute3;

    public void Start()
    {
        em = World.DefaultGameObjectInjectionWorld.EntityManager;

        currentID = 1;
        lastID = 1;

        ChangeText(currentID);
    }

    private void Update()
    {
        GetCurrentEntityID();

        if (currentID != lastID)
            ChangeText(currentID);
    }

    private void GetCurrentEntityID()
    {
        EntityQuery query = em.CreateEntityQuery(typeof(ObjectIDData));
        NativeArray<Entity> entities = query.ToEntityArray(Allocator.Temp);
        foreach (Entity entity in entities)
        {
            ObjectIDData data = em.GetComponentData<ObjectIDData>(entity);
            currentID = data.id;
        }

    }

    public void ChangeText(int id)
    {
            switch(id)
            {
                case 1:
                name.text = "Hypercube - Optimized";

                atribute1.text = "Size";
                atribute2.text = "WSlice";
                atribute3.text = "SliceThickness";

                    break;
                case 2:
                name.text = "Hypercone - Optimized";

                atribute1.text = "WSlices";
                atribute2.text = "ConeSlope";
                atribute3.text = "WMax";
                break;
                case 3:
                name.text = "Hypersphere - Optimized";

                atribute1.text = "ThetaSegments";
                atribute2.text = "PhiSegments";
                atribute3.text = "PsiSegments";
                break;
                case 4:
                name.text = "Hypercylinder - Optimized";

                atribute1.text = "N_h";
                atribute2.text = "N_phi";
                atribute3.text = "N_theta";
                break;
            }

          lastID = id;
        }

        
    }
