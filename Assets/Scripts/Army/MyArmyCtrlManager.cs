using Protocol.Constants;
using Protocol.Dto.Fight;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyArmyCtrlManager : ArmyBase
{
    public List<ArmyCtrl> CardCtrllist { get; private set; }//兵种控制器集合

    private ArmyCtrl LastSelectArmyCtrl;//上一个选择的兵种

    public static MyArmyCtrlManager Instance;
    private MyArmyCtrlManager() { }

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        CardCtrllist = new List<ArmyCtrl>();

        Bind(ArmyEvent.TURN_REFRESH_ARMYSTATE);
    }

    // Update is called once per frame
    void Update()
    {
        //检测是否有单位死亡
        if(CardCtrllist.Count > 0)
        {
            checkArmyDead();
        }
        
    }

    public override void Execute(int eventcode, object message)
    {
        base.Execute(eventcode, message);
        switch (eventcode)
        {
            case ArmyEvent.TURN_REFRESH_ARMYSTATE:
                processRefreshState();
                break;
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
        foreach (var item in CardCtrllist)
        {
            if(item.armyState.Hp <= 0)
            {
                Dispatch(AreoCode.UI, UIEvent.PROMPT_PANEL_EVENTCODE, "我方单位死亡");
                if (item.armyState.MoveType == ArmyMoveType.SKY)
                {

                    //如果是飞行单位
                    Destroy(item.ArmymapPointCtrl.SkyArmy);
                    item.ArmymapPointCtrl.RemoveSkyArmy();
                    
                }
                else
                {
                    //如果是陆地单位
                    Destroy(item.ArmymapPointCtrl.LandArmy);
                    item.ArmymapPointCtrl.RemoveLandArmy();
                }
                
                isdead = true;
                break;
            }
            index++;
        }
        if (isdead)
        {
            CardCtrllist.RemoveAt(index);
        }
        isdead = false;
    }

    /// <summary>
    /// 处理刷新单位状态
    /// </summary>
    private void processRefreshState()
    {
        foreach (var item in CardCtrllist)
        {
            item.CanturnMove = true;//所有单位可以移动
            if(item.armyState.Class == ArmyClassType.Ordinary)
            {
                item.canAttack = true;
                //item.isAttack = false;
            }
            else
            {
                item.canAttack = false;
                //item.isAttack = false;
            }
        }
    }

    public void Add(ArmyCtrl armyCtrl)
    {
        //armyCtrl.Init(cardDto, mapPointCtrl , armyprefab);
        armyCtrl.ArmySelectEvent = processArmySelect;
        CardCtrllist.Add(armyCtrl);
        
    }


    private bool processArmySelect(ArmyCtrl armyCtrl)
    {
        if(LastSelectArmyCtrl == null)
        {
            //第一次选择
            LastSelectArmyCtrl = armyCtrl;
            armyCtrl.isSelect = true;
            return true;
        }
        else if(armyCtrl != LastSelectArmyCtrl)
        {
            //和上次选择不一样
            LastSelectArmyCtrl.isSelect = false;
            LastSelectArmyCtrl.CheckIsA();
            //LastSelectArmyCtrl.transform.localScale /= 2;

            armyCtrl.isSelect = true;
            LastSelectArmyCtrl = armyCtrl;
            return true;
        }
        else
        {
            //和上次选择一样
            LastSelectArmyCtrl.isSelect = false;
            LastSelectArmyCtrl.CheckIsA();
            LastSelectArmyCtrl = null;
            return false;
        }
    }
}
