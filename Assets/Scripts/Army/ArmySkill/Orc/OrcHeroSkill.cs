using UnityEngine;
using System.Collections;

public class OrcHeroSkill : ArmySkillBase
{
    private bool isturnFirst = true;//是否是本回合首次使用

    public bool isKillOther = false;//是否击杀了其他单位

    //public bool isUsed = false;//本回合是否使用过了

    public OrcHeroSkill()
    {
        //this.armyCtrl = armyCtrl;
        canPerTurn = true;
        isPassive = true;
        isNeedOtherDead = true;
        isUsed = false;
        isBind = true;
    }
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void TurnRefrsh()
    {
        isturnFirst = true;
        isKillOther = false;
        isUsed = false;
    }

    public override void ProcessOtherDead()
    {
        isKillOther = true;
    }

    /// <summary>
    /// 狂怒血脉
    /// </summary>
    public override void UseSkill()
    {
        if (isturnFirst)
        {
            //本回合首次使用
            armyCtrl.canAttack = true;
            isturnFirst = false;
            Dispatch(AreoCode.UI, UIEvent.PROMPT_PANEL_EVENTCODE, "铁血长老发动狂怒血脉");
        }
        else
        {
            if (isKillOther && !isUsed)//如果击杀其他单位了
            {
                armyCtrl.canAttack = true;
                armyCtrl.CanturnMove = true;

                isKillOther = false;
                isUsed = true;
                Dispatch(AreoCode.UI, UIEvent.PROMPT_PANEL_EVENTCODE, "铁血长老发动狂怒血脉");
            }
        }
    }
}
