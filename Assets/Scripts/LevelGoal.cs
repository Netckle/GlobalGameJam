using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGoal : MonoBehaviour
{
    public string TargetSceneName;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            SceneLoader.Instance.StartLoadingScene(TargetSceneName);
        }
    }
}
