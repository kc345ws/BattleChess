using UnityEngine;
using System.Collections;
using Protocol.Constants.Map;
using Protocol.Code;
using System.Collections.Generic;

/// <summary>
/// 乌鸦萨满技能
/// </summary>
public class OrcRavenShamanSkill : ArmySkillBase
{
    private ArmyCtrl healArmyctrl = null;//选中的治疗单位

    //private bool isStartSelect = false;//是否开始治疗单位
    // Use this for initialization

    Color healcolor = new Color(95f / 255f, 242f / 255f, 10f / 255f);
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
        List<MapPoint> canhealPoints = MapSkillType.Instance.GetSkillRange(armyCtrl.armyState);
        List<MapPointCtrl> canhealMapCtrls = MapTools.getMapCtrlByMapPoint(canhealPoints);
        MapTools.setMappointCtrlColor(canhealMapCtrls,healcolor);
        yield return new WaitUntil(isSelecthealArmy);


        bool canHeal = false;
        foreach (var item in canhealMapCtrls)
        {
            if (healArmyctrl.armyState.Position.X == item.mapPoint.X
                && healArmyctrl.armyState.Position.Z == item.mapPoint.Z)
            {
                canHeal = true;
                break;
            }
        }
        if (!canHeal)
        {
            Dispatch(AreoCode.UI, UIEvent.CURSOR_SET_NORMAL, "正常光标");
            Dispatch(AreoCode.UI, UIEvent.PROMPT_PANEL_EVENTCODE, "不在治疗范围内");
            MapTools.setMappointCtrlColor(canhealMapCtrls);
            healArmyctrl = null;
            yield break;
        }

        if (healArmyctrl.armyState.Hp >= healArmyctrl.armyState.MaxHp)
        {
            Dispatch(AreoCode.UI, UIEvent.CURSOR_SET_NORMAL, "正常光标");
            Dispatch(AreoCode.UI, UIEvent.PROMPT_PANEL_EVENTCODE, "该单位血量已经达到上限");
            MapTools.setMappointCtrlColor(canhealMapCtrls);
            healArmyctrl = null;
            yield break;
        }
              
        

        


        healArmyctrl.armyState.Hp++;
        Dispatch(AreoCode.UI, UIEvent.CURSOR_SET_NORMAL, "正常光标");
        Dispatch(AreoCode.UI, UIEvent.PROMPT_PANEL_EVENTCODE, "治疗成功");

        

        Dispatch(AreoCode.UI, UIEvent.CLOSE_ARMY_MENU_PANEL, "关闭单位功能面板");
        Dispatch(AreoCode.UI, UIEvent.SHOW_ARMY_MENU_PANEL, armyCtrl);
        isUsed = true;

        //给对方发送消息
        MapPoint targetMappoint = healArmyctrl.armyState.Position;
        skillDto.Change(armyCtrl.armyState.Race, armyCtrl.armyState.Name, healArmyctrl.armyState.Name, targetMappoint);
        socketMsg.Change(OpCode.FIGHT, FightCode.ARMY_USE_SKILL_CREQ, skillDto);
        Dispatch(AreoCode.NET, NetEvent.SENDMSG, socketMsg);

        MapTools.setMappointCtrlColor(canhealMapCtrls);
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
