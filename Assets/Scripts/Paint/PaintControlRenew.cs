using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HimeLib;

public class PaintControlRenew : MonoBehaviour
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

        penSizeBar.onValueChanged.AddListener(x => {
            painterCanvas.brushScale = x;
        });

        penAlphaBar.onValueChanged.AddListener(x => {
            //事實上沒作用, 只會干擾顏色
            painterCanvas.penMat.SetFloat("_Alpha", 1);
            //painterCanvas.penMat.SetFloat("_Alpha", x);
        });

        isColorfulToggle.onValueChanged.AddListener(x => {
            painterCanvas.paintType = x ? Painter.PaintType.DrawColorfulLine : Painter.PaintType.DrawLine;
        });

        isEarseToggle.onValueChanged.AddListener(x => {
            painterCanvas.isErase = x;
        });

        BTN_Clear.onClick.AddListener(() => {
            painterCanvas.ClearCanvas();
        });

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
            painterCanvas.EndDraw();
            _isMouseDown = false;
        }
    }

    void OnDisable() {
        painterCanvas.EndDraw();
    }
}
