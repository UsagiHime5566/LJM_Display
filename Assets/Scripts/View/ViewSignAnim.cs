using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Doozy.Runtime.UIManager.Components;
using Doozy.Runtime.UIManager.Containers;
using DG.Tweening;

public class ViewSignAnim : MonoBehaviour
{
    public UIView VW_Self;
    public RectTransform Draw_Leaf;
    public CanvasGroup Draw_LeafAlpha;
    public CanvasGroup IMG_TipText;
    public CanvasGroup IMG_FinText;
    public Ease easeType;

    [Header("參數")]
    public float signSecond = 3;
    public float scaleSecond = 2;
    public float waitToNextA = 15;
    public float waitToNextB = 15;
    void Start()
    {
        VW_Self.OnShowCallback.Event.AddListener(() => {
            Draw_Leaf.localScale = Vector3.one;
            Draw_LeafAlpha.alpha = 1;
            IMG_TipText.alpha = 1;
            IMG_FinText.alpha = 0;

            StartCoroutine(StayView());
        });
    }

    IEnumerator StayView(){
        yield return new WaitForSeconds(waitToNextA);

        Draw_Leaf.DOScale(0, scaleSecond).SetEase(easeType);
        Draw_LeafAlpha.DOFade(0, scaleSecond).SetEase(easeType);
        IMG_TipText.DOFade(0, scaleSecond);

        yield return new WaitForSeconds(scaleSecond);

        IMG_FinText.DOFade(1, 1);

        yield return new WaitForSeconds(waitToNextB);

        LJMSignalManager.instance.ToViewHome();
    }
}
