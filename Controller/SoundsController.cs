using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundsController : MonoBehaviour
{   
    
    private AudioSource m_audioBackground;
    private AudioSource m_audioFX;
    private AudioSource m_audioSpeech;

    public const string MELODY_MAIN_MENU = "MELODY_MAIN_MENU";
    public const string MELODY_INGAME = "MELODY_INGAME";
    public const string MELODY_WIN = "MELODY_WIN";
    public const string MELODY_LOSE = "MELODY_LOSE";

    public const string FX_SHOOT = "FX_SHOOT";
    public const string FX_DEAD_ENEMY = "FX_DEAD_ENEMY";
    public const string FX_DEAD_NPC = "FX_DEAD_NPC";

    public const string SPEECH_NPC = "SPEECH_NPC";


    private static SoundsController _instance;

    public static SoundsController Instance
    {
        get
        {
            if (!_instance)
            {
                _instance = GameObject.FindObjectOfType<SoundsController>();
            }
            return _instance;
        }
    }


    
    public AudioClip[] Sounds;


     void Awake()
    {
        AudioSource[] myAudioSources = GetComponents<AudioSource>();
        m_audioBackground = myAudioSources[0];
        m_audioFX = myAudioSources[1];
        m_audioSpeech = myAudioSources[2];
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void PlaySoundClipBackground(AudioClip _audio, bool _loop, float _volume)
    {
        
        
        m_audioBackground.clip = _audio;
        m_audioBackground.loop = _loop;
        m_audioBackground.volume = _volume;
        m_audioBackground.Play();
    }

    public void StopSoundsBackground()
    {
        m_audioBackground.clip = null;
        m_audioBackground.Stop();
    }

    public void PlaySoundBackground(string _audioName, bool _loop, float _volume)
    {
        for (int i = 0; i < Sounds.Length; i++)
        {
            if(Sounds[i].name == _audioName)
            {
                PlaySoundClipBackground(Sounds[i], _loop, _volume);
            }
        }
    }

    private void PlaySoundClipFX(AudioClip _audio, bool _loop, float _volume)
    {
        
        m_audioFX.clip = _audio;
        m_audioFX.loop = _loop;
        m_audioFX.volume = _volume;
        m_audioFX.Play();
    }

    public void StopSoundsFX()
    {
        m_audioFX.clip = null;
        m_audioFX.Stop();
    }


    public void PlaySoundFX(string _audioName, bool _loop, float _volume)
    {
        for (int i = 0; i < Sounds.Length; i++)
        {
            if (Sounds[i].name == _audioName)
            {
                PlaySoundClipFX(Sounds[i], _loop, _volume);
            }
        }
    }

    private void PlaySoundClipSpeech(AudioClip _audio, bool _loop, float _volume)
    {
        m_audioSpeech.clip = _audio;
        m_audioSpeech.loop = _loop;
        m_audioSpeech.volume = _volume;
        m_audioSpeech.Play();
    }

    public void StopSoundsSpeech()
    {
        m_audioSpeech.clip = null;
        m_audioSpeech.Stop();
    }


    public void PlaySoundSpeech(string _audioName, bool _loop, float _volume)
    {
        for (int i = 0; i < Sounds.Length; i++)
        {
            if (Sounds[i].name == _audioName)
            {
                PlaySoundClipSpeech(Sounds[i], _loop, _volume);
            }
        }
    }

    
}
