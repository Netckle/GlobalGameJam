using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyTimer : MonoBehaviour
{
    public bool DestroyOnStart = true;
    public float DestroyTime = 3.0f;

    private void Start()
    {
        if(DestroyOnStart)
        {
            StopAllCoroutines();
            StartCoroutine(Cor_DestoryTimer(DestroyTime));
        }
    }

    public void StartTimer()
    {
        StopAllCoroutines();
        StartCoroutine(Cor_DestoryTimer(DestroyTime));
    }

    IEnumerator Cor_DestoryTimer(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}
