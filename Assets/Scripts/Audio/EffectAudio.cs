using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 特效音乐
/// </summary>
public class EffectAudio : AudioBase
{
    private AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        Bind(AudioEvent.PLAY_EFFECT_AUDIO);
        audioSource = transform.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Awake()
    {
        
    }

    public override void Execute(int eventcode, object message)
    {
        base.Execute(eventcode, message);
        switch (eventcode)
        {
            case AudioEvent.PLAY_EFFECT_AUDIO:
                //playChatEffect((int)message);
                playChatEffect((string)message);
                break;
        }
    }

    /*//播放聊天音效
    public void playChatEffect(int chattype)
    {
        AudioClip audioClip = Resources.Load<AudioClip>("Sound/Chat/Chat_" + chattype);
        audioSource.clip = audioClip;
        audioSource.Play();
    }*/

    //播放音效
    public void playChatEffect(string path)
    {
        AudioClip audioClip = Resources.Load<AudioClip>("Sound/" + path);
        audioSource.clip = audioClip;
        audioSource.Play();
    }
}
