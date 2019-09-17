using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgmManager : AudioBase
{
    private AudioSource audioSource;

    private void Awake()
    {
        Bind(AudioEvent.PLAY_BG_AUDIO);
        Bind(AudioEvent.CHANGE_BGM_VOLUME);
        Bind(AudioEvent.STOP_BGM);
    }

    public override void Execute(int eventcode, object message)
    {
        base.Execute(eventcode, message);
        switch (eventcode)
        {
            case AudioEvent.PLAY_BG_AUDIO:
                Play();
                break;

            case AudioEvent.CHANGE_BGM_VOLUME:
                ChangeVolume((float)message);
                break;

            case AudioEvent.STOP_BGM:
                Stop();
                break;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //AudioClip clip = Resources.Load<AudioClip>("Sound/BGM");
        AudioClip clip = Resources.Load<AudioClip>("Audio/BGM");
        audioSource = GetComponent<AudioSource>();

        audioSource.clip = clip;
        Play();
    }

    private void Play()
    {
        audioSource.Play();
    }

    private void Stop()
    {
        audioSource.Pause();
    }

    private void ChangeVolume(float value)
    {
        audioSource.volume = value;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
