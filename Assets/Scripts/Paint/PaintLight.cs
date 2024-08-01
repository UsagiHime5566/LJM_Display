using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HimeLib;


//Only Single color Draw version
public class PaintLight : MonoBehaviour
{
    [Header("Setting")]
    public List<float> sizeValues;

    [Header("Paint Canvas")]
    public Color DefaultColor;
	public Painter painterCanvas;
    public Material drawOtherRtMat;
    public Material matForCopy;

    [Header("Canvas Camera")]
    public Camera canvasCamera;

    public System.Action<Vector2> OnDrawStart;
    public System.Action<Vector2> OnDrawDrag;
    public System.Action OnDrawEnd;

    public void DrawStartLight(Vector2 pos){
        painterCanvas.ClickDraw(pos, canvasCamera, painterCanvas.penMat.mainTexture, painterCanvas.brushScale, painterCanvas.penMat, painterCanvas.renderTexture, true);
    }

    public void DrawDragLight(Vector2 pos){
        painterCanvas.Drawing(pos, canvasCamera, painterCanvas.renderTexture, false, true);
    }

    public void DrawEndLight(){
        painterCanvas.EndDraw();
    }

    public void DefaultUserSettingLight(){
        painterCanvas.brushScale = sizeValues[0];
        painterCanvas.canvasMat.SetFloat("_Alpha", 1);
        painterCanvas.canvasMat.SetTexture("_MaskTex", null);
        painterCanvas.penColor = DefaultColor;
        drawOtherRtMat.SetFloat("_Alpha", 1);
        drawOtherRtMat.SetVector("_Color", Color.white);
    }

    public void ClearDraw(){
        painterCanvas.ClearCanvas();
    }

    public void CopyTextureToRender(Texture rt, RenderTexture target = null){
        GL.PushMatrix();
        GL.LoadPixelMatrix(0, rt.width, rt.height, 0);
        if(target == null) target = painterCanvas.renderTexture;
        RenderTexture.active = target;
        Graphics.DrawTexture(new Rect(0, 0, rt.width, rt.height), rt, matForCopy);
        RenderTexture.active = null;
        GL.PopMatrix();
    }

    public RenderTexture CombineTextures(Texture tex){
        RenderTexture m_rt = new RenderTexture(painterCanvas.renderTexWidth, painterCanvas.renderTexHeight, 0, RenderTextureFormat.ARGB32);
        m_rt.filterMode = FilterMode.Bilinear;
        m_rt.useMipMap = false;

        Graphics.Blit(tex, m_rt);
        CopyTextureToRender(painterCanvas.renderTexture, m_rt);

        return m_rt;
    }

    void OnDisable() {
        painterCanvas.EndDraw();
    }
}
