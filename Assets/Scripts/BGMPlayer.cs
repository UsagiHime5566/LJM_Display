using UnityEngine;
using DG.Tweening;

public class BGMPlayer : HimeLib.SingletonMono<BGMPlayer>
{
    public AudioSource music1;
    public AudioSource music2;
    public float fadeDuration = 2.0f;

    private AudioSource currentMusic;

    void Start()
    {
        // 初始化时不播放任何音乐
        currentMusic = null;
    }

    public void PlayMusic1()
    {
        SwitchMusic(music1);
    }

    public void PlayMusic2()
    {
        SwitchMusic(music2);
    }

    private void SwitchMusic(AudioSource newMusic)
    {
        if (currentMusic == newMusic)
            return;

        // 如果有当前播放的音乐，淡出
        if (currentMusic != null)
        {
            currentMusic.DOFade(0, fadeDuration);
        }

        // 淡入新的音乐
        newMusic.volume = 0;
        newMusic.Play();
        newMusic.DOFade(1, fadeDuration);

        currentMusic = newMusic;
    }
}
