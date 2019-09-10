using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Protocol.Dto.Fight;
using Protocol.Constants;
using Protocol.Constants.Orc.OtherCard;
using Protocol.Code;

/// <summary>
/// 其他人的兵种管理器集合
/// </summary>
public class OtherArmyCtrlManager : ArmyBase
{
    /// <summary>
    /// 兵种集合
    /// </summary>
    public static List<GameObject> ArmyList { get; private set; }

    /// <summary>
    /// 兵种控制器集合
    /// </summary>
    public static List<OtherArmyCtrl> OtherArmyCtrlList { get; private set; }

    private OtherCharacterCtrl otherCharacterCtrl;//卡牌控制器管理类

    //private Object Lock = new Object();


    //public static OtherArmyCtrlManager Instance = null;

    /// <summary>
    /// 上一次选择的兵种
    /// </summary>
    public static OtherArmyCtrl LastSelectArmy;


    private SocketMsg socketMsg;

    private void Awake()
    {
        //Instance = this;
    }
    
    // Use this for initialization
    void Start()
    {
        ArmyList = new List<GameObject>();
        OtherArmyCtrlList = new List<OtherArmyCtrl>();
        otherCharacterCtrl = GetComponent<OtherCharacterCtrl>();
        socketMsg = new SocketMsg();

        Bind(ArmyEvent.ADD_OTHER_ARMY);
        Bind(ArmyEvent.OTHER_USE_REST);
        Bind(ArmyEvent.OTHER_USE_OTHERCARD);
    }

    // Update is called once per frame
    void Update()
    {
        if(OtherArmyCtrlList.Count > 0)
        {
            checkArmyDead();
        }
        checkWin();
    }

    /// <summary>
    /// 检测是否胜利
    /// </summary>
    private void checkWin()
    {
        foreach (var item in OtherArmyCtrlList)
        {
            if(item.armyState.Class == ArmyClassType.Hero)
            {
                //如果敌方还有英雄单位
                return;
            }
        }

        foreach (var item in MapManager.OtherLineMapPointCtrls)
        {
            if(item.LandArmy!= null && item.LandArmy.GetComponent<ArmyCtrl>() != null)
            {
                //如果敌方胜利线有我方单位
                //发送消息
                socketMsg.Change(OpCode.FIGHT, FightCode.GAME_OVER_CREQ, "游戏结束");
                Dispatch(AreoCode.NET, NetEvent.SENDMSG, socketMsg);
                //显示结束面板
                Dispatch(AreoCode.UI, UIEvent.SHOW_WIN_OVER_PANEL, "游戏结束面板");
            }
            else if (item.SkyArmy != null && item.SkyArmy.GetComponent<ArmyCtrl>() !=null)
            {
                // 如果敌方胜利线有我方单位
                //发送消息
                socketMsg.Change(OpCode.FIGHT, FightCode.GAME_OVER_CREQ, "游戏结束");
                Dispatch(AreoCode.NET, NetEvent.SENDMSG, socketMsg);
                //显示结束面板
                Dispatch(AreoCode.UI, UIEvent.SHOW_WIN_OVER_PANEL, "游戏结束面板");
            }
        }
    }

    /// <summary>
    /// 检查单位是否死亡
    /// </summary>
    /// <returns></returns>
    private void checkArmyDead()
    {
        int index = 0;
        bool isdead = false;
        foreach (var item in OtherArmyCtrlList)
        {
            if (item.armyState.Hp <= 0)
            {
                Dispatch(AreoCode.UI, UIEvent.PROMPT_PANEL_EVENTCODE, "敌方单位死亡");
                if (item.armyState.CanFly)
                {
                    //如果是飞行单位
                    Destroy(item.OtherMapPintctrl.SkyArmy);
                    item.OtherMapPintctrl.RemoveSkyArmy();
                }
                else
                {
                    //如果是陆地单位
                    Destroy(item.OtherMapPintctrl.LandArmy);
                    item.OtherMapPintctrl.RemoveLandArmy();
                }
                
                isdead = true;
                break;
            }
            index++;
        }
        if (isdead)
        {
            OtherArmyCtrlList.RemoveAt(index);
        }
        isdead = false;
    }

    public override void Execute(int eventcode, object message)
    {
        switch (eventcode)
        {
            case ArmyEvent.ADD_OTHER_ARMY:
                //ArmyList.Add(message as GameObject);
                processAddArmy(message as GameObject);
                break;

            case ArmyEvent.OTHER_USE_REST:
                processRest(message as MapPointDto);
                break;

            case ArmyEvent.OTHER_USE_OTHERCARD:
                processOtherCard(message as CardDto);
                break;
        }
    }

    /// <summary>
    /// 处理非指令卡
    /// </summary>
    /// <param name="cardDto"></param>
    private void processOtherCard(CardDto cardDto)
    {
        switch (cardDto.Race)
        {
            case RaceType.ORC://兽族
                switch (cardDto.Name)
                {
                    case OrcOtherCardType.Ancestor_Helmets://先祖头盔
                        //使用
                        OtherCard_AncestorHelmets();
                        //提示
                        Dispatch(AreoCode.UI, UIEvent.PROMPT_PANEL_EVENTCODE, "对方使用先祖头盔");
                        //移除卡牌
                        Dispatch(AreoCode.CHARACTER, CharacterEvent.REMOVE_OTHER_CARDS, "移除手牌");
                        break;
                }

                break;
        }
    }

    #region 兽族
    /// <summary>
    /// 先祖头盔
    /// </summary>
    private void OtherCard_AncestorHelmets()
    {
        foreach (var item in OtherArmyCtrlList)
        {
            if(item.armyState.Name == OrcArmyCardType.Infantry)
            {
                //场上所有兽族步兵血量加一，最大血量不变
                lock (this)
                {
                    item.armyState.Hp++;
                }        
            }
        }
    }

    #endregion

    /// <summary>
    /// 处理使用修养
    /// </summary>
    /// <param name="mapPointDto"></param>
    private void processRest(MapPointDto mapPointDto)
    {
        //镜像对称
        int totalX = 12;
        int totalZ = 8;
        bool canfly = false;

        int realx = totalX - mapPointDto.mapPoint.X;
        int realz = totalZ - mapPointDto.mapPoint.Z;
        if (mapPointDto.LandArmyRace == -1)
        {
            //如果是飞行单位使用了修养
            canfly = true;
        }

        foreach (var item in OtherArmyCtrlList)
        {
            if(item.armyState.Position.X == realx && item.armyState.Position.Z == realz && item.armyState.CanFly == canfly)
            {
                item.armyState.Hp++;
                break;
            }
        }

        Dispatch(AreoCode.UI, UIEvent.PROMPT_PANEL_EVENTCODE, "对方(" + realx + "," + realz + ")使用修养卡");
        //移除卡牌
        Dispatch(AreoCode.CHARACTER, CharacterEvent.REMOVE_OTHER_CARDS, "移除手牌");
    }

    /// <summary>
    /// 处理添加单位
    /// </summary>
    /// <param name="army"></param>
    private void processAddArmy(GameObject army)
    {
        OtherArmyCtrl otherArmyCtrl = army.gameObject.GetComponent<OtherArmyCtrl>();
        //therArmyCtrl.Init();
        otherArmyCtrl.ArmySelectDelegate = processOtherArmySelect;

        OtherArmyCtrlList.Add(otherArmyCtrl);
        ArmyList.Add(army);
    }

    private bool processOtherArmySelect(OtherArmyCtrl otherArmyCtrl)
    {
        if(LastSelectArmy == null)
        {
            //如果是第一次选择           
            otherArmyCtrl.isSelect = true;
            LastSelectArmy = otherArmyCtrl;
            return true;
        }
        else if(LastSelectArmy != otherArmyCtrl)
        {
            //和上一次不相等
            LastSelectArmy.isSelect = false;
            otherArmyCtrl.isSelect = true;
            LastSelectArmy = otherArmyCtrl;
            return true;
        }
        else
        {
            //和上一次相等
            LastSelectArmy.isSelect = false;
            LastSelectArmy = null;
            return false;
        }
    }
}
