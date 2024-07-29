using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Doozy.Runtime.UIManager.Components;
using Doozy.Runtime.UIManager.Containers;
using DG.Tweening;

public class ViewAffiliated : MonoBehaviour
{
    public UIView VW_Self;
    public float waitToHome = 15;
    void Start()
    {
        VW_Self.OnShowCallback.Event.AddListener(() => {
            StartCoroutine(StayView());
        });
    }

    IEnumerator StayView(){
        yield return new WaitForSeconds(waitToHome);
        LJMSignalManager.instance.ToViewHome();
    }
}
