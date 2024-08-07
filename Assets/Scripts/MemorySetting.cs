using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MemorySetting : MonoBehaviour
{
    [Header("畫圖速度")]
    public PainterMemory painterMemory;
    public InputField INP_DrawDelay;

    [Header("畫圖筆畫")]
    public Painter painter;
    public PaintLight paintLight;
    public InputField INP_Size;

    [Header("廣告路徑")]
    public InputField INP_UrlAD;
    void Start()
    {
        Application.targetFrameRate = 120;

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

        INP_Size?.onValueChanged.AddListener(x => {
            if(float.TryParse(x, out float val)){
                paintLight.sizeValues[0] = val;
                painter.brushScale = val;
                SystemConfig.Instance.SaveData("ssize", x);
            }
        });
        if(INP_Size) INP_Size.text = SystemConfig.Instance.GetData<string>("ssize", "0.25");
    }
}
