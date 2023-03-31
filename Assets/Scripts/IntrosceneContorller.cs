using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntrosceneContorller : MonoBehaviour
{
    public void LoadSene(string SceneName)
    {
        SceneLoader.Instance.StartLoadingScene(SceneName);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
