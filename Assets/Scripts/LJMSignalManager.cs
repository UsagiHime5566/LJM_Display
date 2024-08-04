using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doozy.Runtime.Signals;

public class LJMSignalManager : HimeLib.SingletonMono<LJMSignalManager>
{
    public string Category = "LJM";
    public string ToSign = "ToSign";
    public string ToHome = "ToHome";
    public string ToAd = "ToAd";
    public string ToAffiliated = "ToAffiliated";
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)){
            ToViewSignAnim();
        }

        if(Input.GetKeyDown(KeyCode.A)){
            ToViewAd();
        }
    }

    public void ToViewAffiliated(){
        SignalsService.SendSignal(Category, ToAffiliated);
    }

    public void ToViewSignAnim(){
        SignalsService.SendSignal(Category, ToSign);
    }

    public void ToViewHome(){
        SignalsService.SendSignal(Category, ToHome);
    }

    public void ToViewAd(){
        SignalsService.SendSignal(Category, ToAd);
    }
}
