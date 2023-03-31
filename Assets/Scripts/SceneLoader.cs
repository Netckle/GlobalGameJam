using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Management.Instrumentation;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private static SceneLoader instance;
    public static SceneLoader Instance
    {
        get 
        { 
            return instance;
        }
    }

    [SerializeField] private DOTweenAnimation CoverAnim;

    bool isLoading = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    public void StartLoadingScene(string SceneName)
    {
        if (isLoading) return;

        StopAllCoroutines();
        StartCoroutine(Cor_LoadingSeq(SceneName));
    }

    IEnumerator Cor_LoadingSeq(string SceneName)
    {
        isLoading = true;
        CoverAnim.DORestartById("Loading_Close");
        var tw = CoverAnim.tween;
        yield return tw.WaitForCompletion();

        var async = SceneManager.LoadSceneAsync(SceneName);

        yield return new WaitUntil(() => async.isDone);
        yield return new WaitForEndOfFrame();

        CoverAnim.DORestartById("Loading_Open");
        yield return tw.WaitForCompletion();
        isLoading = false;
    }
}
