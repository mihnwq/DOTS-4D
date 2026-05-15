using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{

    public void OnClickSimulation()
    {
        SceneManager.LoadScene(2);
    }

    public void OnClickVisualisation()
    {
        SceneManager.LoadScene(1);
    }

    public void OnClickMenu()
    {
        SceneManager.LoadScene(0);
    }

}

