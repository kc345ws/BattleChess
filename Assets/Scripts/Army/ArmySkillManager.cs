using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Protocol.Constants;
using Assets.Scripts.Army.ArmySkill.Orc;

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
                        armyCtrl.gameObject.AddComponent<OrcHeroSkill>();
                        // armyCtrl.armySkill = new OrcHeroSkill(ref armyCtrl);
                        armyCtrl.gameObject.GetComponent<OrcHeroSkill>().SetArmyCtrl(ref armyCtrl);
                        armyCtrl.armySkill = armyCtrl.gameObject.GetComponent<OrcHeroSkill>();
                        break;

                    case OrcArmyCardType.Raven_Shaman:
                        //armyCtrl.armySkill = new OrcRavenShamanSkill(ref armyCtrl);
                        armyCtrl.gameObject.AddComponent<OrcRavenShamanSkill>();
                        armyCtrl.gameObject.GetComponent<OrcRavenShamanSkill>().SetArmyCtrl(ref armyCtrl);
                        armyCtrl.armySkill = armyCtrl.gameObject.GetComponent<OrcRavenShamanSkill>();
                        break;

                    case OrcArmyCardType.Giant_mouthed_Frog://巨口蛙
                        armyCtrl.gameObject.AddComponent<OrcGiantSkill>();
                        armyCtrl.gameObject.GetComponent<OrcGiantSkill>().SetArmyCtrl(ref armyCtrl);
                        armyCtrl.armySkill = armyCtrl.gameObject.GetComponent<OrcGiantSkill>();
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
