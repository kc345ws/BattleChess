using Protocol.Code;
using Protocol.Constants;
using Protocol.Constants.Orc.OtherCard;
using Protocol.Dto.Fight;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 非指令卡管理类
/// </summary>
public class OtherCardManager : CharacterBase
{
    public static OtherCardManager Instance;

    private OtherCardManager() { }

    private CardCtrl selectOtherCardCtrl;//选中的非指令卡

    private ArmyCtrl selectArmyCtrl;//选中的兵种控制器

    private MyArmyCtrlManager myArmyCtrlManager;//我的兵种控制集合

    private MyCharacterCtrl myCharacterCtrl;//我的角色控制器

    public delegate void SelectArmyCtrlDelegate(ArmyCtrl armyCtrl);
    public SelectArmyCtrlDelegate selectArmyCtrlDelegate;//选择单位控制器事件

    private SocketMsg socketMsg;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        Bind(CharacterEvent.SELECT_OTHERCARD);

        myArmyCtrlManager = GetComponent<MyArmyCtrlManager>();
        myCharacterCtrl = GetComponent<MyCharacterCtrl>();
        selectArmyCtrlDelegate = processSelectArmyDelegate;
        socketMsg = new SocketMsg();
    }

    // Update is called once per frame
    void Update()
    {
        useCard();
    }

    public override void Execute(int eventcode, object message)
    {
        base.Execute(eventcode, message);
        switch (eventcode)
        {
            case CharacterEvent.SELECT_OTHERCARD:
                if (message != null)
                    processSelectOtherCard(message as CardCtrl);
                else
                    processSelectOtherCard(null);
                break;
        }
    }

    /// <summary>
    /// 处理传送单位控制器委托
    /// </summary>
    /// <param name="armyCtrl"></param>
    private void processSelectArmyDelegate(ArmyCtrl armyCtrl)
    {
        selectArmyCtrl = armyCtrl;
    }

    /// <summary>
    /// 处理选中非指令卡事件
    /// </summary>
    /// <param name="cardCtrl"></param>
    private void processSelectOtherCard(CardCtrl cardCtrl)
    {
        selectOtherCardCtrl = cardCtrl;
    }

    private void useCard()
    {
        if (selectOtherCardCtrl == null || selectOtherCardCtrl.cardDto.Type != CardType.OTHERCARD)
        {
            return;
            //如果没有选择指令卡
        }

        switch (selectOtherCardCtrl.cardDto.Race)
        {
            case RaceType.ORC:
                //如果使用兽族的非指令卡
                useOrcOtherCard(selectOtherCardCtrl.cardDto.Name);
                break;
        }     
    }

    private void useOrcOtherCard(int name)
    {
        if (name == OrcOtherCardType.Ancestor_Helmets)
        {
            //如果使用先祖头盔
            StartCoroutine(AncestorHelmets());
        }
        else if(name == OrcOtherCardType.Totem_summon)
        {
            //召唤图腾
            StartCoroutine(Totem_Summon());
        }
    }

    #region 兽族
    /// <summary>
    /// 先祖头盔协程
    /// </summary>
    /// <returns></returns>
    private IEnumerator AncestorHelmets()
    {
        //开始选择兵种控制器
        if (selectOtherCardCtrl == null)
        {
            Dispatch(AreoCode.UI, UIEvent.PROMPT_PANEL_EVENTCODE, "请先选择非指令卡");
            yield break;
        }

        bool hasInfantry = false;
        foreach (var item in myArmyCtrlManager.CardCtrllist)
        {
            if(item.armyState.Name == OrcArmyCardType.Infantry)
            {
                //如果场上有兽族步兵
                hasInfantry = true;
                break;
            }
        }

        if (!hasInfantry)
        {
            //如果场上没有兽族步兵
            Dispatch(AreoCode.UI, UIEvent.PROMPT_PANEL_EVENTCODE, "场上没有兽族步兵");
            yield break;
        }
        /*if (selectArmyCtrl == null)
        {
            Dispatch(AreoCode.UI, UIEvent.PROMPT_PANEL_EVENTCODE, "请先选择使用单位");
            yield break;
        }*/


        foreach (var item in myArmyCtrlManager.CardCtrllist)
        {
            if (item.armyState.Name == OrcArmyCardType.Infantry)
            {
                //所有在场上的兽族步兵血量加一，最大血量不变
                item.armyState.Hp++;
            }
        }
        //发送消息给其他人
        socketMsg.Change(OpCode.FIGHT, FightCode.USE_OTHERCARD_CREQ, selectOtherCardCtrl.cardDto);
        Dispatch(AreoCode.NET, NetEvent.SENDMSG, socketMsg);
        //提示消息
        Dispatch(AreoCode.UI, UIEvent.PROMPT_PANEL_EVENTCODE, "先祖头盔使用成功");
        //移除先祖头盔卡牌     
        Dispatch(AreoCode.CHARACTER, CharacterEvent.REMOVE_MY_CARDS, selectOtherCardCtrl.cardDto);
        //状态重置
        selectArmyCtrl = null;
        selectOtherCardCtrl = null;
    }


    /// <summary>
    /// 召唤图腾
    /// </summary>
    /// <returns></returns>
    private IEnumerator Totem_Summon()
    {
        if (selectOtherCardCtrl == null)
        {
            Dispatch(AreoCode.UI, UIEvent.PROMPT_PANEL_EVENTCODE, "请先选择非指令卡");
            yield break;
        }

        //发送消息给服务器
        socketMsg.Change(OpCode.FIGHT, FightCode.USE_OTHERCARD_CREQ, selectOtherCardCtrl.cardDto);
        Dispatch(AreoCode.NET, NetEvent.SENDMSG, socketMsg);
        //移除卡牌     
        Dispatch(AreoCode.CHARACTER, CharacterEvent.REMOVE_MY_CARDS, selectOtherCardCtrl.cardDto);
    }
    #endregion
}
