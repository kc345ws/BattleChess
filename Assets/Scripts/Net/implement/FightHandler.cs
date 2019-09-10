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

            case FightCode.SELECT_RACE_SBOD://选择种族
                processSelectRaceSBOD();
                break;

            case FightCode.GET_CARD_SRES://服务器给客户端发牌
                processGetCard(message as List<CardDto>);
                break;

            case FightCode.MAP_SET_ARMY_SBOD://地图放置单位
                processMapSetArmySbod(message as MapPointDto);
                break;

            case FightCode.DEAL_ARMYCARD_SBOD://其他人使用兵种卡
                processDealCard(message as ClientPeer);
                break;

            case FightCode.MAP_ARMY_MOVE_SBOD://单位移动
                processArmyMove(message as MapMoveDto);
                break;

            case FightCode.ARMY_ATTACK_SBOD://单位攻击
                processArmyAttack(message as MapAttackDto);
                break;

            case FightCode.DEAL_DODGE_SBOD://闪避
                processDodgeSbod((bool)message);
                break;

            case FightCode.DEAL_BACKATTACK_SBOD://反击
                processBackAttackSBOD((bool)message);
                break;

            case FightCode.DEAL_REST_SBOD://修养
                processRestSBOD(message as MapPointDto);
                break;

            case FightCode.USE_OTHERCARD_SBOD://非指令卡
                processOtherCard(message as CardDto);
                break;

            case FightCode.DEAL_ATTACK_SBOD://攻击卡
                processAttackCard();
                break;

            case FightCode.NEXT_TURN_SBOD://轮到某人开始下一回合
                processNextTurn((int)message);
                break;

            case FightCode.ADD_CARD_SRES://服务器给自己发牌
                processAddCardSRES(message as List<CardDto>);
                break;

            case FightCode.ADD_CARD_SBOD://服务器给其他人发牌
                processAddCardSBOD((int)message);
                break;

            case FightCode.GAME_OVER_SBOD:
                processGameOver();
                break;
        }
    }

    /// <summary>
    /// 处理游戏结束
    /// </summary>
    private void processGameOver()
    {
        Dispatch(AreoCode.UI, UIEvent.SHOW_LOSE_OVER_PANEL, "游戏失败");
    }

    /// <summary>
    /// 处理服务器给其他人发牌
    /// </summary>
    /// <param name="cardCount">手牌数量</param>
    private void processAddCardSBOD(int cardCount)
    {
        Dispatch(AreoCode.CHARACTER, CharacterEvent.ADD_OTHERT_CARDS, cardCount);
    }

    /// <summary>
    /// 处理服务器给自己发牌
    /// </summary>
    /// <param name="cardlist"></param>
    private void processAddCardSRES(List<CardDto> cardlist)
    {
        Dispatch(AreoCode.CHARACTER, CharacterEvent.ADD_MY_CARDS, cardlist);
    }

    /// <summary>
    /// 处理下一回合开始
    /// </summary>
    /// <param name="uid"></param>
    private void processNextTurn(int uid)
    {
        Dispatch(AreoCode.UI, UIEvent.NEXT_TURN, uid);
    }

    /// <summary>
    /// 处理攻击卡
    /// </summary>
    private void processAttackCard()
    {
        Dispatch(AreoCode.CHARACTER, CharacterEvent.OTHER_DEAL_ATTACK, "使用了攻击卡");
    }

    /// <summary>
    /// 处理别人使用非指令卡
    /// </summary>
    /// <param name="cardDto"></param>
    private void processOtherCard(CardDto cardDto)
    {
        Dispatch(AreoCode.ARMY, ArmyEvent.OTHER_USE_OTHERCARD, cardDto);
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
        Dispatch(AreoCode.UI, UIEvent.SHOW_LOSE_OVER_PANEL, true);
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
