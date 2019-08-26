using Protocol.Dto.Fight;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyArmyCtrlManager : ArmyBase
{
    private List<ArmyCtrl> CardCtrllist;//兵种控制器集合

    private ArmyCtrl LastSelectArmyCtrl;//上一个选择的兵种

    // Start is called before the first frame update
    void Start()
    {
        CardCtrllist = new List<ArmyCtrl>();

    }

    // Update is called once per frame
    void Update()
    {
        
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
