using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using System.IO;
using System.Threading.Tasks;

public class ESAdManager : HimeLib.SingletonMono<ESAdManager>
{
    public List<VideoPlayer> adPlayers;
    public List<string> adPaths;
    public bool AutoNextAd = true;
    public float vp_debug_speed = 20;

    public System.Action<int> OnAdEnd;
    System.Action requireStopCallback;

    [Header("Runtime (不需要設定)")]
    [SerializeField] List<int> clipIndex = new List<int>(){0, 0};
    [SerializeField] List<bool> requireStop = new List<bool>(){false, false};
    [SerializeField, Multiline] string adOrder;
    [SerializeField] List<FileInfo []> currentPlayList;
    
    void Start()
    {
        for (int i = 0; i < adPlayers.Count; i++)
        {
            int v = i;
            adPlayers[v].loopPointReached += x => {
                //clipIndex[v] += 1;

                if(requireStop[v]){
                    StopAD();
                    requireStopCallback?.Invoke();
                    return;
                }
                OnAdEnd?.Invoke(v);
                if(AutoNextAd)
                    PlayClip(v);
            };

            #if UNITY_EDITOR
                adPlayers[v].playbackSpeed = vp_debug_speed;
            #endif
        }

        StartCoroutine(DelaySetup());
    }

    IEnumerator DelaySetup(){
        yield return null;
        currentPlayList = new List<FileInfo[]>();
        clipIndex = new List<int>();
        requireStop = new List<bool>();
        for (int i = 0; i < adPlayers.Count; i++)
        {
            currentPlayList.Add(null);
            clipIndex.Add(0);
            requireStop.Add(false);
            GeneratePlayList(i);
        }
    }

    public void PlayAD(){
        
        clipIndex = new List<int>();
        requireStop = new List<bool>();
        for (int i = 0; i < adPlayers.Count; i++)
        {
            clipIndex.Add(0);
            requireStop.Add(false);
            GeneratePlayList(i);
            PlayClip(i);
        }
    }

    void GeneratePlayList(int index){
        var info = GetAdList(adPaths[index]);
        if(info == null || info.Length == 0) return;
        if(clipIndex[index] >= info.Length) clipIndex[index] = 0;
        ShuffleArray(info);

        currentPlayList[index] = info;

        adOrder = "";
        foreach (var item in info)
        {
            adOrder += $"{item.Name}\n";
        }
    }

    public async void PlayClip(int index){
        var info = currentPlayList[index];
        if(clipIndex[index] >= info.Length) clipIndex[index] = 0;

        await Task.Delay(200);

        adPlayers[index].url = info[clipIndex[index]].FullName;
        //adPlayers[index].time = 0;
        adPlayers[index].Play();
        clipIndex[index] += 1;

        Debug.Log($"({clipIndex[index]}) file name: {info[clipIndex[index]].FullName}. / " + System.DateTime.Now);
    }

    FileInfo [] GetAdList(string url){
        if(string.IsNullOrEmpty(url)) return null;

        DirectoryInfo dir = new DirectoryInfo(url);
        var infos = dir.GetFiles("*.mp4");

        return infos;
    }

    public void StopAD(){
        for (int i = 0; i < adPlayers.Count; i++)
        {
            adPlayers[i].Stop();
        }
    }

    public void RequireStopAD(System.Action callback){
        requireStopCallback = callback;
        for (int i = 0; i < adPlayers.Count; i++)
        {
            requireStop[i] = true;
        }
    }

    // 泛型函数，用于随机排序数组
    void ShuffleArray<T>(T[] array)
    {
        System.Random rng = new System.Random();
        int n = array.Length;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = array[k];
            array[k] = array[n];
            array[n] = value;
        }

        // 打印随机排序后的数组内容
        //Debug.Log("Randomized Array: " + System.String.Join(", ", array));
    }
}
