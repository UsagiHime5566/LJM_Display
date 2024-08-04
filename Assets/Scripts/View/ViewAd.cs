using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using Doozy.Runtime.UIManager.Containers;
using Doozy.Runtime.Signals;
using System.IO;

public class ViewAd : MonoBehaviour
{
    public UIView VW_Self;
    public Button BTN_Touch;
    public float TimeToTitle = 180;

    [Header("Runtime")]
    [SerializeField] float remainTimeToAd = 0;

    void Start()
    {
        BTN_Touch.onClick.AddListener(() => {
            StopAD();
        });

        VW_Self.OnShowCallback.Event.AddListener(OnADShow);
        
        ESAdManager.instance.OnAdEnd += x => {
            LJMSignalManager.instance.ToViewHome();
        };

        //StartCoroutine(LoopCheckViewState());
    }

    void OnADShow(){
        ESAdManager.instance.PlayClip(0);
        remainTimeToAd = TimeToTitle;
    }

    public void StopAD(){
        ESAdManager.instance.StopAD();
        LJMSignalManager.instance.ToViewHome();
    }

    public void SoftStop(){
        ESAdManager.instance.RequireStopAD(() => {
            LJMSignalManager.instance.ToViewHome();
        });
    }

    IEnumerator LoopCheckViewState(){
        while(true){
            if(remainTimeToAd > 0 && VW_Self.isVisible) {
                remainTimeToAd--;
                if(remainTimeToAd <= 0){
                    SoftStop();
                }
            }

            if(TimeArrange.instance.currentViewState == TimeArrange.ViewState.Game && VW_Self.isVisible){
                SoftStop();
            }
            yield return new WaitForSeconds(1);
        }
    }
}
