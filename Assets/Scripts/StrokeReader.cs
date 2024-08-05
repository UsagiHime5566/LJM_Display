using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.IO.Compression;
using System.Text;

public class StrokeReader : HimeLib.SingletonMono<StrokeReader>
{
    public PaintLight paintLight;
    public PainterMemory painterMemory;

    public string OutputDrawPath;
    public string lastFileName;

    public System.Action<string> OnPNGSaved;

    void Start(){
        paintLight.DefaultUserSettingLight();
        ESNetwork.instance.OnNewStrokeCome += StrokeCome;
        //painterMemory.OnReplayFinished += StrokeFinished;

        OutputDrawPath = LJMFileManager.instance.OutputDrawPath;
    }

    void StrokeCome(string compressMsg){
        string decompressedJson = DecompressString(compressMsg);
        painterMemory.paintData = JsonUtility.FromJson<PaintData>(decompressedJson);
        //painterMemory.Replay();

        ConvertStrokeToPNG();
    }

    public void ConvertStrokeToPNG(){
        SaveImgFile(painterMemory.GetDraw());
    }

    void StrokeFinished(){
        SaveImgFile(painterMemory.GetFinishedTexture());
    }

    //[EasyButtons.Button]
    void ReplayStoke(){
        string readCompressedJson = ReadFromFile("strokeData.txt");
        string decompressedJson = DecompressString(readCompressedJson);

        painterMemory.paintData = JsonUtility.FromJson<PaintData>(decompressedJson);
        painterMemory.Replay();
    }

    public void CreateJSON(){
        string jsonString = JsonUtility.ToJson(painterMemory.paintData);

        string compressedJson = CompressString(jsonString);

        // 將JSON字符串儲存到本地的txt文件上
        SaveToFile(compressedJson, "strokeData.txt");

        //ESNetwork.instance.SendStrokeToDisplay(compressedJson);
    }

    void SaveToFile(string dataString, string fileName)
    {
        string path = Path.Combine(Application.dataPath, fileName);
        File.WriteAllText(path, dataString);
        Debug.Log("File saved to: " + path);
    }

    string ReadFromFile(string fileName)
    {
        string path = Path.Combine(Application.dataPath, fileName);
        return File.ReadAllText(path);
    }

    string CompressString(string str)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(str);
        using (MemoryStream memoryStream = new MemoryStream())
        {
            using (GZipStream gzipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
            {
                gzipStream.Write(bytes, 0, bytes.Length);
            }
            return System.Convert.ToBase64String(memoryStream.ToArray());
        }
    }

    string DecompressString(string compressedStr)
    {
        byte[] bytes = System.Convert.FromBase64String(compressedStr);
        using (MemoryStream memoryStream = new MemoryStream(bytes))
        {
            using (GZipStream gzipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
            {
                using (MemoryStream decompressedStream = new MemoryStream())
                {
                    gzipStream.CopyTo(decompressedStream);
                    return Encoding.UTF8.GetString(decompressedStream.ToArray());
                }
            }
        }
    }

    public void SaveImgFile(RenderTexture RenderTextureRef){
        if(string.IsNullOrEmpty(OutputDrawPath)) return;
        FolderDetect(OutputDrawPath);

        byte[] bytes = SavePng(RenderTextureRef, OutputDrawPath, true);

        RenderTextureRef.Release();
        Object.Destroy(RenderTextureRef);
    }

    void FolderDetect(string path){
        try
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
        catch (IOException ex)
        {
            Debug.Log(ex.Message);
        }
    }

    public byte[] SavePng(RenderTexture RenderTextureRef, string path, bool saveDisk)
    {
        Texture2D tex = new Texture2D(RenderTextureRef.width, RenderTextureRef.height, TextureFormat.RGBA32, false);
        RenderTexture.active = RenderTextureRef;
        tex.ReadPixels(new Rect(0, 0, RenderTextureRef.width, RenderTextureRef.height), 0, 0);
        tex.Apply();

        // Encode texture into PNG
        byte[] bytes = tex.EncodeToPNG();
        Object.Destroy(tex);

        RenderTexture.active = null;

        //Write to a file in the project folder
        //string path = Application.dataPath + "/../SavedPaint.png";
        string fileName = System.DateTime.Now.ToString("yyyyMMddHHmmss") + ".png";
        string fullPath = Path.Combine(path, fileName);

        if(saveDisk){
            File.WriteAllBytes(fullPath, bytes);
            Debug.Log(bytes.Length/1024  + "Kb was saved as: " + fullPath);

            OnPNGSaved?.Invoke(fullPath);
        }

        lastFileName = fileName;

        return bytes;
    }
}
