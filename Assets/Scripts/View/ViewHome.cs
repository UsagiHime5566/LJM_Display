using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using Doozy.Runtime.UIManager.Components;
using Doozy.Runtime.UIManager.Containers;
using DG.Tweening;

public class ViewHome : MonoBehaviour
{
    public UIView VW_Self;

    [Header("介紹電影相關")]
    public VideoPlayer ForegroundMovie;
    public VideoPlayer IntroMovie;
    public List<float> speedupTime;
    public float speedupRange = 5;
    public CanvasGroup IntroVisible;

    [Header("宣言相關")]
    public RectTransform Rect_Preword;
    public float sizeTime = 7;
    [SerializeField] float initRect_Width = 961;
    [SerializeField] float initRect_Height = 545;
    void Start()
    {
        initRect_Width = Rect_Preword.sizeDelta.x;
        initRect_Height = Rect_Preword.sizeDelta.y;

        VW_Self.OnShowCallback.Event.AddListener(() => {
            IntroVisible.alpha = 1;
            SetHeight(Rect_Preword, 0);
        });

        StartCoroutine(CheckMovieTime());
    }

    IEnumerator CheckMovieTime(){
        WaitForSeconds wait = new WaitForSeconds(0.1f);
        while(true){
            if(IntroMovie.time > speedupTime[0] && IntroMovie.time < speedupTime[0] + speedupRange){
                ForegroundMovie.playbackSpeed = 2;
            } else
            if(IntroMovie.time > speedupTime[1] && IntroMovie.time < speedupTime[1] + speedupRange){
                ForegroundMovie.playbackSpeed = 2;
            } else
            if(IntroMovie.time > speedupTime[2] && IntroMovie.time < speedupTime[2] + speedupRange){
                ForegroundMovie.playbackSpeed = 2;
            } else {
                ForegroundMovie.playbackSpeed = 1;
            }

            yield return wait;
        }
    }

    public void StartSigning(){
        Debug.Log("Recieve OSC Starting! - " + System.DateTime.Now);
        IntroVisible.DOFade(0, 1);
        Rect_Preword.DOSizeDelta(new Vector2(initRect_Width, initRect_Height), sizeTime);
    }

    public void SetHeight(RectTransform rectTransform, float height)
    {
        // 保持现有的锚点、锚定位置和宽度
        Vector2 sizeDelta = rectTransform.sizeDelta;
        sizeDelta.y = height;
        rectTransform.sizeDelta = sizeDelta;
    }
}
