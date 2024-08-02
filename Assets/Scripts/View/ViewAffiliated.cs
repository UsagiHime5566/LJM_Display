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
    public SignatureManager signatureManager;
    public float waitToHome = 15;

    [SerializeField] float remainTime = 15;
    void Start()
    {
        VW_Self.OnShowCallback.Event.AddListener(() => {
            remainTime = waitToHome;
            signatureManager.VisibleSignature(true);
        });

        VW_Self.OnHideCallback.Event.AddListener(() => {
            signatureManager.VisibleSignature(false);
        });

        StartCoroutine(StayView());
    }

    IEnumerator StayView(){
        while(true){
            yield return new WaitForSeconds(1);
            if(remainTime>= 0){
                remainTime -= 1;

                if(remainTime < 0){
                    if(VW_Self.isVisible){
                        LJMSignalManager.instance.ToViewHome();
                    }
                }
            }
        }
    }
}
