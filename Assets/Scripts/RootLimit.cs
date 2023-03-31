using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RootLimit : MonoBehaviour
{
    private int limitCount;
    public int LimitCount { get { return limitCount; } }

    public TextMeshProUGUI tmp;

    public void OpenUI()
    {
        tmp.transform.parent.gameObject.SetActive(true);
        tmp.text = limitCount.ToString();
    }

    public void CloseUI()
    {
        tmp.transform.parent.gameObject.SetActive(false);
        tmp.text = string.Empty;
    }
}
