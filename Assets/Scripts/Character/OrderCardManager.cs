using Protocol.Constants;
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
    private CardCtrl selectOrderCardCtrl;//选中的指令卡

    private ArmyCtrl selectArmyCtrl;//选中的兵种控制器

    private bool isNeedGetArmyCtrl = false;//是否需要从地图获取兵种卡控制器

    //private bool isUseCard = false;//是否要使用卡牌

    private MyArmyCtrlManager myArmyCtrlManager;//我的兵种控制集合

    private void Start()
    {
        Bind(CharacterEvent.SELECT_ORDERCARD);

        myArmyCtrlManager = GetComponent<MyArmyCtrlManager>();
    }

    private void Update()
    {
        /*if (isNeedGetArmyCtrl)
        {
            getArmyCtrlByMouse();
        }*/
        selectArmyCtrl = getArmyCtrlByMouse();
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
        }
    }

    /// <summary>
    /// 处理传中指令卡的事件
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
        else if(selectOrderCardCtrl.cardDto.Name == OrderCardType.DODGE)
        {
            //如果使用闪避卡
        }
    }

    private IEnumerator useAttack()
    {
        
        //开始选择兵种控制器

        isNeedGetArmyCtrl = true;
        if (selectArmyCtrl == null)
        {
            yield return new WaitUntil(isGetArmyCtrl);
            if (selectOrderCardCtrl == null)
            {
                yield break;
            }
        }
        

        if (selectArmyCtrl.armyState.Class == ArmyClassType.Ordinary)
        {
            Dispatch(AreoCode.UI, UIEvent.PROMPT_PANEL_EVENTCODE, "普通兵种不能使用攻击卡");
            isNeedGetArmyCtrl = false;
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
        isNeedGetArmyCtrl = false;
        selectArmyCtrl = null;
        selectOrderCardCtrl = null;
        //Dispatch(AreoCode.UI, UIEvent.CLOSE_ARMY_MENU_PANEL, "关闭面板");
    }

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

