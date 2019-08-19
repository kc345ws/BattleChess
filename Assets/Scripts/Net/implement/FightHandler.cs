using Assets.Scripts.Net;
using Protocol.Code;
using Protocol.Constants;
using Protocol.Dto.Fight;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightHandler : HandlerBase
{
    private static FightHandler instance = new FightHandler();
    public static FightHandler Instance { get
        {
            lock (instance)
            {
                if(instance == null)
                {
                    instance = new FightHandler();
                }
                return instance;
            }
        } }

    private FightHandler() { }

    public override void OnReceive(int subcode, object message)
    {
        switch (subcode)
        {

            /*case FightCode.DEAL_SRES:
                processDealSres((bool)message);
                break;

            case FightCode.DEAL_SBOD:
                processDealSBOD(message as DealDto);
                break;

            case FightCode.GET_CARD_SRES://服务器给客户端发牌
                processGetCard(message as List<CardDto>);
                break;

            case FightCode.PASS_SRES:
                processPassSRES((bool)message);
                break;

            case FightCode.TURN_LANDLORD_SBOD:
                processTurnLandlord((int)message);
                break;

            case FightCode.GRAB_LANDLORD_SBOD:
                processGrabLandlord(message as LandLordDto);
                break;

            case FightCode.TURN_DEAL_SBOD:
                processTurnDeal((int)message);
                break;

            case FightCode.GAME_OVER_SBOD:
                processGameOver(message as OverDto);
                break;*/
            case FightCode.SELECT_RACE_SBOD:
                processSelectRaceSBOD();
                break;
        }
    }

    private void processSelectRaceSBOD()
    {
        Dispatch(AreoCode.UI, UIEvent.SHOW_SELECT_RACE_PANEL, true);
    }

    #region 其他代码
    /// <summary>
    /// 处理游戏结束
    /// </summary>
    private void processGameOver(OverDto overDto)
    {
        //显示结束面板
        Dispatch(AreoCode.UI, UIEvent.SHOW_OVER_PANEL, true);
        //设置信息并播放音效
        Dispatch(AreoCode.UI, UIEvent.SET_OVER_PANEL_MESSAGE, overDto);
    }

    /// <summary>
    /// 处理跳过
    /// </summary>
    private void processPassSRES(bool active)
    {
        if (active)
        {
            Dispatch(AreoCode.UI, UIEvent.SHOW_DEAL_BUTTON, false);
            Dispatch(AreoCode.AUDIO, AudioEvent.PLAY_EFFECT_AUDIO, "Fight/Woman_buyao1");
        }
    }

    private void processDealSBOD(DealDto dealDto)
    {
        //移除手牌
        int uid = dealDto.UserID;
        /*if (uid == GameModles.Instance.userDto.ID)
        {
            Dispatch(AreoCode.CHARACTER, CharacterEvent.REMOVE_MY_CARDS, dealDto.remainCards);
        }
        else if (uid == GameModles.Instance.matchRoomDto.Leftid)
        {
            Dispatch(AreoCode.CHARACTER, CharacterEvent.REMOVE_LEFT_CARDS, dealDto.remainCards);
        }
        else if (uid == GameModles.Instance.matchRoomDto.Rightid)
        {
            Dispatch(AreoCode.CHARACTER, CharacterEvent.REMOVE_RIGHT_CARDS, dealDto.remainCards);
        }*/
        //将手牌显示在桌面上
        Dispatch(AreoCode.CHARACTER, CharacterEvent.UPDATE_SHOW_dESK, dealDto.SelectCards);

        //播放音效
        /*if (dealDto.Type == CardType.SINGLE)
        {
            if (dealDto.Weight == CardWeight.THREE)
            {
                Dispatch(AreoCode.AUDIO, AudioEvent.PLAY_EFFECT_AUDIO, "Fight/Woman_3");
            }
            else if (dealDto.Weight == CardWeight.FOUR)
            {
                Dispatch(AreoCode.AUDIO, AudioEvent.PLAY_EFFECT_AUDIO, "Fight/Woman_4");
            }
            else if (dealDto.Weight == CardWeight.FIVE)
            {
                Dispatch(AreoCode.AUDIO, AudioEvent.PLAY_EFFECT_AUDIO, "Fight/Woman_5");
            }
            else if (dealDto.Weight == CardWeight.SIX)
            {
                Dispatch(AreoCode.AUDIO, AudioEvent.PLAY_EFFECT_AUDIO, "Fight/Woman_6");
            }
            else if (dealDto.Weight == CardWeight.SEVEN)
            {
                Dispatch(AreoCode.AUDIO, AudioEvent.PLAY_EFFECT_AUDIO, "Fight/Woman_7");
            }
            else if (dealDto.Weight == CardWeight.EIGHT)
            {
                Dispatch(AreoCode.AUDIO, AudioEvent.PLAY_EFFECT_AUDIO, "Fight/Woman_8");
            }
            else if (dealDto.Weight == CardWeight.NENE)
            {
                Dispatch(AreoCode.AUDIO, AudioEvent.PLAY_EFFECT_AUDIO, "Fight/Woman_9");
            }
            else if (dealDto.Weight == CardWeight.TEN)
            {
                Dispatch(AreoCode.AUDIO, AudioEvent.PLAY_EFFECT_AUDIO, "Fight/Woman_10");
            }
            else if (dealDto.Weight == CardWeight.JACK)
            {
                Dispatch(AreoCode.AUDIO, AudioEvent.PLAY_EFFECT_AUDIO, "Fight/Woman_11");
            }
            else if (dealDto.Weight == CardWeight.QUEEN)
            {
                Dispatch(AreoCode.AUDIO, AudioEvent.PLAY_EFFECT_AUDIO, "Fight/Woman_12");
            }
            else if (dealDto.Weight == CardWeight.KING)
            {
                Dispatch(AreoCode.AUDIO, AudioEvent.PLAY_EFFECT_AUDIO, "Fight/Woman_13");
            }
            else if (dealDto.Weight == CardWeight.A)
            {
                Dispatch(AreoCode.AUDIO, AudioEvent.PLAY_EFFECT_AUDIO, "Fight/Woman_14");
            }
            else if (dealDto.Weight == CardWeight.TWO)
            {
                Dispatch(AreoCode.AUDIO, AudioEvent.PLAY_EFFECT_AUDIO, "Fight/Woman_15");
            }
            else if (dealDto.Weight == CardWeight.SMALLJOKER)
            {
                Dispatch(AreoCode.AUDIO, AudioEvent.PLAY_EFFECT_AUDIO, "Fight/Woman_100");
            }
            else if (dealDto.Weight == CardWeight.BIGJOKER)
            {
                Dispatch(AreoCode.AUDIO, AudioEvent.PLAY_EFFECT_AUDIO, "Fight/Woman_200");
            }
        }
        else if (dealDto.Type == CardType.DOUBLE)
        {
            if (dealDto.Weight == CardWeight.THREE * 2)
            {
                Dispatch(AreoCode.AUDIO, AudioEvent.PLAY_EFFECT_AUDIO, "Fight/Woman_dui3");
            }
            else if (dealDto.Weight == CardWeight.FOUR * 2)
            {
                Dispatch(AreoCode.AUDIO, AudioEvent.PLAY_EFFECT_AUDIO, "Fight/Woman_dui4");
            }
            else if (dealDto.Weight == CardWeight.FIVE * 2)
            {
                Dispatch(AreoCode.AUDIO, AudioEvent.PLAY_EFFECT_AUDIO, "Fight/Woman_dui5");
            }
            else if (dealDto.Weight == CardWeight.SIX * 2)
            {
                Dispatch(AreoCode.AUDIO, AudioEvent.PLAY_EFFECT_AUDIO, "Fight/Woman_dui6");
            }
            else if (dealDto.Weight == CardWeight.SEVEN * 2)
            {
                Dispatch(AreoCode.AUDIO, AudioEvent.PLAY_EFFECT_AUDIO, "Fight/Woman_dui7");
            }
            else if (dealDto.Weight == CardWeight.EIGHT * 2)
            {
                Dispatch(AreoCode.AUDIO, AudioEvent.PLAY_EFFECT_AUDIO, "Fight/Woman_dui8");
            }
            else if (dealDto.Weight == CardWeight.NENE * 2)
            {
                Dispatch(AreoCode.AUDIO, AudioEvent.PLAY_EFFECT_AUDIO, "Fight/Woman_dui9");
            }
            else if (dealDto.Weight == CardWeight.TEN * 2)
            {
                Dispatch(AreoCode.AUDIO, AudioEvent.PLAY_EFFECT_AUDIO, "Fight/Woman_dui10");
            }
            else if (dealDto.Weight == CardWeight.JACK * 2)
            {
                Dispatch(AreoCode.AUDIO, AudioEvent.PLAY_EFFECT_AUDIO, "Fight/Woman_dui11");
            }
            else if (dealDto.Weight == CardWeight.QUEEN * 2)
            {
                Dispatch(AreoCode.AUDIO, AudioEvent.PLAY_EFFECT_AUDIO, "Fight/Woman_dui12");
            }
            else if (dealDto.Weight == CardWeight.KING * 2)
            {
                Dispatch(AreoCode.AUDIO, AudioEvent.PLAY_EFFECT_AUDIO, "Fight/Woman_dui13");
            }
            else if (dealDto.Weight == CardWeight.A * 2)
            {
                Dispatch(AreoCode.AUDIO, AudioEvent.PLAY_EFFECT_AUDIO, "Fight/Woman_dui14");
            }
            else if (dealDto.Weight == CardWeight.TWO * 2)
            {
                Dispatch(AreoCode.AUDIO, AudioEvent.PLAY_EFFECT_AUDIO, "Fight/Woman_dui15");
            }
        }
        else if (dealDto.Type == CardType.THREE_STRIGHT_FLIGHT)
        {
            Dispatch(AreoCode.AUDIO, AudioEvent.PLAY_EFFECT_AUDIO, "Fight/Woman_feiji");
        }
        else if (dealDto.Type == CardType.STRIGHT)
        {
            Dispatch(AreoCode.AUDIO, AudioEvent.PLAY_EFFECT_AUDIO, "Fight/Woman_shunzi");
        }
        else if (dealDto.Type == CardType.DOUBLE_STRIGHT || dealDto.Type == CardType.THREE_STRIGHT)
        {
            Dispatch(AreoCode.AUDIO, AudioEvent.PLAY_EFFECT_AUDIO, "Fight/Woman_liandui");
        }
        else if (dealDto.Type == CardType.THREE_ONE)
        {
            Dispatch(AreoCode.AUDIO, AudioEvent.PLAY_EFFECT_AUDIO, "Fight/Woman_sandaiyi");

        }
        else if (dealDto.Type == CardType.THREE_DOUBLE)
        {
            Dispatch(AreoCode.AUDIO, AudioEvent.PLAY_EFFECT_AUDIO, "Fight/Woman_sandaiyidui");
        }
        else if (dealDto.Type == CardType.FOUR_DOUBLE)
        {
            Dispatch(AreoCode.AUDIO, AudioEvent.PLAY_EFFECT_AUDIO, "Fight/Woman_sidaier");
        }
        else if (dealDto.Type == CardType.THREE)
        {
            if (dealDto.Weight == CardWeight.THREE * 3)
            {
                Dispatch(AreoCode.AUDIO, AudioEvent.PLAY_EFFECT_AUDIO, "Fight/Woman_tuple3");
            }
            else if (dealDto.Weight == CardWeight.FOUR * 3)
            {
                Dispatch(AreoCode.AUDIO, AudioEvent.PLAY_EFFECT_AUDIO, "Fight/Woman_tuple4");
            }
            else if (dealDto.Weight == CardWeight.FIVE * 3)
            {
                Dispatch(AreoCode.AUDIO, AudioEvent.PLAY_EFFECT_AUDIO, "Fight/Woman_tuple5");
            }
            else if (dealDto.Weight == CardWeight.SIX * 3)
            {
                Dispatch(AreoCode.AUDIO, AudioEvent.PLAY_EFFECT_AUDIO, "Fight/Woman_tuple6");
            }
            else if (dealDto.Weight == CardWeight.SEVEN * 3)
            {
                Dispatch(AreoCode.AUDIO, AudioEvent.PLAY_EFFECT_AUDIO, "Fight/Woman_tuple7");
            }
            else if (dealDto.Weight == CardWeight.EIGHT * 3)
            {
                Dispatch(AreoCode.AUDIO, AudioEvent.PLAY_EFFECT_AUDIO, "Fight/Woman_tuple8");
            }
            else if (dealDto.Weight == CardWeight.NENE * 3)
            {
                Dispatch(AreoCode.AUDIO, AudioEvent.PLAY_EFFECT_AUDIO, "Fight/Woman_tuple9");
            }
            else if (dealDto.Weight == CardWeight.TEN * 3)
            {
                Dispatch(AreoCode.AUDIO, AudioEvent.PLAY_EFFECT_AUDIO, "Fight/Woman_tuple10");
            }
            else if (dealDto.Weight == CardWeight.JACK * 3)
            {
                Dispatch(AreoCode.AUDIO, AudioEvent.PLAY_EFFECT_AUDIO, "Fight/Woman_tuple11");
            }
            else if (dealDto.Weight == CardWeight.QUEEN * 3)
            {
                Dispatch(AreoCode.AUDIO, AudioEvent.PLAY_EFFECT_AUDIO, "Fight/Woman_tuple12");
            }
            else if (dealDto.Weight == CardWeight.KING * 3)
            {
                Dispatch(AreoCode.AUDIO, AudioEvent.PLAY_EFFECT_AUDIO, "Fight/Woman_tuple13");
            }
            else if (dealDto.Weight == CardWeight.A * 3)
            {
                Dispatch(AreoCode.AUDIO, AudioEvent.PLAY_EFFECT_AUDIO, "Fight/Woman_tuple14");
            }
            else if (dealDto.Weight == CardWeight.TWO * 3)
            {
                Dispatch(AreoCode.AUDIO, AudioEvent.PLAY_EFFECT_AUDIO, "Fight/Woman_tuple15");
            }
            else if (dealDto.Type == CardType.BOOM)
            {
                Dispatch(AreoCode.AUDIO, AudioEvent.PLAY_EFFECT_AUDIO, "Fight/Woman_zhadan");
            }
            else if (dealDto.Type == CardType.JOKER_BOOM)
            {
                Dispatch(AreoCode.AUDIO, AudioEvent.PLAY_EFFECT_AUDIO, "Fight/Woman_wangzha");
            }
        }
    }

    private void processDealSres(bool active)
    {
        if (active)
        {
            Dispatch(AreoCode.UI, UIEvent.SHOW_DEAL_BUTTON, false);
        }
    }

    /// <summary>
    /// 处理转换出牌请求
    /// </summary>
    /// <param name="uid"></param>
    private void processTurnDeal(int uid)
    {
        if (uid == GameModles.Instance.userDto.ID)
        {
            Dispatch(AreoCode.UI, UIEvent.SHOW_DEAL_BUTTON, true);
        }
    }

    private void processGrabLandlord(LandLordDto landLordDto)
    {
        //播放抢地主声音
        Dispatch(AreoCode.AUDIO, AudioEvent.PLAY_EFFECT_AUDIO, "Fight/Woman_Order");
        //设置地主身份
        Dispatch(AreoCode.UI, UIEvent.PLAYER_CHANGE_IDENTITY, landLordDto.UserId);
        //设置底牌
        Dispatch(AreoCode.UI, UIEvent.SET_TABLE_CARD, landLordDto.TableCardList);
        //给地主发底牌
        addTableCard(landLordDto);
    }

    private void addTableCard(LandLordDto landLordDto)
    {
        int uid = landLordDto.UserId;
        /*if (uid == GameModles.Instance.userDto.ID)
        {
            Dispatch(AreoCode.CHARACTER, CharacterEvent.ADD_MY_TABLECARDS, landLordDto.TableCardList);
        }
        else if(uid == GameModles.Instance.matchRoomDto.Leftid)
        {
            Dispatch(AreoCode.CHARACTER, CharacterEvent.ADD_LEFT_TABLECARDS, landLordDto.TableCardList);
        }
        else if(uid == GameModles.Instance.matchRoomDto.Rightid)
        {
            Dispatch(AreoCode.CHARACTER, CharacterEvent.ADD_RIGHT_TABLECARDS, landLordDto.TableCardList);
        }*/
    }

    /// <summary>
    /// 是否是第一个人开始叫地主
    /// </summary>
    private bool isfirstturn = true;
    //轮换抢地主(不叫)
    private void processTurnLandlord(int uid)
    {
        if (isfirstturn)
        {
            isfirstturn = false;//第一次时不播放不叫的音效
        }
        else
        {
            Dispatch(AreoCode.AUDIO, AudioEvent.PLAY_EFFECT_AUDIO, "Fight/Woman_NoOrder");
        }

        if (uid == GameModles.Instance.userDto.ID)
        {
            Dispatch(AreoCode.UI, UIEvent.SHOW_GRAB_BUTTON, true);
        }
    }

    private void processGetCard(List<CardDto> cardList)
    {
        //给自己创建牌
        Dispatch(AreoCode.CHARACTER, CharacterEvent.INIT_MY_CARDLIST, cardList);

        Dispatch(AreoCode.CHARACTER, CharacterEvent.INIT_LEFT_CARDLIST, "初始化左边");
        Dispatch(AreoCode.CHARACTER, CharacterEvent.INIT_RIGHT_CARDLIST, "初始化右边");
    }
    #endregion   
}
