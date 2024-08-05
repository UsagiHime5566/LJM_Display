using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Doozy.Runtime.Signals;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
using OscJack;
using DG.Tweening;

public class EventMode : MonoBehaviour
{
    public string receiverIPAddress;
    public int eventPort;
    public CanvasGroup OverlaySay;
    public Camera mainCamera;
    public InputField INP_SelfIP;


    public Painter painter;
    public PaintLight paintLight;
    public InputField INP_Size;

    void Start(){
        INP_SelfIP.onValueChanged.AddListener(x => {
            receiverIPAddress = x;
            SystemConfig.Instance.SaveData("selfIP", x);
        });
        INP_SelfIP.text = SystemConfig.Instance.GetData<string>("selfIP", "192.168.11.1");


        INP_Size?.onValueChanged.AddListener(x => {
            if(float.TryParse(x, out float val)){
                paintLight.sizeValues[0] = val;
                painter.brushScale = val;
                SystemConfig.Instance.SaveData("ssize", x);
            }
        });
        if(INP_Size) INP_Size.text = SystemConfig.Instance.GetData<string>("ssize", "0.5");
    }

    public void UserSigned(){
        LJMSignalManager.instance.ToViewAffiliated();
    }

    public void VisibleOverlay(bool val){
        if(val){
            OverlaySay.DOFade(1, 0.7f);
            mainCamera.transform.position = new Vector3(-100, mainCamera.transform.position.y, mainCamera.transform.position.z);
        } else {
            OverlaySay.DOFade(0, 0.7f);
            mainCamera.transform.position = new Vector3(0, mainCamera.transform.position.y, mainCamera.transform.position.z);
        }
    }

    public void BrocastOSC(string msg){
        string[] ipParts = receiverIPAddress.Split('.');
        string subnet = ipParts[0] + "." + ipParts[1] + "." + ipParts[2] + ".";
        for (int i = 0; i <= 255; i++)
        {
            string fullIp = subnet + i;
            using (var client = new OscClient(fullIp, eventPort))
            {
                client.Send(msg);
            }
        }
    }
}
