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
        VW_Self.OnHideCallback.Event.AddListener(() => {
            ESAdManager.instance.StopAD();
        });
        
        ESAdManager.instance.OnAdEnd += x => {
            LJMSignalManager.instance.ToViewHomeFromAd();
        };

        //StartCoroutine(LoopCheckViewState());
    }

    void OnADShow(){
        ESAdManager.instance.PlayClip(0);
        remainTimeToAd = TimeToTitle;

        BGMPlayer.instance.Stop();
    }

    public void StopAD(){
        ESAdManager.instance.StopAD();
        LJMSignalManager.instance.ToViewHomeFromAd();
    }

    public void SoftStop(){
        ESAdManager.instance.RequireStopAD(() => {
            LJMSignalManager.instance.ToViewHomeFromAd();
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
