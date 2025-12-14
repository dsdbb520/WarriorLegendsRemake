using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    public AudioSource bgmSource;  // 背景音乐
    public AudioSource sfxSource;  // 音效
    public AudioMixer audioMixer;  // 可选：用来控制音量

    [Header("默认音效")]
    public AudioClip defaultBGM;
    public AudioClip attackClip1;
    public AudioClip attackClip2;
    public AudioClip hitClip;
    public AudioClip jumpClip;
    public AudioClip deathClip;

    private void Awake()
    {
        // 单例初始化
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // 场景切换时不销毁
    }

    private void Start()
    {
        if (defaultBGM != null)
        {
            PlayBGM(defaultBGM);
        }
    }

    //播放背景音乐
    public void PlayBGM(AudioClip clip, bool loop = true)
    {
        if (clip == null) return;
        bgmSource.clip = clip;
        bgmSource.loop = loop;
        bgmSource.Play();
    }

    //停止背景音乐
    public void StopBGM()
    {
        bgmSource.Stop();
    }

    //播放单次音效
    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip);
    }

    //调节音量（可连接 UI 滑条）
    public void SetVolume(string parameter, float volume)
    {
        //parameter例如："MasterVolume"、"BGMVolume"、"SFXVolume"
        //volume范围 0.0001f ~ 1f
        audioMixer.SetFloat(parameter, Mathf.Log10(volume) * 20);
    }

    // 快捷播放角色音效
    public void PlayCharacterSound(string type)
    {
        switch (type)
        {
            case "attack1":
                PlaySFX(attackClip1);
                break;
            case "attack2":
                PlaySFX(attackClip2);
                break;
            case "hit":
                PlaySFX(hitClip);
                break;
            case "jump":
                PlaySFX(jumpClip);
                break;
            case "death":
                PlaySFX(deathClip);
                break;
        }
    }
}
