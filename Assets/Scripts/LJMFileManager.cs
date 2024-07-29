using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class LJMFileManager : HimeLib.SingletonMono<LJMFileManager>
{
    public List<Text> TXT_UserCounts;
    public string OutputDrawPath;   
    void Start()
    {
        OutputDrawPath = Application.dataPath + "/../";
        int pngFileCount = GetPngFileCount(OutputDrawPath);
        Debug.Log("Number of .png files: " + pngFileCount);

        StartCoroutine(LoopUser());
    }

    IEnumerator LoopUser(){
        while (true)
        {
            yield return new WaitForSeconds(1);

            int pngFileCount = GetPngFileCount(OutputDrawPath);
            string count = pngFileCount.ToString("D5");
            foreach (var item in TXT_UserCounts)
            {
                item.text = $"連署人數：{count}人";
            }
        }
    }

    int GetPngFileCount(string path)
    {
        if (Directory.Exists(path))
        {
            // 取得資料夾內所有副檔名為 .png 的檔案
            string[] pngFiles = Directory.GetFiles(path, "*.png", SearchOption.AllDirectories);
            return pngFiles.Length;
        }
        else
        {
            Debug.LogError("The specified folder path does not exist.");
            return 0;
        }
    }
}
