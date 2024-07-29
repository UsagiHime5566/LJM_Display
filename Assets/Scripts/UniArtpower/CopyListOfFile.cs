using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class CopyListOfFile : MonoBehaviour
{
    [Multiline] public string data;
    public List<string> allowedExtensions; // 允許的副檔名列表
    public string destinationFolder; // 目標資料夾
    void Start()
    {
        Copy();
    }

    void Copy()
    {
        string[] lines = data.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries); // 同時考慮 \n 和 \r

        foreach (string line in lines)
        {
            foreach (string extension in allowedExtensions)
            {
                if (line.Contains("Assets") && line.EndsWith(extension, StringComparison.OrdinalIgnoreCase))
                {
                    // 擷取 "Assets" 之後的文字資料
                    int startIndex = line.IndexOf("Assets");
                    string assetPath = line.Substring(startIndex).Trim(); // 去除空白

                    // 取得檔案完整路徑
                    string fullPath = Path.Combine(Application.dataPath, "..", assetPath);

                    // 確認檔案存在
                    if (File.Exists(fullPath))
                    {
                        // 建立目標檔案路徑
                        string destinationPath = Path.Combine(destinationFolder, Path.GetFileName(assetPath));

                        // 複製檔案
                        File.Copy(fullPath, destinationPath, true);

                        Debug.Log("檔案複製成功：" + destinationPath);
                    }
                    else
                    {
                        Debug.LogError("檔案不存在：" + fullPath);
                    }
                    break;
                }
            }
        }
    }
    
    void Go()
    {
        string[] lines = data.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries); // 同時考慮 \n 和 \r

        foreach (string line in lines)
        {
            foreach (string extension in allowedExtensions)
            {
                if (line.Contains("Assets") && line.EndsWith(extension, StringComparison.OrdinalIgnoreCase))
                {
                    // 擷取 "Assets" 之後的文字資料
                    int startIndex = line.IndexOf("Assets");
                    string assetName = line.Substring(startIndex);

                    Debug.Log(assetName);
                    break;
                }
            }
        }
    }

    void Go2()
    {
        string[] lines = data.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries); // 同時考慮 \n 和 \r

        foreach (string line in lines)
        {
            // foreach (var item in line.ToCharArray())
            // {
            //     Debug.Log((int)item + "|" + item);
            // }
            // Debug.Log(line);
            // Debug.Log(line.EndsWith(".cs"));

            if (line.Contains("Assets") && (line.EndsWith(".cs")))
            {
                // 擷取 "Assets" 之後的文字資料
                int startIndex = line.IndexOf("Assets");
                string assetName = line.Substring(startIndex);

                Debug.Log(assetName);
            }
        }
    }
}