using Protocol.Code;
using Protocol.Constants;
using Protocol.Constants.Map;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// 巨口蛙技能
/// </summary>
namespace Assets.Scripts.Army.ArmySkill.Orc
{
    public class OrcGiantSkill : ArmySkillBase
    {
        private OtherArmyCtrl eatArmyctrl = null;//选中的吞噬单位

        Color eatcolor = new Color(95f / 255f, 242f / 255f, 10f / 255f);
        public OrcGiantSkill()
        {
            //this.armyCtrl = armyCtrl;
            canPerTurn = true;
            isPassive = false;
            isNeedOtherDead = false;
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
            eatArmyctrl = getOtherArmyCtrlByMouse();
        }

        public override void TurnRefrsh()
        {
            isUsed = false;
        }


        /// <summary>
        /// 吞噬
        /// </summary>
        public override void UseSkill()
        {
            if (!isUsed)
            {
                StartCoroutine(EatOther());
            }


            //Dispatch(AreoCode.UI, UIEvent.PROMPT_PANEL_EVENTCODE, "巨口蛙发动吞噬");


        }


        private IEnumerator EatOther()
        {
            eatArmyctrl = null;
            //isStartSelect = true;

            Dispatch(AreoCode.UI, UIEvent.CURSOR_SET_HEART, "治疗光标");
            List<MapPoint> caneatPoints = MapAttackType.Instance.GetAttakRange(armyCtrl.armyState);
            List<MapPointCtrl> caneatMapCtrls = MapTools.getMapCtrlByMapPoint(caneatPoints);
            MapTools.setMappointCtrlColor(caneatMapCtrls, eatcolor);
            yield return new WaitUntil(isSelecteateArmy);


            bool caneat = false;
            foreach (var item in caneatMapCtrls)
            {
                if (eatArmyctrl.armyState.Position.X == item.mapPoint.X
                    && eatArmyctrl.armyState.Position.Z == item.mapPoint.Z)
                {
                    caneat = true;
                    break;
                }
            }
            if (!caneat)
            {
                Dispatch(AreoCode.UI, UIEvent.CURSOR_SET_NORMAL, "正常光标");
                Dispatch(AreoCode.UI, UIEvent.PROMPT_PANEL_EVENTCODE, "不在吞噬范围内");
                MapTools.setMappointCtrlColor(caneatMapCtrls);
                eatArmyctrl = null;
                yield break;
            }

            if (eatArmyctrl.armyState.Class == ArmyClassType.HighClass ||
                eatArmyctrl.armyState.Class == ArmyClassType.Hero)
            {
                Dispatch(AreoCode.UI, UIEvent.CURSOR_SET_NORMAL, "正常光标");
                Dispatch(AreoCode.UI, UIEvent.PROMPT_PANEL_EVENTCODE, "无法吞噬髙阶或英雄单位");
                MapTools.setMappointCtrlColor(caneatMapCtrls);
                eatArmyctrl = null;
                yield break;
            }






            eatArmyctrl.armyState.Hp = 0;
            Dispatch(AreoCode.UI, UIEvent.CURSOR_SET_NORMAL, "正常光标");
            Dispatch(AreoCode.UI, UIEvent.PROMPT_PANEL_EVENTCODE, "吞噬成功");



            Dispatch(AreoCode.UI, UIEvent.CLOSE_ARMY_MENU_PANEL, "关闭单位功能面板");
            Dispatch(AreoCode.UI, UIEvent.SHOW_ARMY_MENU_PANEL, armyCtrl);
            isUsed = true;

            //给对方发送消息
            MapPoint targetMappoint = eatArmyctrl.armyState.Position;
            skillDto.Change(armyCtrl.armyState.Race, armyCtrl.armyState.Name, eatArmyctrl.armyState.Name, targetMappoint);
            socketMsg.Change(OpCode.FIGHT, FightCode.ARMY_USE_SKILL_CREQ, skillDto);
            Dispatch(AreoCode.NET, NetEvent.SENDMSG, socketMsg);

            MapTools.setMappointCtrlColor(caneatMapCtrls);
        }

        /// <summary>
        /// 是否选择了要吞噬的单位
        /// </summary>
        /// <returns></returns>
        private bool isSelecteateArmy()
        {
            if (eatArmyctrl != null)
            {
                return true;
            }
            return false;
        }
    }
}
