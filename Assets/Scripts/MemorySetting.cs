using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MemorySetting : MonoBehaviour
{
    [Header("畫圖速度")]
    public PainterMemory painterMemory;
    public InputField INP_DrawDelay;

    [Header("廣告路徑")]
    public InputField INP_UrlAD;
    void Start()
    {
        INP_DrawDelay.onValueChanged.AddListener(x => {
            if(int.TryParse(x, out int val)){
                painterMemory.replayStepDelay = val;
                SystemConfig.Instance.SaveData("replayDelay", x);
            }
        });
        INP_DrawDelay.text = SystemConfig.Instance.GetData<string>("replayDelay", "10");

        INP_UrlAD.onValueChanged.AddListener(x => {
            ESAdManager.instance.adPaths[0] = x;
            SystemConfig.Instance.SaveData("UrlAD", x);
        });
        INP_UrlAD.text = SystemConfig.Instance.GetData<string>("UrlAD", "");
    }
}
