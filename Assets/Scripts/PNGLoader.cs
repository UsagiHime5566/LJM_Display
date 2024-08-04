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

    public System.Action<Sprite> OnSinatureLoaded;

    void Start()
    {
        strokeReader.OnPNGSaved += x => {
            StartCoroutine(LoadSinglePath(x));
        };

        var folderPath = LJMFileManager.instance.OutputDrawPath;

        // 获取文件夹中的所有PNG文件路径
        if (Directory.Exists(folderPath))
        {
            pngFiles = Directory.GetFiles(folderPath, "*.png");

            if (pngFiles.Length > 0)
            {
                StartCoroutine(LoadAndDisplayImages());
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

    IEnumerator LoadAndDisplayImages()
    {
        for(int i = 0; i < pngFiles.Length ; i++)
        {
            string filePath = "file://" + pngFiles[i];
            //Debug.Log(filePath);
            using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(filePath))
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

                    OnSinatureLoaded?.Invoke(sprite);
                }
            }
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

                OnSinatureLoaded?.Invoke(sprite);
            }
        }
    }
}
