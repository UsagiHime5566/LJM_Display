using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public class PNGLoader : MonoBehaviour
{
    public StrokeReader strokeReader;
    public List<Sprite> currentFiles;
    private string[] pngFiles;
    private int currentIndex = 0;

    public System.Action<Sprite, System.DateTime> OnSinatureLoaded;

    void Start()
    {
        strokeReader.OnPNGSaved += x => {
            StartCoroutine(LoadSinglePath(x));
        };

        LoadFolderPNG();
    }

    void LoadFolderPNG(){
        var folderPath = LJMFileManager.instance.OutputDrawPath;

        // 获取文件夹中的所有PNG文件路径
        if (Directory.Exists(folderPath))
        {
            pngFiles = Directory.GetFiles(folderPath, "*.png");

            if (pngFiles.Length > 0)
            {
                StartCoroutine(LoadAndDisplayImages(pngFiles));
            }
            else
            {
                Debug.LogError("No PNG files found in the specified folder.");
            }
        }
        else
        {
            Debug.LogError("The specified folder path does not exist.");
        }
    }

    IEnumerator LoadAndDisplayImages(string[] files)
    {
        for(int i = 0; i < files.Length ; i++)
        {
            //改檔案名稱用
            // string file = Path.GetFileNameWithoutExtension(files[i]);
            // if(file.Length < 4){
            //     string directory = Path.GetDirectoryName(files[i]);
            //     string fileName = Path.GetFileName(files[i]);
            //     string newFileName = "0" + fileName;
            //     string newFilePath = Path.Combine(directory, newFileName);
            //     File.Move(files[i], newFilePath);
            // }

            string filePath = "file://" + files[i];
            //Debug.Log(filePath);
            yield return LoadSinglePath(filePath);
        }
    }

    IEnumerator LoadSinglePath(string path){
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(path))
        {
            yield return uwr.SendWebRequest();

            if (uwr.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to load texture: " + uwr.error);
            }
            else
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(uwr);
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                currentFiles.Add(sprite);

                
                OnSinatureLoaded?.Invoke(sprite, GetFileTime(path));
            }
        }
    }

    System.DateTime GetFileTime(string filePath){
        string format = "yyyyMMddHHmmss";
        try {
            string file = Path.GetFileNameWithoutExtension(filePath);
            System.DateTime dTime = System.DateTime.ParseExact(file, format, null);
            return dTime;
            
        } catch (System.Exception e) {
            return new System.DateTime(1999,1,1);
        }
    }
}
