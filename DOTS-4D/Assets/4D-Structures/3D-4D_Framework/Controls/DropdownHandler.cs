using UnityEngine;
using TMPro; // if using TextMeshPro dropdown
using System.Windows.Forms;
using Unity.VisualScripting;
using System.Collections;
using System.Collections.Generic;

public class DropdownHandler : MonoBehaviour
{
    public TMP_Dropdown dropdown;

    [SerializeField]
    private GameObject selectButton;

    [SerializeField]
    private List<GameObject> _3D_Objects;

    private Mesh currentObjectMesh;


    private void Start()
    {
        dropdown.SetValueWithoutNotify(4);
        dropdown.onValueChanged.AddListener(OnDropdownChanged);
    }

    public void OnDropdownChanged(int index)
    {
        //  Debug.Log("Selected index: " + index);

        currentObjectMesh = MeshExtractor.GetMeshFromGameObject(_3D_Objects[index]);

        DOTSMeshSwapper.instance.testMesh = currentObjectMesh;

        selectButton.SetActive(true);
        dropdown.Hide();
        dropdown.GameObject().SetActive(false);
    }

    

    public void OnDropdownSelection()
    {
       
        StartCoroutine(OpenDropdownNextFrame());
    }

    IEnumerator OpenDropdownNextFrame()
    {
        dropdown.gameObject.SetActive(true);
        selectButton.SetActive(false);

        yield return null;   

        dropdown.Show();     
    }
}