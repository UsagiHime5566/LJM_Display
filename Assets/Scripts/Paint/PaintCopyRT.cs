using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HimeLib;

public class PaintCopyRT : MonoBehaviour
{
    [Header("Setting")]
	public ToggleGroup brushBar;
    public List<Toggle> brushToggles;
	public ToggleGroup colorBar;
    public List<Toggle> colorToggles;

	public Slider penSizeBar;
	public Slider penAlphaBar;
	//public Slider penLerp;
	
	public Toggle isColorfulToggle;
    public Toggle isEarseToggle;

    public Button BTN_Clear;

    [Header("Paint Canvas")]
	public Painter painterCanvas;

    [Header("Canvas Camera")]
    public Camera canvasCamera;

    [Header("Runtime")]
    public Texture maskSprite;
    public RawImage PainterOutput;
    public Material drawOtherRtMat;
    RenderTexture _bufferRenderTexture;

    bool _isMouseDown =false;

    [EasyButtons.Button]
    void BindBrushToggles(){
        brushToggles = new List<Toggle>(brushBar.GetComponentsInChildren<Toggle>());
        colorToggles = new List<Toggle>(colorBar.GetComponentsInChildren<Toggle>());
    }

    void Start()
    {
        foreach (Toggle toggle in brushToggles)
        {
            toggle.onValueChanged.AddListener(OnBrushToggleEvent);
        }
        foreach (Toggle toggle in colorToggles)
        {
            toggle.onValueChanged.AddListener(OnColorToggleEvent);
        }

        penSizeBar?.onValueChanged.AddListener(x => {
            painterCanvas.brushScale = x;
        });

        penAlphaBar?.onValueChanged.AddListener(x => {
            //幫畫布調整alpha顏色, 使其最後複製過去的時候是正確的alpha色
            painterCanvas.canvasMat.SetFloat("_Alpha", x);
            drawOtherRtMat.SetFloat("_Alpha", x);
        });

        isColorfulToggle.onValueChanged.AddListener(x => {
            painterCanvas.paintType = x ? Painter.PaintType.DrawColorfulLine : Painter.PaintType.DrawLine;
        });

        isEarseToggle.onValueChanged.AddListener(x => {
            painterCanvas.isErase = x;
        });

        BTN_Clear?.onClick.AddListener(() => {
            //painterCanvas.ClearCanvas();
            SetupMask();
        });

        
        NewDrawFoNewUser();

        void OnBrushToggleEvent(bool val){
            foreach(Toggle toggle in brushBar.ActiveToggles()){

                //Set pen texture.
                painterCanvas.penMat.mainTexture = toggle.GetComponent<Image>().sprite.texture;
                break;
            }
        }

        void OnColorToggleEvent(bool val){
            foreach(Toggle toggle in colorBar.ActiveToggles()){
                painterCanvas.penColor = toggle.GetComponent<Image>().color;
                break;
            }
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _isMouseDown = true;
            //Draw once when mouse down.

            Vector2 pos;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(painterCanvas.transform as RectTransform, Input.mousePosition, canvasCamera, out pos))
            {
                painterCanvas.ClickDraw(pos, canvasCamera, painterCanvas.penMat.mainTexture, painterCanvas.brushScale, painterCanvas.penMat, painterCanvas.renderTexture, true);
            }
        }
        else if (Input.GetMouseButton(0))
        {
            if (_isMouseDown)
            {
                //draw on mouse drag.
                Vector2 pos;
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(painterCanvas.transform as RectTransform, Input.mousePosition, canvasCamera, out pos))
                {
                    painterCanvas.Drawing(pos, canvasCamera, painterCanvas.renderTexture, false, true);
                }
            }
        }
        else if (Input.GetMouseButtonUp(0) && _isMouseDown)
        {
            
            painterCanvas.DrawRT2OtherRT(painterCanvas.renderTexture, _bufferRenderTexture, drawOtherRtMat);
            painterCanvas.ClearCanvas();
            painterCanvas.EndDraw();
            _isMouseDown = false;
        }
    }

    void OnDisable() {
        painterCanvas.EndDraw();
    }

    public void NewDrawFoNewUser(){
        //記得 要與初始值不一樣才會有callback
        if(penSizeBar) penSizeBar.value = 0.5f;
        if(penAlphaBar) penAlphaBar.value = 1;
        painterCanvas.penColor = Color.black;
        SetupMask();
    }

    public void SetupMask(){
        if(maskSprite == null) {
            return;
        }
        if(_bufferRenderTexture != null){
            DestroyImmediate(_bufferRenderTexture);
        }

        //這個值怎麼動都怪怪的, 要保持1 彩度才正確
        painterCanvas.penMat.SetFloat("_Alpha", 1);
        //這個值怎麼動都怪怪的, 要保持White 彩度才正確
        drawOtherRtMat.SetVector("_Color", Color.white);

        //先根據Painter建立一張RenderTexture，並貼上Mask圖
        _bufferRenderTexture = new RenderTexture(painterCanvas.renderTexWidth, painterCanvas.renderTexHeight, 0, RenderTextureFormat.ARGB32);
        _bufferRenderTexture.useMipMap = false;
        Graphics.SetRenderTarget (_bufferRenderTexture);
        Graphics.Blit(maskSprite, _bufferRenderTexture);
        RenderTexture.active = null;

        //有這個Mask, 最後複製的時候只會取Mask裡的圖
        drawOtherRtMat.SetTexture("_MaskTex", maskSprite);
        //有這個Mask, 畫圖時只會顯示Mask裡的筆劃
        painterCanvas.canvasMat.SetTexture("_MaskTex", maskSprite);

        //先幫RawImage上預覽圖
        PainterOutput.texture = _bufferRenderTexture;
    }

    public void SetupNewMask(Texture tex){
        maskSprite = tex;
        SetupMask();
    }

    public void CopyTextureToRender(Texture rt){
        if (rt && _bufferRenderTexture)
        {
            GL.PushMatrix();
            GL.LoadPixelMatrix(0, rt.width, rt.height, 0);
            RenderTexture.active = _bufferRenderTexture;
            Graphics.DrawTexture(new Rect(0, 0, rt.width, rt.height), rt, drawOtherRtMat);
            RenderTexture.active = null;
            GL.PopMatrix();
        }
    }

    // private void OnValidate() {
    //     SetupMask();
    // }
}
