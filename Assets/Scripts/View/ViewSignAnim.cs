using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using Doozy.Runtime.UIManager.Components;
using Doozy.Runtime.UIManager.Containers;
using DG.Tweening;

public class ViewSignAnim : MonoBehaviour
{
    public UIView VW_Self;
    public PainterMemory painterMemory;

    [Header("樹葉動畫")]
    public List<LeafAnimInfo> Leaf_infos;
    public VideoPlayer VP_Leaf;
    public CanvasGroup Draw_LeafAlpha;
    public LeafStep leafStep; 

    public float leafEntryTo = 3;
    public float leafStayEnd = 10;
    

    [Header("參數")]
    public float waitToNextA = 15;
    public float waitToNextB = 15;
    void Start()
    {
        VW_Self.OnShowCallback.Event.AddListener(() => {
            Draw_LeafAlpha.alpha = 0;

            VP_Leaf.time = 0;
            VP_Leaf.Play();

            StartCoroutine(StartCommand(0.1f, () => {
                leafStep = LeafStep.LeafEntry;
            }));
        });

        // painterMemory.OnReplayFinished += () => {
        //     IntoLeafLeave();
        // };

        StartCoroutine(LeafAnimation());
    }

    IEnumerator StartCommand(float delay, System.Action callback){
        yield return new WaitForSeconds(delay);
        callback?.Invoke();
    }

    IEnumerator LeafAnimation(){
        WaitForSeconds wait = new WaitForSeconds(0.1f);
        while(true){
            if(leafStep == LeafStep.LeafEntry){
                if(VP_Leaf.time > leafEntryTo){
                    VP_Leaf.Pause();
                    IntoReplayDraw();
                    leafStep = LeafStep.LeafSign;
                }
            }

            if(leafStep == LeafStep.LeafLeaving){
                if(VP_Leaf.time > leafStayEnd){
                    VP_Leaf.Pause();
                    IntoStayEnd();
                    leafStep = LeafStep.LeafStayEnd;
                }
            }

            yield return wait;
        }
    }

    void IntoReplayDraw(){
        Draw_LeafAlpha.DOFade(1, 1);
        painterMemory.Replay();
        StartCoroutine(StartCommand(waitToNextA, IntoLeafLeave));
    }

    void IntoLeafLeave(){
        leafStep = LeafStep.LeafLeaving;
        Draw_LeafAlpha.DOFade(0, 1);
        VP_Leaf.Play();
    }

    void IntoStayEnd(){
        StartCoroutine(StartCommand(waitToNextB, LeaveView));
    }

    void LeaveView(){
        LJMSignalManager.instance.ToViewHome();
    }

    // IEnumerator StayView(){
    //     yield return new WaitForSeconds(waitToNextA);

    //     Draw_LeafAlpha.DOFade(0, scaleSecond).SetEase(easeType);
    //     IMG_TipText.DOFade(0, scaleSecond);

    //     yield return new WaitForSeconds(scaleSecond);

    //     IMG_FinText.DOFade(1, 1);

    //     yield return new WaitForSeconds(waitToNextB);

    //     LJMSignalManager.instance.ToViewHome();
    // }
}

[System.Serializable]
public class LeafAnimInfo
{
    public string clipName;
    public float clipStart;
    public float clipStay;
}

public enum LeafStep
{
    LeafEntry = 0,
    LeafSign = 1,
    LeafLeaving = 2,
    LeafStayEnd = 3,
}