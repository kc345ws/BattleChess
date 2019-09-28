using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Protocol.Constants;

/// <summary>
/// 兵种技能管理类
/// </summary>
public class ArmySkillManager : ArmyBase
{

    public static ArmySkillManager Instance;


    private ArmySkillManager() { }

    //public ArmyCardBase armyState;//兵种属性

    private void Awake()
    {
        Instance = this;
    }
    // Use this for initialization
    void Start()
    {
        Bind(ArmyEvent.TURN_REFRESH_ARMYSKILL);
        Bind(ArmyEvent.ARMY_DEAD);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void Execute(int eventcode, object message)
    {
        base.Execute(eventcode, message);
        switch (eventcode)
        {
            case ArmyEvent.TURN_REFRESH_ARMYSKILL:

                foreach (var item in MyArmyCtrlManager.Instance.CardCtrllist)
                {
                    if (item.armySkill.canPerTurn)
                    {
                        item.armySkill.TurnRefrsh();//刷新单位技能状态
                    }
                }

                break;

            case ArmyEvent.ARMY_DEAD:

                foreach (var item in MyArmyCtrlManager.Instance.CardCtrllist)
                {
                    if (item.armySkill.isNeedOtherDead)
                    {
                        //item.armySkill.UseSkill();
                        item.armySkill.ProcessOtherDead();
                    }
                }

                break;
        }
    }

    /// <summary>
    /// 绑定技能
    /// </summary>
    /// <param name="armyState"></param>
    public void BindSkill(ref ArmyCtrl armyCtrl)
    {
        //this.armyState = armyState;
        switch (armyCtrl.armyState.Race)
        {
            case RaceType.ORC:
                switch (armyCtrl.armyState.Name)
                {
                    case OrcArmyCardType.Hero:
                        armyCtrl.armySkill = new OrcHeroSkill(ref armyCtrl);

                        break;

                    default:

                        armyCtrl.armySkill = new ArmySkillBase();
                        break;
                }
                break;

            default:
                armyCtrl.armySkill = new ArmySkillBase();
                break;
        }
    }

}
