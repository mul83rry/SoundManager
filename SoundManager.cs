using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [SerializeField]
    private SfxAudio[] sfxClips;

    [SerializeField]
    private Audio[] musicClips;

    [SerializeField]
    private bool loop;

    [SerializeField]
    private bool autoPlayBackgroundClip;

    [SerializeField]
    private Toggle sfxToggle, musicToggle;
    


    public bool MuteMusic
    {
        set
        {
            musicToggle.isOn = !value;
            PlayerPrefs.SetInt("Music", !value ? 1 : 0);
        }
        get => PlayerPrefs.GetInt("Music") == 0;
    }
    public bool MuteSfx
    {
        set
        {
            sfxToggle.isOn = !value;
            PlayerPrefs.SetInt("SFX", !value ? 1 : 0);
        }
        get => PlayerPrefs.GetInt("SFX") == 0;
    }

    private AudioSource audioSource;



    private void OnValidate()
    {
        for (var i = 0; i < sfxClips.Length; i++)
        {
            if (sfxClips[i].Clips.Length == 0)
                continue;
            if (sfxClips[i].Clips[0] == null)
                continue;
            if (!string.IsNullOrEmpty(sfxClips[i].Tag))
                continue;
            sfxClips[i].Tag = sfxClips[i].Clips[0].name;
        }
        for (var i = 0; i < musicClips.Length; i++)
        {
            if (musicClips[i].Clip == null)
                continue;
            if (!string.IsNullOrEmpty(musicClips[i].Tag))
                continue;
            musicClips[i].Tag = musicClips[i].Clip.name;
        }
    }

    private void Awake()
    {
        instance = this;
        if (!PlayerPrefs.HasKey("Music"))
        {
            MuteMusic = false;
        }
        if (!PlayerPrefs.HasKey("SFX"))
        {
            MuteSfx = false;
        }

        // setting up sound setting
        audioSource = GetComponent<AudioSource>();
        audioSource.mute = PlayerPrefs.GetInt("Music") == 0;
        audioSource.loop = loop;
        audioSource.playOnAwake = false;

        MuteSfx = MuteSfx;
        MuteMusic = MuteMusic;
    }

    private void Start()
    {
        if (autoPlayBackgroundClip)
            PlayMusic();
    }

    public void MuteUnmuteSfx(string audioTag)
    {
        MuteSfx = !MuteSfx;
        PlaySfx(audioTag);
    }
    public void MuteUnmuteMusic(string audioTag)
    {
        MuteMusic = !MuteMusic;
        PlaySfx(audioTag);
    }

    public void PlaySfx(string audioTag)
    {
        if (MuteSfx)
            return;
        if (audioTag == string.Empty)
            return;
        var clip = sfxClips.FirstOrDefault(c => c.Tag == audioTag);
        if (clip == null)
            throw new System.Exception($"audio clip is null {audioTag}");
        var i = new System.Random().Next(0, clip.Clips.Length);
        if (audioSource != null) audioSource.PlayOneShot(clip.Clips[i]);
    }
    public void PlaySfx(string audioTag, int index)
    {
        if (MuteSfx)
            return;
        if (audioTag == string.Empty)
            return;


        var clip = sfxClips.FirstOrDefault(c => c.Tag == audioTag);
        if (clip == null)
            throw new System.Exception("audio clip is null");

        if (index == -1)
        {
            index = Random.Range(0, clip.Clips.Length);
        }

        if (audioSource != null) audioSource.PlayOneShot(clip.Clips[index]);
    }


    public void PlayMusic()
    {
        if (musicClips.Length == 0)
            return;
        if (MuteMusic)
            return;
        audioSource.Stop();
        audioSource.clip = musicClips[Random.Range(0, musicClips.Length)].Clip;
        audioSource.Play();
    }
    public void PlayMusic(int index)
    {
        if (musicClips.Length <= index)
            return;
        if (MuteMusic)
            return;
        audioSource.Stop();
        audioSource.clip = musicClips[index].Clip;
        audioSource.Play();
    }
    public void PlayMusic(string audioTag)
    {
        if (musicClips.Length == 0)
            return;
        if (MuteMusic)
            return;
        audioSource.Stop();
        var clip = musicClips.FirstOrDefault(c => c.Tag == audioTag);
        if (clip == null)
            throw new System.Exception("audio clip is null");
        audioSource.clip = clip.Clip;
        audioSource.Play();
    }


    [System.Serializable]
    public class SfxAudio
    {
        public string Tag;
        public AudioClip[] Clips;
        public void Play()
        {
            instance.PlaySfx(Tag);
        }
    }
    [System.Serializable]
    public class Audio
    {
        public string Tag;
        public AudioClip Clip;
        public void Play()
        {
            instance.PlaySfx(Tag);
        }
    }
}
