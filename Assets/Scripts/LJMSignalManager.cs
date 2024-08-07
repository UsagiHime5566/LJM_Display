using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doozy.Runtime.Signals;
using UnityEngine.SceneManagement;

public class LJMSignalManager : HimeLib.SingletonMono<LJMSignalManager>
{
    public string Category = "LJM";
    public string ToSign = "ToSign";
    public string ToHome = "ToHome";
    public string AdToHome = "AdToHome";
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

        if(Input.GetKeyDown(KeyCode.V)){
            ToViewAffiliated();
        }

        if(Input.GetKeyDown(KeyCode.B)){
            ToViewHome();
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

    public void ToViewHomeFromAd(){
        SignalsService.SendSignal(Category, AdToHome);
    }

    public void ToViewAd(){
        SignalsService.SendSignal(Category, ToAd);
    }

    public void ToMain(){
        SceneManager.LoadScene("Main");
    }

    public void ToEvent(){
        SceneManager.LoadScene("MainEvent");
    }
}
