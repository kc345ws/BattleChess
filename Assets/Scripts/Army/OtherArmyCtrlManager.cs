using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Protocol.Dto.Fight;

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
        Bind(ArmyEvent.OTHER_USE_REST);
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

            case ArmyEvent.OTHER_USE_REST:
                processRest(message as MapPointDto);
                break;
        }
    }

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
