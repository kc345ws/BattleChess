using Assets.Scripts.Net;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Protocol.Code;
using Protocol.Dto.Fight;

public class ChatHandler : HandlerBase
{

    private static ChatHandler instance = new ChatHandler();
    public static ChatHandler Instance { get
        {
            lock (instance)
            {
                if(instance == null)
                {
                    instance = new ChatHandler();
                }
                return instance;
            }
        } }

    private ChatDto chatDto;

    private ChatHandler() { }

    public override void OnReceive(int subcode, object message)
    {
        switch (subcode)
        {
            case ChatCode.SBOD:
                chatDto = message as ChatDto;
                processSBOD(chatDto);
                break;
        }
    }

    private void processSBOD(ChatDto chatDto)
    {
        switch (chatDto.ChatType)
        {
            case 1:
                chatDto.SetText("大家好，很高兴见到各位~");
                Dispatch(AreoCode.UI, UIEvent.PLAYER_CHAT, chatDto);
                Dispatch(AreoCode.AUDIO, AudioEvent.PLAY_EFFECT_AUDIO, "Chat/Chat_1");
                break;

            case 2:
                chatDto.SetText("和你合作真是太愉快了！");
                Dispatch(AreoCode.UI, UIEvent.PLAYER_CHAT, chatDto);
                Dispatch(AreoCode.AUDIO, AudioEvent.PLAY_EFFECT_AUDIO, "Chat/Chat_2");
                break;

            case 3:
                chatDto.SetText("快点吧，我等到花儿都谢了！");
                Dispatch(AreoCode.UI, UIEvent.PLAYER_CHAT, chatDto);
                Dispatch(AreoCode.AUDIO, AudioEvent.PLAY_EFFECT_AUDIO, "Chat/Chat_3");
                break;

            case 4:
                chatDto.SetText("你的牌打得太好了！");
                Dispatch(AreoCode.UI, UIEvent.PLAYER_CHAT, chatDto);
                Dispatch(AreoCode.AUDIO, AudioEvent.PLAY_EFFECT_AUDIO, "Chat/Chat_4");
                break;

            case 5:
                chatDto.SetText("不要吵了，有什么好吵得，专心玩游戏吧！");
                Dispatch(AreoCode.UI, UIEvent.PLAYER_CHAT, chatDto);
                Dispatch(AreoCode.AUDIO, AudioEvent.PLAY_EFFECT_AUDIO, "Chat/Chat_5");
                break;

            case 6:
                chatDto.SetText("不要走，决战到天亮！");
                Dispatch(AreoCode.UI, UIEvent.PLAYER_CHAT, chatDto);
                Dispatch(AreoCode.AUDIO, AudioEvent.PLAY_EFFECT_AUDIO, "Chat/Chat_6");
                break;

            case 7:
                chatDto.SetText("再见了，我会想念大家的");
                Dispatch(AreoCode.UI, UIEvent.PLAYER_CHAT, chatDto);
                Dispatch(AreoCode.AUDIO, AudioEvent.PLAY_EFFECT_AUDIO, "Chat/Chat_7");
                break;
        }
    }
}
