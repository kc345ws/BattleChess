using Protocol.Code;
using Protocol.Constants;
using Protocol.Dto.Fight;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 指令卡管理类
/// </summary>
public class OrderCardManager:CharacterBase
{
    public static OrderCardManager Instance;

    private OrderCardManager() { }

    private CardCtrl selectOrderCardCtrl;//选中的指令卡

    private ArmyCtrl selectArmyCtrl;//选中的兵种控制器

    //private bool isNeedGetArmyCtrl = false;//是否需要从地图获取兵种卡控制器

    //private bool isUseCard = false;//是否要使用卡牌

    private MyArmyCtrlManager myArmyCtrlManager;//我的兵种控制集合

    private SocketMsg socketMsg;
    private MapPointDto mapPointDto;

    //private bool isSelectMulArmy = true;//是否需要选择重叠兵种
    //private int isContinueProcessID = -1;//需要继续处理的卡牌事件 0攻击 1修养

    public delegate void SelectArmyCtrlDelegate(ArmyCtrl armyCtrl);
    public SelectArmyCtrlDelegate armyCtrlDelegate;//选择单位控制器事件

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Bind(CharacterEvent.SELECT_ORDERCARD);
        //Bind(CharacterEvent.SET_MY_LAND_SKY);

        armyCtrlDelegate = processSelectArmyDelegate;

        myArmyCtrlManager = GetComponent<MyArmyCtrlManager>();
        mapPointDto = new MapPointDto();
        socketMsg = new SocketMsg();
    }

    private void Update()
    {
        /*if (isNeedGetArmyCtrl)
        {
            getArmyCtrlByMouse();
        }*/
        //selectArmyCtrl = getArmyCtrlByMouse();
        useCard();
    }

    public override void Execute(int eventcode, object message)
    {
        base.Execute(eventcode, message);
        switch (eventcode)
        {
            case CharacterEvent.SELECT_ORDERCARD:
                if(message !=null)
                    processSelectOrderCard(message as CardCtrl);
                else
                    processSelectOrderCard(null);
                break;

            /*case CharacterEvent.SET_MY_LAND_SKY:
                if((int)message == ArmyMoveType.LAND && selectArmyCtrl !=null)
                {
                    selectArmyCtrl = selectArmyCtrl.ArmymapPointCtrl.LandArmy.GetComponent<ArmyCtrl>();
                }
                else if((int)message == ArmyMoveType.SKY && selectArmyCtrl != null)
                {
                    selectArmyCtrl = selectArmyCtrl.ArmymapPointCtrl.SkyArmy.GetComponent<ArmyCtrl>();
                }
                continueProcess();
                break;*/
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
    /// 继续处理卡牌事件
    /// </summary>
    /*private void continueProcess()
    {
        if(isContinueProcessID == 0)
        {
            //如果使用攻击卡
            StartCoroutine(useAttack());
        }
        else if(isContinueProcessID == 1)
        {
            //如果使用修养卡
            StartCoroutine(useRest());
        }
    }*/

    /// <summary>
    /// 处理传送指令卡的事件
    /// </summary>
    /// <param name="cardCtrl"></param>
    private void processSelectOrderCard(CardCtrl cardCtrl)
    {
        selectOrderCardCtrl = cardCtrl;
    }

    private void useCard()
    {
        if(selectOrderCardCtrl == null)
        {
            return;
            //如果没有选择指令卡
        }

        if(selectOrderCardCtrl.cardDto.Name == OrderCardType.ATTACK)
        {
            //如果使用攻击卡
            StartCoroutine(useAttack());
        }
        else if(selectOrderCardCtrl.cardDto.Name == OrderCardType.REST)
        {
            //如果使用修养卡
            StartCoroutine(useRest());
        }
    }

    /// <summary>
    /// 修养协程
    /// </summary>
    /// <returns></returns>
    private IEnumerator useRest()
    {

        //开始选择兵种控制器

        //isNeedGetArmyCtrl = true;
        if (selectOrderCardCtrl == null)
        {
            Dispatch(AreoCode.UI, UIEvent.PROMPT_PANEL_EVENTCODE, "请先选择指令卡");
            yield break;
        }
        if (selectArmyCtrl == null)
        {
            //yield return new WaitUntil(isGetArmyCtrl);
            Dispatch(AreoCode.UI, UIEvent.PROMPT_PANEL_EVENTCODE, "请先选择使用单位");
            yield break;
            /*if (selectOrderCardCtrl == null)
            {
                yield break;
            }*/
        }
        

        /*if (isSelectMulArmy && SelectUseArmy())
        {
            //如果有两个单位
            isSelectMulArmy = false;
            isContinueProcessID = 1;
            yield break;
        }*/

        if (selectArmyCtrl.armyState.Hp >= selectArmyCtrl.armyState.MaxHp)
        {
            Dispatch(AreoCode.UI, UIEvent.PROMPT_PANEL_EVENTCODE, "已达到血量上限");
            //isNeedGetArmyCtrl = false;
            selectArmyCtrl = null;
            selectOrderCardCtrl = null;
            yield break;
        }

        foreach (var item in myArmyCtrlManager.CardCtrllist)
        {
            if (item.armyState.Name == selectArmyCtrl.armyState.Name)
            {
                //血量加一
                item.armyState.Hp++;
                //发送消息给其他人
                if (item.armyState.CanFly)
                {
                    //如果是飞行单位
                    mapPointDto.Change(item.armyState.Position, -1, -1, item.armyState.Race, item.armyState.Name);
                }
                else
                {
                    //如果是陆地单位
                    mapPointDto.Change(item.armyState.Position, item.armyState.Race, item.armyState.Name, -1, -1);
                }
                socketMsg.Change(OpCode.FIGHT, FightCode.DEAL_REST_CREQ, mapPointDto);
                Dispatch(AreoCode.NET, NetEvent.SENDMSG, socketMsg);
                Dispatch(AreoCode.UI, UIEvent.PROMPT_PANEL_EVENTCODE, "回复血量成功");
            }
        }
        //移除修养卡牌     
        Dispatch(AreoCode.CHARACTER, CharacterEvent.REMOVE_MY_CARDS, selectOrderCardCtrl.cardDto);
        //状态重置
        //isNeedGetArmyCtrl = false;
        selectArmyCtrl = null;
        selectOrderCardCtrl = null;
        //isSelectMulArmy = true;
        //isContinueProcessID = -1;
        //Dispatch(AreoCode.UI, UIEvent.CLOSE_ARMY_MENU_PANEL, "关闭面板");
    }

    /// <summary>
    /// 两个单位重合时选择使用的单位
    /// </summary>
    private bool SelectUseArmy()
    {
        if(selectArmyCtrl.ArmymapPointCtrl.LandArmy.GetComponent<ArmyCtrl>() != null
            && selectArmyCtrl.ArmymapPointCtrl.SkyArmy.GetComponent<ArmyCtrl>() != null)
        {
            //如果该地块上有两个我方单位
            Dispatch(AreoCode.UI, UIEvent.SELECT_MY_LAND_SKY, false);
            return true;
        }
        return false;
    }

    /// <summary>
    /// 攻击协程
    /// </summary>
    /// <returns></returns>
    private IEnumerator useAttack()
    {

        //开始选择兵种控制器

        //isNeedGetArmyCtrl = true;
        if (selectOrderCardCtrl == null)
        {
            Dispatch(AreoCode.UI, UIEvent.PROMPT_PANEL_EVENTCODE, "请先选择指令卡");
            yield break;
        }
        if (selectArmyCtrl == null)
        {
            //yield return new WaitUntil(isGetArmyCtrl);
            Dispatch(AreoCode.UI, UIEvent.PROMPT_PANEL_EVENTCODE, "请先选择使用单位");
            yield break;
            /*if (selectOrderCardCtrl == null)
            {
                yield break;
            }*/
        }
        /*if (selectArmyCtrl == null)
        {
            yield return new WaitUntil(isGetArmyCtrl);
            if (selectOrderCardCtrl == null)
            {
                yield break;
            }
        }*/

        /*if (isSelectMulArmy && SelectUseArmy())
        {
            //如果有两个单位
            isSelectMulArmy = false;
            isContinueProcessID = 0;
            yield break;
        }*/


        if (selectArmyCtrl.armyState.Class == ArmyClassType.Ordinary)
        {
            Dispatch(AreoCode.UI, UIEvent.PROMPT_PANEL_EVENTCODE, "普通兵种不能使用攻击卡");
            //isNeedGetArmyCtrl = false;
            selectArmyCtrl = null;
            selectOrderCardCtrl = null;
            yield break;
        }

        foreach (var item in myArmyCtrlManager.CardCtrllist)
        {
            if(item.armyState.Name == selectArmyCtrl.armyState.Name)
            {
                //将该兵种设置为能攻击
                item.canAttack = true;
            }
        }
        //移除攻击卡牌     
        Dispatch(AreoCode.CHARACTER, CharacterEvent.REMOVE_MY_CARDS, selectOrderCardCtrl.cardDto);
        //状态重置
        //isNeedGetArmyCtrl = false;
        selectArmyCtrl = null;
        selectOrderCardCtrl = null;
        //isSelectMulArmy = true;
        //isContinueProcessID = -1;
        //Dispatch(AreoCode.UI, UIEvent.CLOSE_ARMY_MENU_PANEL, "关闭面板");
    }

    /// <summary>
    /// 是否选择了兵种卡
    /// </summary>
    /// <returns></returns>
    private bool isGetArmyCtrl()
    {
        if(selectArmyCtrl != null)
        {          
            return true;
        }
        //getArmyCtrlByMouse();
        return false;
    }

    /// <summary>
    /// 获得鼠标点击兵种控制器
    /// </summary>
    private ArmyCtrl getArmyCtrlByMouse()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                bool isCollider = Physics.Raycast(ray, out hit, 1000, LayerMask.GetMask("MyArmy"));
                if (isCollider)
                {
                    //selectArmyCtrl = hit.collider.gameObject.GetComponent<ArmyCtrl>();
                    
                    return hit.collider.gameObject.GetComponent<ArmyCtrl>();
                }
            }
        }
        return null;
    }
}

