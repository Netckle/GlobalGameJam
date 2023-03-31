using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMBehavior : MonoBehaviour
{
    private static BGMBehavior instance;
    public static BGMBehavior Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        if(instance == null) instance = this;
        else
        {
            DestroyThis();
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void DestroyThis()
    {
        Destroy(gameObject);
    }
}
