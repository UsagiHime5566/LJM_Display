using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HimeLib;
using System.IO;

public class TimeArrange : SingletonMono<TimeArrange>
{
    [System.Serializable]
    public enum AdMode
    {
        No = 0,
        OnlyAd = 1,
        OnlyGame = 2,
        MoreAd = 3,
        MoreGame = 4,
    }

    [System.Serializable]
    public enum ViewState
    {
        Game = 0,
        Ad = 1
    }

    [System.Serializable]
    public class TimeFlagSetting
    {
        public AdMode adMode;
        public TimeRange timeRange;
    }

    [SerializeField] bool useDebug = false;

    public Text TXT_Show;
    public AdMode currentAdMode;
    public ViewState currentViewState;

    public List<TimeFlagSetting> flagSettings = new List<TimeFlagSetting>();

    void Start()
    {
        StartTimeArrange();

        #if !UNITY_EDITOR
            useDebug = false;
        #endif

        var testmode = string.Format("{0}/../{1}", Application.dataPath, "admode.txt");
        if(File.Exists(testmode)){
            useDebug = true;
        }
    }

    public void StartTimeArrange(){
        StartCoroutine(TimeTick());
        //Debug.Log("Ready to Play Stages");
    }

    IEnumerator TimeTick(){
        while(true){
            // 獲取當前時間
            DateTime currentTime = DateTime.Now;

            // 遍歷所有Flag設置，找到當前時間對應的Flag
            foreach (var setting in flagSettings)
            {
                if (setting.timeRange.IsTimeInRange(currentTime))
                {
                    // if(currentAdMode != setting.adMode){
                    //     Debug.Log($"Current Time Flag: {currentAdMode} {currentViewState}");
                    // }

                    currentAdMode = setting.adMode;

                    currentViewState = NewViewByMode(currentAdMode, currentTime);

                    if(useDebug){
                        currentAdMode = AdMode.MoreGame;
                        currentViewState = ViewState.Ad;
                    }

                    if(TXT_Show) TXT_Show.text = $"{currentAdMode} - {currentViewState}";
                    
                    goto NextLoop;
                }
            }

            Debug.LogWarning("No time flag matched.");

            NextLoop:
            yield return new WaitForSeconds(1);
        }
    }

    ViewState NewViewByMode(AdMode mode, DateTime ctime){
        if(mode == AdMode.OnlyAd){
            return ViewState.Ad;
        }
        if(mode == AdMode.MoreGame){
            if(ctime.Minute < 50) return ViewState.Game;
            else return ViewState.Ad;
        }
        return ViewState.Game;
    }
}

[System.Serializable]
public class TimeRange
{
    public string startTimeString; // 開始時間的字符串表示（格式：hh:mm）
    public string endTimeString; // 結束時間的字符串表示（格式：hh:mm）

    [HideInInspector]
    public TimeSpan startTime; // 開始時間的TimeSpan對象
    
    [HideInInspector]
    public TimeSpan endTime; // 結束時間的TimeSpan對象

    bool initialized = false;

    public TimeRange(string startTimeString, string endTimeString)
    {
        this.startTimeString = startTimeString;
        this.endTimeString = endTimeString;
    }

    // 檢查指定的時間是否在範圍內
    public bool IsTimeInRange(DateTime time)
    {
        InitializeTimes();
        TimeSpan currentTime = time.TimeOfDay;

        if (endTime <= startTime)
        {
            // 當結束時間小於等於開始時間時，表示跨越了一天的時間範圍
            // 因此，檢查當前時間是否在開始時間之後或者在結束時間之前即可
            return (currentTime >= startTime || currentTime <= endTime);
        }
        else
        {
            // 檢查當前時間是否在開始時間和結束時間之間
            return (currentTime >= startTime && currentTime <= endTime);
        }
    }

    void InitializeTimes()
    {
        if (initialized == false)
        {
            startTime = TimeSpan.Parse(startTimeString);
            endTime = TimeSpan.Parse(endTimeString);
            initialized = true;
        }
    }
}
