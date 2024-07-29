using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class PainterCanva2Controller2 : MonoBehaviour {

    public Camera cameraMain;
    public RawImage rawImg;
    public Painter painterCanvas;
    public Material drawOtherRtMat;
    [Range(0f,1f)]
    public float penAlpha = 1f;
    public Color penColor = Color.white;

    private bool _mouseIsDown = false;

    private RenderTexture _rt;
	// Use this for initialization
	void Start () {
        painterCanvas.gameObject.SetActive(true);
        _rt = new RenderTexture(painterCanvas.renderTexWidth, painterCanvas.renderTexHeight, 0, RenderTextureFormat.ARGB32);
        _rt.useMipMap = false;
        if (rawImg.texture != null)
        {
            Graphics.SetRenderTarget (_rt);
            Graphics.Blit(rawImg.texture,_rt);
            RenderTexture.active = null;
        }

        //set mask
        //drawOtherRtMat.SetTexture("_MaskTex",rawImg.texture);
        //painterCanvas.canvasMat.SetTexture("_MaskTex",rawImg.texture);

        //use rendertexture to replace.
        rawImg.texture = _rt;
	}
	
	// Update is called once per frame
	void Update () {
        penColor.a = 1;
        if(Input.GetMouseButtonDown(0))
        {
            _mouseIsDown = true;
        }
        if(_mouseIsDown && Input.GetMouseButton(0))
        {
            Vector2 pos;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(painterCanvas.transform as RectTransform, Input.mousePosition, cameraMain, out pos))
            {
                painterCanvas.canvasMat.SetFloat("_Alpha",penAlpha);
                painterCanvas.penColor = penColor;
                painterCanvas.Drawing(pos,cameraMain,painterCanvas.renderTexture,false,true);
            }
        }
        if(Input.GetMouseButtonUp(0))
        {
            _mouseIsDown = false;
            drawOtherRtMat.SetFloat("_Alpha",penAlpha);
            //drawOtherRtMat.SetVector("_Color",penColor);
                //painterCanvas.DrawRT2OtherRT(painterCanvas.renderTexture,_rt,drawOtherRtMat);
                //painterCanvas.ClearCanvas();
            painterCanvas.EndDraw();
        }
	}

    [EasyButtons.Button]
    public void SavePNG(){
        SavePng(_rt);
    }

    public void SavePng(RenderTexture RenderTextureRef)
    {
        Texture2D tex = new Texture2D(RenderTextureRef.width, RenderTextureRef.height, TextureFormat.RGBA32, false);
        RenderTexture.active = RenderTextureRef;
        tex.ReadPixels(new Rect(0, 0, RenderTextureRef.width, RenderTextureRef.height), 0, 0);
        tex.Apply();

        // Encode texture into PNG
        byte[] bytes = tex.EncodeToPNG();
        Object.Destroy(tex);

        //Write to a file in the project folder
        string path = Application.dataPath + "/../SavedPaint.png";
        File.WriteAllBytes(path, bytes);

        Debug.Log(bytes.Length/1024  + "Kb was saved as: " + path);
    }
}
