using UnityEngine;

// 确保挂载此脚本的物体会自动添加 AudioSource 组件
[RequireComponent(typeof(AudioSource))]
public class BGMManager : MonoBehaviour
{
    public static BGMManager Instance; // 单例模式
    private AudioSource audioSource;

    void Awake()
    {
        // 实现单例模式，确保切换场景时背景音乐不会被销毁
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 切换场景时不销毁此对象
            audioSource = GetComponent<AudioSource>();
            
            // 确保音频会循环播放
            audioSource.loop = true; 
        }
        else
        {
            // 如果场景中已经有了 BGMManager，销毁多余的
            Destroy(gameObject);
        }
    }

    // 播放指定的背景音乐
    public void PlayMusic(AudioClip bgmClip)
    {
        if (bgmClip == null) return;

        // 如果要播放的音乐已经是当前正在播放的音乐，则不重复播放（防止切换场景时音乐重新开始）
        if (audioSource.clip == bgmClip) return;

        audioSource.clip = bgmClip;
        audioSource.Play();
    }

    // 停止播放音乐
    public void StopMusic()
    {
        audioSource.Stop();
    }
}