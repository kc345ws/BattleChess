using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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


    //public static OtherArmyCtrlManager Instance = null;

    /// <summary>
    /// 上一次选择的兵种
    /// </summary>
    public static OtherArmyCtrl LastSelectArmy;

    private void Awake()
    {
        //Instance = this;
    }
    
    // Use this for initialization
    void Start()
    {
        ArmyList = new List<GameObject>();
        OtherArmyCtrlList = new List<OtherArmyCtrl>();

        Bind(ArmyEvent.ADD_OTHER_ARMY);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void Execute(int eventcode, object message)
    {
        switch (eventcode)
        {
            case ArmyEvent.ADD_OTHER_ARMY:
                //ArmyList.Add(message as GameObject);
                processAddArmy(message as GameObject);
                break;
        }
    }

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
