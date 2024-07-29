using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Doozy.Runtime.UIManager.Components;
using Doozy.Runtime.UIManager.Containers;
using DG.Tweening;

public class ViewHome : MonoBehaviour
{
    public UIView VW_Self;

    public List<Image> IMG_Titles;
    void Start()
    {
        VW_Self.OnShowCallback.Event.AddListener(() => {
            IMG_Titles[0].DOFade(1, 0);
            IMG_Titles[1].DOFade(0, 0);
        });
    }

    public void StartSigning(){
        Debug.Log("Recieve OSC Starting! - " + System.DateTime.Now);
        IMG_Titles[0].DOFade(0, 0.5f);
        IMG_Titles[1].DOFade(1, 0.5f);
    }

    IEnumerator LoopFade(){
        int index = 0;
        while(true){
            
            IMG_Titles[index].DOFade(0, 0.5f);
            index = (index + 1) % IMG_Titles.Count;
            IMG_Titles[index].DOFade(1, 0.5f);
            yield return new WaitForSeconds(5);
        }
    }
}
