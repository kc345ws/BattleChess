using Assets.Scripts.Net;
using Protocol.Code;
using Protocol.Constants;
using Protocol.Constants.Map;
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

            case FightCode.GET_CARD_SRES://服务器给客户端发牌
                processGetCard(message as List<CardDto>);
                break;

            case FightCode.MAP_SET_ARMY_SBOD:
                processMapSetArmySbod(message as MapPointDto);
                break;

            case FightCode.DEAL_CARD_SBOD:
                processDealCard(message as ClientPeer);
                break;

            case FightCode.MAP_ARMY_MOVE_SBOD:
                processArmyMove(message as MapMoveDto);
                break;

            case FightCode.ARMY_ATTACK_SBOD:
                processArmyAttack(message as MapAttackDto);
                break;

            case FightCode.DEAL_DODGE_SBOD:
                processDodgeSbod((bool)message);
                break;

            case FightCode.DEAL_BACKATTACK_SBOD:
                processBackAttackSBOD((bool)message);
                break;

            case FightCode.DEAL_REST_SBOD:
                processRestSBOD(message as MapPointDto);
                break;
        }
    }

    /// <summary>
    /// 处理修养广播
    /// </summary>
    /// <param name="mapPointDto"></param>
    private void processRestSBOD(MapPointDto mapPointDto)
    {
        Dispatch(AreoCode.ARMY, ArmyEvent.OTHER_USE_REST, mapPointDto);
    }

    /// <summary>
    /// 处理反击广播
    /// </summary>
    /// <param name="active"></param>
    private void processBackAttackSBOD(bool active)
    {
        if (active)
        {
            //如果反击了
            Dispatch(AreoCode.UI, UIEvent.IS_BACKATTACK, true);
        }
        else
        {
            Dispatch(AreoCode.UI, UIEvent.IS_BACKATTACK, false);
        }
    }
    
    /// <summary>
    /// 处理闪避广播
    /// </summary>
    private void processDodgeSbod(bool active)
    {
        if (active)
        {
            //如果出了闪避,攻击失败
            Dispatch(AreoCode.UI, UIEvent.IS_ATTACK_SUCCESS, false);
        }
        else
        {
            Dispatch(AreoCode.UI, UIEvent.IS_ATTACK_SUCCESS, true);
        }
    }

    /// <summary>
    /// 处理其他人兵种攻击自己兵种的请求
    /// </summary>
    private void processArmyAttack(MapAttackDto mapAttackDto)
    {
        Dispatch(AreoCode.CHARACTER, CharacterEvent.INQUIRY_DEAL_DODGE, mapAttackDto);
    }
    
    /// <summary>
    /// 处理其他人的兵种移动
    /// </summary>
    /// <param name="mapMoveDto"></param>
    private void processArmyMove(MapMoveDto mapMoveDto)
    {
        Dispatch(AreoCode.MAP, MapEvent.MOVE_OTHER_ARMY, mapMoveDto);
    }

    /// <summary>
    /// 处理其他人的出牌
    /// </summary>
    /// <param name="clientPeer"></param>
    private void processDealCard(ClientPeer clientPeer)
    {
        Dispatch(AreoCode.CHARACTER, CharacterEvent.REMOVE_OTHER_CARDS, 1);
    }

    /// <summary>
    /// 处理设置其他人兵种
    /// </summary>
    /// <param name="mapPointDto"></param>
    private void processMapSetArmySbod(MapPointDto mapPointDto)
    {
        Dispatch(AreoCode.MAP, MapEvent.SET_OTHER_ARMY, mapPointDto);
    }

    /// <summary>
    /// 处理选择种族
    /// </summary>
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

    private void processGetCard(List<CardDto> cardList)
    {
        //给自己创建牌
        Dispatch(AreoCode.CHARACTER, CharacterEvent.INIT_MY_CARDLIST, cardList);

        Dispatch(AreoCode.CHARACTER, CharacterEvent.INIT_OTHER_CARDLIST, "初始化其他玩家的手牌");
        //Dispatch(AreoCode.CHARACTER, CharacterEvent.INIT_RIGHT_CARDLIST, "初始化右边");
    }
    #endregion   
}
