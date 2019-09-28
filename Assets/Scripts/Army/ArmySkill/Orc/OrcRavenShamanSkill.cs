using UnityEngine;
using System.Collections;

/// <summary>
/// 乌鸦萨满技能
/// </summary>
public class OrcRavenShamanSkill : ArmySkillBase
{
    private ArmyCtrl healArmyctrl = null;//选中的治疗单位

    //private bool isStartSelect = false;//是否开始治疗单位
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        healArmyctrl = getArmyCtrlByMouse();
    }

    public OrcRavenShamanSkill()
    {
        //this.armyCtrl = armyCtrl;
        canPerTurn = true;
        isPassive = false;
        isNeedOtherDead = false;
        isUsed = false;
        isBind = true;
    }

    public override void TurnRefrsh()
    {
        //base.TurnRefrsh();
        isUsed = false;
    }

    public override void UseSkill()
    {
        //base.UseSkill();
        if (!isUsed)
        {
            StartCoroutine(heal());           
        }
    }

    private IEnumerator heal()
    {
        healArmyctrl = null;
        //isStartSelect = true;

        Dispatch(AreoCode.UI, UIEvent.CURSOR_SET_HEART, "治疗光标");
        yield return new WaitUntil(isSelecthealArmy);

        if(healArmyctrl.armyState.Hp >= healArmyctrl.armyState.MaxHp)
        {
            Dispatch(AreoCode.UI, UIEvent.CURSOR_SET_NORMAL, "正常光标");
            Dispatch(AreoCode.UI, UIEvent.PROMPT_PANEL_EVENTCODE, "该单位血量已经达到上限");
            yield break;
        }


        healArmyctrl.armyState.Hp++;
        Dispatch(AreoCode.UI, UIEvent.CURSOR_SET_NORMAL, "正常光标");
        Dispatch(AreoCode.UI, UIEvent.PROMPT_PANEL_EVENTCODE, "治疗成功");

        //给对方发送消息

        Dispatch(AreoCode.UI, UIEvent.CLOSE_ARMY_MENU_PANEL, "关闭单位功能面板");
        Dispatch(AreoCode.UI, UIEvent.SHOW_ARMY_MENU_PANEL, armyCtrl);
        isUsed = true;
    }

    /// <summary>
    /// 是否选择了要治疗的单位
    /// </summary>
    /// <returns></returns>
    private bool isSelecthealArmy()
    {
        if(healArmyctrl != null)
        {
            return true;
        }
        return false;
    }
}
